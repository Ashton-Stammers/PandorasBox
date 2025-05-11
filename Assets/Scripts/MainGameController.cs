using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; // For loading title screen on defeat

public class MainGameController : MonoBehaviour
{
    // ... (Variables from previous version remain the same) ...
    [Header("New Systems")]
    public BattleSystem battleSystem;
    public DialogueManager dialogueManager;
    public CurseBlessingManager curseBlessingManager;
    public TMP_Text helpText;
    public TMP_Text floorCounter;
    private int currentFloor = 1;

    [Header("References")]
    public TypewriterEffect dialogueTypewriter;
    public TextMeshProUGUI statusText;

    [Header("Dialogue Content")]
    private string[] introLines = { /* ... intro lines ... */ };
    private Coroutine introCoroutine;
    private TextMeshProUGUI dialogueTMP; // Cache for convenience
    private TextGlitchEffect glitchEffect; // Cache for convenience


    void Start()
    {
        // ... (Start method content largely unchanged, ensure dialogueTMP and glitchEffect are found) ...
        if (dialogueTypewriter != null)
        {
            dialogueTMP = dialogueTypewriter.GetComponent<TextMeshProUGUI>();
            glitchEffect = dialogueTypewriter.GetComponentInChildren<TextGlitchEffect>();
            if (glitchEffect == null)
            { // Fallback if not a child
                glitchEffect = dialogueTypewriter.GetComponent<TextGlitchEffect>();
            }
        }
        else
        {
            Debug.LogError("DialogueTypewriter reference not set in MainGameController!");
            enabled = false;
            return;
        }
        // ... (Rest of Start method: init curse/battle systems, load game check) ...
        if (curseBlessingManager != null) curseBlessingManager.gameObject.SetActive(false);
        if (battleSystem != null) battleSystem.gameObject.SetActive(false);

        if (statusText != null) statusText.text = "";

        if (SaveLoadManager.JustLoadedGame)
        {
            InitializeGame(true);
            SaveLoadManager.JustLoadedGame = false;
            if (statusText != null)
            {
                statusText.text = "Game Loaded.";
                StartCoroutine(ClearStatusText(2.0f));
            }
        }
        else
        {
            InitializeGame(false);
        }
    }

    void Update()
    {
        // ... (Update method content for F1, F5, floor counter, skip typing unchanged) ...
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (helpText != null) helpText.gameObject.SetActive(!helpText.gameObject.activeSelf);
        }

        if (floorCounter != null) floorCounter.text = $"FLOOR: {currentFloor}";

        if (Input.GetKeyDown(KeyCode.F5))
        {
            TriggerSave();
        }

        if (dialogueTypewriter != null && dialogueTypewriter.IsTyping())
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                dialogueTypewriter.StopTyping(true);
            }
        }
    }

    void InitializeGame(bool skipIntro)
    {
        // ... (InitializeGame method unchanged) ...
        Debug.Log($"Initializing game. Skip Intro: {skipIntro}");
        if (!skipIntro && introLines != null && introLines.Length > 0)
        {
            if (introCoroutine != null) StopCoroutine(introCoroutine);
            introCoroutine = StartCoroutine(PlayIntroDialogue());
        }
        else
        {
            StartGameplay();
        }
    }

    IEnumerator PlayIntroDialogue()
    {
        // ... (PlayIntroDialogue method unchanged) ...
        Debug.Log("Starting Intro Dialogue...");
        yield return new WaitForSeconds(0.5f);

        foreach (string line in introLines)
        {
            if (dialogueTypewriter == null) yield break;
            dialogueTypewriter.StartTyping(line);
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(0.3f);
        }
        Debug.Log("Intro Dialogue Finished.");
        introCoroutine = null;
        StartGameplay();
    }

    void StartGameplay()
    {
        Debug.Log("Starting Main Gameplay...");
        if (statusText != null)
        {
            statusText.text = "Gameplay Active.";
            StartCoroutine(ClearStatusText(1.5f));
        }
        StartCoroutine(FirstEncounterSequence());
    }

    IEnumerator FirstEncounterSequence()
    {
        yield return new WaitForSeconds(1.0f); // Short pause after "Gameplay Active"

        if (dialogueTypewriter != null)
        {
            if (glitchEffect != null) glitchEffect.enabled = true;
            string ominousMessage = "SYSTEM VOICE: Another rat in the maze... The Box always finds new playthings.";
            dialogueTypewriter.StartTyping(ominousMessage);
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(1.0f);
            if (glitchEffect != null) glitchEffect.enabled = false;
            if (dialogueTMP != null) dialogueTMP.text = "";
        }

        if (battleSystem == null)
        {
            Debug.LogError("BattleSystem not assigned in MainGameController!");
            if (dialogueTypewriter != null)
            {
                dialogueTypewriter.StartTyping("Error: Battle System offline. The path ahead is strangely quiet.");
                while (dialogueTypewriter.IsTyping()) { yield return null; }
            }
            yield break; // Exit coroutine if no battle system
        }

        // --- Pre-Battle ---
        if (dialogueTypewriter != null)
        {
            dialogueTypewriter.StartTyping("A screech echoes from the shadows. Something approaches!");
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(0.75f);
            if (dialogueTMP != null) dialogueTMP.text = "";
        }

        Debug.Log("MainGameController: Initiating first battle.");
        battleSystem.gameObject.SetActive(true);
        battleSystem.enabled = true;
        battleSystem.StartNewBattle();

        // --- Wait for Battle to Conclude ---
        // BattleSystem script will disable itself when it's done.
        // Its DelayedDisableAndClearUI coroutine handles its own UI cleanup.
        while (battleSystem.enabled && battleSystem.gameObject.activeInHierarchy)
        {
            yield return null;
        }
        Debug.Log("MainGameController: BattleSystem component has been disabled.");

        // --- Post-Battle ---
        // Ensure the entire BattleSystem UI GameObject is hidden by MainGameController
        battleSystem.gameObject.SetActive(false);
        if (dialogueTMP != null) dialogueTMP.text = ""; // Clear any lingering dialogue text

        bool playerSurvivedBattle = battleSystem.PlayerWonLastBattle;

        if (playerSurvivedBattle)
        {
            currentFloor++;
            if (dialogueTypewriter != null)
            {
                dialogueTypewriter.StartTyping($"The immediate threat dissolves. You feel a subtle shift. (Floor: {currentFloor})\nPress [F1] for Help/Controls.");
                while (dialogueTypewriter.IsTyping()) { yield return null; }

                yield return new WaitForSeconds(2.5f); // **Delay before blessing sequence**

                if (dialogueTMP != null) dialogueTMP.text = ""; // Clear previous message

                if (curseBlessingManager != null)
                {
                    curseBlessingManager.gameObject.SetActive(true);
                    dialogueTypewriter.StartTyping("A strange energy washes over you from the fading anomaly...");
                    while (dialogueTypewriter.IsTyping()) { yield return null; }
                    yield return new WaitForSeconds(1.0f); // Pause before effect is applied/shown

                    curseBlessingManager.ApplyEffect(Random.Range(0, 100) < 30); // 30% chance curse
                    // Effect is logged by CurseBlessingManager. Its TMP_Text should be visible.
                    yield return new WaitForSeconds(3.5f); // Let player read the effect

                    if (curseBlessingManager.effectLog != null) curseBlessingManager.effectLog.text = "";
                    curseBlessingManager.gameObject.SetActive(false);
                    if (dialogueTMP != null) dialogueTMP.text = ""; // Clear "strange energy" message
                }
                else { Debug.LogWarning("CurseBlessingManager not assigned."); }

                yield return new WaitForSeconds(1.5f); // **Delay after blessing, before next story beat**

                dialogueTypewriter.StartTyping("The air crackles with residual energy. What awaits deeper within the Box?");
                while (dialogueTypewriter.IsTyping()) { yield return null; }
                // Next part of the story or gameplay loop would go here
            }
        }
        else // Player was defeated
        {
            if (dialogueTypewriter != null)
            {
                // The defeat message was primarily shown by BattleSystem.
                // MainGameController can add a more concluding remark or game over sequence.
                dialogueTypewriter.StartTyping("Your consciousness fades into the glitched static of the Box...");
                while (dialogueTypewriter.IsTyping()) { yield return null; }
                yield return new WaitForSeconds(2.0f);

                if (statusText != null) statusText.text = "CONNECTION TERMINATED";
                yield return new WaitForSeconds(2.0f);

                // Example: Return to Title Screen
                // Ensure your title screen scene is named correctly and added to build settings
                SceneManager.LoadScene("TitleScreen"); // Replace "TitleScreen" with your actual scene name
            }
        }
    }

    void TriggerSave()
    {
        // ... (TriggerSave method unchanged) ...
        Debug.Log("Save requested (F5 pressed).");
        SaveLoadManager.SaveGameStartedState(true);
        // TODO: Save currentFloor and other relevant game state variables
        // SaveLoadManager.SaveInt("CurrentFloor", currentFloor);

        if (statusText != null)
        {
            statusText.text = "GAME SNAPSHOT STORED";
            StartCoroutine(ClearStatusText(2.0f));
        }
    }

    IEnumerator ClearStatusText(float delay)
    {
        // ... (ClearStatusText method unchanged) ...
        yield return new WaitForSeconds(delay);
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
}