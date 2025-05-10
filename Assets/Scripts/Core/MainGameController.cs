using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshProUGUI

// Manages the main game flow, including starting dialogue and handling saving.
public class MainGameController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the GameObject with the TypewriterEffect script for dialogue.")]
    public TypewriterEffect dialogueTypewriter;
    [Tooltip("Optional TextMeshProUGUI component to display save/load status.")]
    public TextMeshProUGUI statusText; // Optional

    [Header("Dialogue Content")]
    // Initial dialogue lines for the game start
    private string[] introLines = {
        "SYSTEM BOOTING...",
        "ACCESSING ARCHIVE :: PANDORA.BOX",
        "WARNING: CORE INTEGRITY COMPROMISED. CONTAINMENT FIELD FLUCTUATING.",
        "INITIATING INTERFACE...",
        "Welcome, User.",
        "CRITICAL ALERT: ARTIFACT STABILITY AT 12%",
        "CONTAINMENT BREACH IMMINENT. ACTIVATE PROTOCOLS?",
        "USER AUTHORIZATION REQUIRED...",
        "SCANNING BIOMETRICS...",
        "ACCESS GRANTED: USER LEVEL DELTA",
        "WARNING: PROTOCOL ACTIVATION MAY ACCELERATE CORE FAILURE",
        "CHOOSE WISELY... [PRESS Y/N]"
    };
    private bool waitingForChoice = false;
    private string[] activateProtocolLines = {
        "PROTOCOLS ENGAGED.",
        "CONTAINMENT FIELD STABILIZING...",
        "WARNING: UNKNOWN ENTITY DETECTED IN CORE!"
    };

    private string[] denyProtocolLines = {
        "PROTOCOLS DENIED.",
        "CONTAINMENT FIELD COLLAPSING...",
        "ENTITY RELEASE INEVITABLE."
    };

    private Coroutine introCoroutine;

    // New variables for Save/Load integration
    private int currentDialogueIndex;
    private string[] currentDialogueLines;
    private bool choseYPath;

    void Start()
    {
        if (dialogueTypewriter == null)
        {
            Debug.LogError("DialogueTypewriter reference not set in MainGameController! Assign it in the Inspector.");
            enabled = false; // Disable if reference is missing
            return;
        }

        // Clear status text initially if assigned
        if (statusText != null)
        {
            statusText.text = "";
        }

        // Check the static flag from SaveLoadManager to see if we loaded a game
        if (SaveLoadManager.JustLoadedGame)
        {
            InitializeGame(true); // Skip intro if loaded
            SaveLoadManager.JustLoadedGame = false; // Reset flag after checking
            if (statusText != null)
            {
                statusText.text = "Game Loaded.";
                StartCoroutine(ClearStatusText(2.0f)); // Clear after 2 seconds
            }
        }
        else
        {
            InitializeGame(false); // Play intro for new game
        }
    }

    void Update()
    {
        // --- Input Handling ---

        // Check for save key press (e.g., F5) using legacy Input Manager
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TriggerSave();
        }

        // Optional: Check for skip input during typing (e.g., Space or Enter/Return)
        if (dialogueTypewriter != null && dialogueTypewriter.IsTyping())
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                dialogueTypewriter.StopTyping(true); // Stop typing and reveal full line
            }
        }

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                waitingForChoice = false;
                choseYPath = true; // Save choice
                StartCoroutine(PlayChoiceDialogue(activateProtocolLines));
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                waitingForChoice = false;
                choseYPath = false; // Save choice
                StartCoroutine(PlayChoiceDialogue(denyProtocolLines));
            }
        }

        // Add save/load testing commands
        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveLoadManager.DeleteAllSaveData();
            Debug.Log("Deleted all save data");
            if (statusText != null)
            {
                statusText.text = "All Data Deleted!";
                StartCoroutine(ClearStatusText(2.0f));
            }
        }
    }

    /// <summary>
    /// Initializes the game state, potentially skipping the intro.
    /// </summary>
    /// <param name="skipIntro">Whether to skip the intro dialogue.</param>
    void InitializeGame(bool skipIntro)
    {
        bool choiceMade = false;
        bool loadedChoseY = false;
        int loadedIndex = 0;
        string[] loadedLines = null;

        if (SaveLoadManager.HasSaveData())
        {
            SaveLoadManager.LoadChoiceState(out choiceMade, out loadedChoseY,
                                          out loadedIndex, out loadedLines);
        }

        if (choiceMade && loadedLines != null)
        {
            Debug.Log("Loading interrupted story path...");
            currentDialogueIndex = loadedIndex;
            currentDialogueLines = loadedLines;
            choseYPath = loadedChoseY;
            StartCoroutine(ResumeInterruptedPath());
        }
        else if (!skipIntro)
        {
            StartCoroutine(PlayIntroDialogue());
        }
        else
        {
            StartGameplay();
        }
    }

    /// <summary>
    /// Coroutine to resume an interrupted dialogue path.
    /// </summary>
    private IEnumerator ResumeInterruptedPath()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = currentDialogueIndex; i < currentDialogueLines.Length; i++)
        {
            dialogueTypewriter.StartTyping(currentDialogueLines[i]);

            while (dialogueTypewriter.IsTyping())
            {
                yield return null;
            }

            currentDialogueIndex = i + 1;
            SaveGameState();
            yield return new WaitForSeconds(0.3f);
        }

        ContinueAfterChoice();
    }

    /// <summary>
    /// Handles the save game action.
    /// </summary>
    void TriggerSave()
    {
        SaveGameState();

        if (statusText != null)
        {
            statusText.text = "Game Saved";
            StartCoroutine(ClearStatusText(2.0f));
        }
    }

    /// <summary>
    /// Save the current game state.
    /// </summary>
    private void SaveGameState()
    {
        bool saveChoiceMade = currentDialogueLines != null && currentDialogueLines.Length > 0;

        SaveLoadManager.SaveGameStartedState(true);
        SaveLoadManager.SaveChoiceState(
            saveChoiceMade,
            choseYPath,
            currentDialogueIndex,
            currentDialogueLines ?? new string[0]
        );

        Debug.Log($"Saved game state. ChoiceMade: {saveChoiceMade}, Index: {currentDialogueIndex}");
    }

    /// <summary>
    /// Coroutine to play the intro dialogue lines one by one.
    /// </summary>
    IEnumerator PlayIntroDialogue()
    {
        Debug.Log("Starting Intro Dialogue...");
        yield return new WaitForSeconds(0.5f); // Small delay before starting

        foreach (string line in introLines)
        {
            if (dialogueTypewriter == null) yield break; // Stop if reference lost

            dialogueTypewriter.StartTyping(line);

            // Wait until the current line is finished typing (or skipped by input)
            while (dialogueTypewriter.IsTyping())
            {
                yield return null; // Wait for the next frame
            }

            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("Intro Dialogue Finished.");
        introCoroutine = null; // Clear the coroutine reference

        StartGameplay();
    }

    /// <summary>
    /// Called when the intro is skipped or finished. Starts the main game loop/state.
    /// </summary>
    void StartGameplay()
    {
        Debug.Log("Starting Main Gameplay...");
        waitingForChoice = true; // Enable choice input
        if (statusText != null)
        {
            statusText.text = "Gameplay Active.";
            StartCoroutine(ClearStatusText(3.0f));
        }
    }

    /// <summary>
    /// Coroutine to play choice dialogue lines.
    /// </summary>
    private IEnumerator PlayChoiceDialogue(string[] lines)
    {
        currentDialogueLines = lines;
        currentDialogueIndex = 0;
        SaveGameState();

        for (int i = 0; i < lines.Length; i++)
        {
            dialogueTypewriter.StartTyping(lines[i]);
            currentDialogueIndex = i;
            SaveGameState();

            while (dialogueTypewriter.IsTyping())
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }

        ContinueAfterChoice();
    }

    /// <summary>
    /// Clears the dialogue state and proceeds with the story after a choice.
    /// </summary>
    private void ContinueAfterChoice()
    {
        Debug.Log("Continuing story after choice...");
        currentDialogueLines = null;
        currentDialogueIndex = 0;
        SaveGameState();

        // Add your post-choice gameplay logic here
        if (statusText != null)
        {
            statusText.text = choseYPath ?
                "System: Protocol Engaged" :
                "System: Protocol Denied";
            StartCoroutine(ClearStatusText(3.0f));
        }
    }

    /// <summary>
    /// Simple coroutine to clear the status text after a delay.
    /// </summary>
    IEnumerator ClearStatusText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
}