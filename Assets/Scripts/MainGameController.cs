using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; // For loading title screen on defeat

public class MainGameController : MonoBehaviour
{
    [Header("New Systems")]
    public BattleSystem battleSystem;
    public DialogueManager dialogueManager; // Though not directly used in this expansion, kept for consistency
    public CurseBlessingManager curseBlessingManager;
    public TMP_Text helpText;
    public TMP_Text floorCounter;
    private int currentFloor = 1;
    private const int MAX_FLOORS = 5; // Define the total number of floors

    [Header("References")]
    public TypewriterEffect dialogueTypewriter;
    public TextMeshProUGUI statusText;

    [Header("Dialogue Content")]
    // introLines can remain for the initial game intro if desired
    private string[] introLines = {
        "SYSTEM BOOTING...",
        "WELCOME TO THE PANDORA PROTOCOL.",
        "ANALYZING SUBJECT... DESIGNATION: INTRUDER.",
        "OBJECTIVE: DESCEND. INVESTIGATE. SURVIVE?",
        "THE BOX AWAITS."
    };
    private Coroutine gameCoroutine; // Renamed from introCoroutine, will handle the main game loop
    private TextMeshProUGUI dialogueTMP; // Cache for convenience
    private TextGlitchEffect glitchEffect; // Cache for convenience


    void Start()
    {
        if (dialogueTypewriter != null)
        {
            dialogueTMP = dialogueTypewriter.GetComponent<TextMeshProUGUI>();
            glitchEffect = dialogueTypewriter.GetComponentInChildren<TextGlitchEffect>();
            if (glitchEffect == null)
            {
                glitchEffect = dialogueTypewriter.GetComponent<TextGlitchEffect>();
            }
        }
        else
        {
            Debug.LogError("DialogueTypewriter reference not set in MainGameController!");
            enabled = false;
            return;
        }

        if (curseBlessingManager != null) curseBlessingManager.gameObject.SetActive(false);
        if (battleSystem != null) battleSystem.gameObject.SetActive(false);

        if (statusText != null) statusText.text = "";

        if (SaveLoadManager.JustLoadedGame)
        {
            // TODO: Load currentFloor and other relevant data here
            // For now, we'll assume a loaded game might start at a specific floor or reset.
            // Let's say loading always starts you at floor 1 for this example, but clears the flag.
            currentFloor = PlayerPrefs.GetInt("CurrentFloor_Pandora", 1); // Example loading
            InitializeGame(true); // Skip intro dialogue
            SaveLoadManager.JustLoadedGame = false;
            if (statusText != null)
            {
                statusText.text = "Game Loaded. Floor: " + currentFloor;
                StartCoroutine(ClearStatusText(2.0f));
            }
        }
        else
        {
            currentFloor = 1; // Fresh game starts at floor 1
            InitializeGame(false); // Play intro
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (helpText != null) helpText.gameObject.SetActive(!helpText.gameObject.activeSelf);
        }

        if (floorCounter != null) floorCounter.text = $"FLOOR: {currentFloor}/{MAX_FLOORS}";

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
        Debug.Log($"Initializing game. Skip Intro: {skipIntro}. Current Floor: {currentFloor}");
        if (!skipIntro && introLines != null && introLines.Length > 0 && currentFloor == 1) // Only play full intro on a fresh game's first floor
        {
            if (gameCoroutine != null) StopCoroutine(gameCoroutine);
            gameCoroutine = StartCoroutine(PlayIntroAndStartGameplay());
        }
        else
        {
            StartGameplay(); // Directly start gameplay if skipping intro or on a loaded game
        }
    }

    IEnumerator PlayIntroAndStartGameplay()
    {
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
        StartGameplay();
    }

    void StartGameplay()
    {
        Debug.Log("Starting Main Gameplay Loop for Floor: " + currentFloor);
        if (statusText != null && currentFloor == 1 && !SaveLoadManager.JustLoadedGame) // Avoid "Gameplay Active" on load
        {
            statusText.text = "Gameplay Active.";
            StartCoroutine(ClearStatusText(1.5f));
        }
        if (gameCoroutine != null) StopCoroutine(gameCoroutine);
        gameCoroutine = StartCoroutine(PlayFloorSequence());
    }

    IEnumerator PlayFloorSequence()
    {
        // --- Floor-Specific Story Beat ---
        yield return StartCoroutine(DisplayFloorStoryBeat());

        if (battleSystem == null)
        {
            Debug.LogError("BattleSystem not assigned in MainGameController!");
            if (dialogueTypewriter != null)
            {
                dialogueTypewriter.StartTyping("CRITICAL ERROR: COMBAT SUBSYSTEM OFFLINE. Unable to proceed.");
                while (dialogueTypewriter.IsTyping()) { yield return null; }
            }
            yield break;
        }

        // --- Pre-Battle Message ---
        if (dialogueTypewriter != null)
        {
            string preBattleMessage = GetPreBattleMessage();
            dialogueTypewriter.StartTyping(preBattleMessage);
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(0.75f);
            if (dialogueTMP != null) dialogueTMP.text = "";
        }

        Debug.Log($"MainGameController: Initiating battle for Floor {currentFloor}.");
        battleSystem.gameObject.SetActive(true);
        battleSystem.enabled = true;
        battleSystem.StartNewBattle(); // Consider passing floor number if BattleSystem can use it for difficulty

        while (battleSystem.enabled && battleSystem.gameObject.activeInHierarchy)
        {
            yield return null;
        }
        Debug.Log("MainGameController: BattleSystem component has been disabled.");

        battleSystem.gameObject.SetActive(false);
        if (dialogueTMP != null) dialogueTMP.text = "";

        bool playerSurvivedBattle = battleSystem.PlayerWonLastBattle;

        if (playerSurvivedBattle)
        {
            // --- Post-Battle Victory Logic ---
            if (dialogueTypewriter != null)
            {
                dialogueTypewriter.StartTyping(GetVictoryMessage());
                while (dialogueTypewriter.IsTyping()) { yield return null; }
                yield return new WaitForSeconds(1.5f); // Time to read victory message
                if (dialogueTMP != null) dialogueTMP.text = "";
            }

            // --- Curse/Blessing ---
            if (curseBlessingManager != null)
            {
                curseBlessingManager.gameObject.SetActive(true);
                if (dialogueTypewriter != null)
                {
                    dialogueTypewriter.StartTyping("A strange energy washes over you from the vanquished anomaly...");
                    while (dialogueTypewriter.IsTyping()) { yield return null; }
                }
                yield return new WaitForSeconds(1.0f);

                curseBlessingManager.ApplyEffect(Random.Range(0, 100) < 30 + (currentFloor * 5)); // Slightly increasing curse chance

                yield return new WaitForSeconds(3.5f); // Let player read the effect

                if (curseBlessingManager.effectLog != null) curseBlessingManager.effectLog.text = "";
                curseBlessingManager.gameObject.SetActive(false);
                if (dialogueTMP != null) dialogueTMP.text = "";
            }
            else { Debug.LogWarning("CurseBlessingManager not assigned."); }

            yield return new WaitForSeconds(0.5f);

            // --- Floor Progression / Game End ---
            currentFloor++;
            if (currentFloor <= MAX_FLOORS)
            {
                if (dialogueTypewriter != null)
                {
                    dialogueTypewriter.StartTyping(GetFloorTransitionMessage());
                    while (dialogueTypewriter.IsTyping()) { yield return null; }
                    yield return new WaitForSeconds(2.0f);
                }
                StartGameplay(); // Proceed to the next floor
            }
            else
            {
                // Player has cleared all floors
                yield return StartCoroutine(PlayGameWinSequence());
            }
        }
        else
        {
            // --- Player Defeat Logic ---
            yield return StartCoroutine(PlayPlayerDefeatSequence());
        }
    }

    IEnumerator DisplayFloorStoryBeat()
    {
        if (dialogueTypewriter == null) yield break;
        string storyMessage = "";
        bool useGlitch = false;

        switch (currentFloor)
        {
            case 1:
                useGlitch = true;
                storyMessage = "SYSTEM VOICE: Another rat in the maze... The Box always finds new playthings.";
                break;
            case 2:
                useGlitch = true;
                storyMessage = "SYSTEM VOICE: Curiosity is a strong motivator. Predictable. The patterns here are... self-correcting.";
                break;
            case 3:
                useGlitch = false; // Normal dialogue for a change
                storyMessage = "A barely audible whisper seems to emanate from the walls...\n\"...hope trapped... an echo of a choice...\"";
                yield return new WaitForSeconds(1.0f); // Extra pause before this one
                if (dialogueTypewriter != null)
                {
                    dialogueTypewriter.StartTyping(storyMessage);
                    while (dialogueTypewriter.IsTyping()) { yield return null; }
                    yield return new WaitForSeconds(2.0f);
                    if (glitchEffect != null) glitchEffect.enabled = true; // Glitch the next part
                    dialogueTypewriter.StartTyping("SYSTEM VOICE: Irrelevant noise. Focus on your parameters, subject.");
                    while (dialogueTypewriter.IsTyping()) { yield return null; }
                    if (glitchEffect != null) glitchEffect.enabled = false;
                    yield return new WaitForSeconds(1.5f);
                    if (dialogueTMP != null) dialogueTMP.text = "";
                    yield break; // Custom handling for this floor, skip default display
                }
                break;
            case 4:
                useGlitch = true;
                storyMessage = "SYSTEM VOICE: Your persistence is... an anomaly. Anomalies must be cataloged. And purged.";
                break;
            case 5:
                useGlitch = true;
                storyMessage = "SYSTEM VOICE: So, the rat reaches the designated termination point of this sub-routine. Commendable. But the Box demands order. And you, entity, are manifest chaos.";
                break;
        }

        if (!string.IsNullOrEmpty(storyMessage))
        {
            if (glitchEffect != null) glitchEffect.enabled = useGlitch;
            dialogueTypewriter.StartTyping(storyMessage);
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(1.5f);
            if (glitchEffect != null) glitchEffect.enabled = false;
            if (dialogueTMP != null) dialogueTMP.text = "";
        }
        yield return new WaitForSeconds(0.5f); // General pause after story beat
    }

    string GetPreBattleMessage()
    {
        switch (currentFloor)
        {
            case 1: return "A screech echoes from the shadows. Something approaches!";
            case 2: return "The air thickens. Another anomaly detected ahead!";
            case 3: return "Distorted figures coalesce in the digital haze. Hostile contact imminent!";
            case 4: return "Warning! Elevated threat signature approaching rapidly!";
            case 5: return "SYSTEM ALERT: Apex Anomaly. PRIME GUARDIAN ENGAGED. Resistance is futile.";
            default: return "Something stirs nearby...";
        }
    }

    string GetVictoryMessage()
    {
        return $"Anomaly neutralized. Threat level on Floor {currentFloor} reduced.";
    }

    string GetFloorTransitionMessage()
    {
        return $"The path to Floor {currentFloor} stabilizes. The descent continues...";
    }

    IEnumerator PlayGameWinSequence()
    {
        if (dialogueTypewriter != null)
        {
            if (dialogueTMP != null) dialogueTMP.text = ""; // Clear previous

            if (glitchEffect != null) glitchEffect.enabled = true;
            dialogueTypewriter.StartTyping("SYSTEM VOICE: Sub-routine complete. Anomaly... contained? No... Transformed.");
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(1.5f);

            dialogueTypewriter.StartTyping("The digital fabric around you shudders violently... then goes still.");
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(2.0f);
            if (glitchEffect != null) glitchEffect.enabled = false;

            dialogueTypewriter.StartTyping("A single, fragile prompt appears:\n[1] CONTINUE DEEPER?\n[2] ATTEMPT INTEGRITY RESET?");
            while (dialogueTypewriter.IsTyping()) { yield return null; }

            // For now, this is an endpoint. Game would need input handling for this choice.
            if (statusText != null) statusText.text = "THE BOX WATCHES... WHAT NEXT?";
            yield return new WaitForSeconds(5.0f); // Wait for player to read
            SceneManager.LoadScene("TitleScreen"); // Or a credits screen
        }
    }

    IEnumerator PlayPlayerDefeatSequence()
    {
        if (dialogueTypewriter != null)
        {
            if (glitchEffect != null) glitchEffect.enabled = true;
            dialogueTypewriter.StartTyping("SYSTEM VOICE: Subject viability terminated. Resetting parameters...");
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(1.0f);
            if (glitchEffect != null) glitchEffect.enabled = false;

            dialogueTypewriter.StartTyping("Your consciousness fades into the glitched static of the Box...");
            while (dialogueTypewriter.IsTyping()) { yield return null; }
            yield return new WaitForSeconds(2.0f);

            if (statusText != null) statusText.text = "CONNECTION TERMINATED";
            yield return new WaitForSeconds(3.0f);
            SceneManager.LoadScene("TitleScreen");
        }
    }

    void TriggerSave()
    {
        Debug.Log("Save requested (F5 pressed). Floor: " + currentFloor);
        SaveLoadManager.SaveGameStartedState(true);
        PlayerPrefs.SetInt("CurrentFloor_Pandora", currentFloor); // Example saving current floor
        PlayerPrefs.Save(); // Ensure it's written

        if (statusText != null)
        {
            statusText.text = "GAME SNAPSHOT STORED (FLOOR " + currentFloor + ")";
            StartCoroutine(ClearStatusText(2.0f));
        }
    }

    IEnumerator ClearStatusText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
}