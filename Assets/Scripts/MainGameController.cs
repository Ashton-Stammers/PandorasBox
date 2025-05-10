using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshProUGUI

// Manages the main game flow, including starting dialogue and handling saving.
public class MainGameController : MonoBehaviour
{

    [Header("New Systems")]
    public BattleSystem battleSystem;
    public DialogueManager dialogueManager;
    public TMP_Text helpText;
    public TMP_Text floorCounter;
    private int currentFloor = 1;

    [Header("References")]
    [Tooltip("Assign the GameObject with the TypewriterEffect script for dialogue.")]
    public TypewriterEffect dialogueTypewriter;
    [Tooltip("Optional TextMeshProUGUI component to display save/load status.")]
    public TextMeshProUGUI statusText; // Optional

    [Header("Dialogue Content")]
    // Initial dialogue lines for the game start
    private string[] introLines = {
    "██╗░░██╗███████╗██╗░░░░░██████╗░  ██████╗░███████╗██╗░░██╗",
    "██║░░██║██╔════╝██║░░░░░██╔══██╗  ██╔══██╗██╔════╝╚██╗██╔╝",
    "███████║█████╗░░██║░░░░░██║░░██║  ██████╦╝█████╗░░░╚███╔╝░",
    "██╔══██║██╔══╝░░██║░░░░░██║░░██║  ██╔══██╗██╔══╝░░░██╔██╗░",
    "██║░░██║███████╗███████╗██████╔╝  ██████╦╝███████╗██╔╝╚██╗",
    "╚═╝░░╚═╝╚══════╝╚══════╝╚═════╝░  ╚═════╝░╚══════╝╚═╝░░╚═╝",
    "ACCESSING ARCHIVE :: PANDORA.BOX...",
    "WARNING: CORE INTEGRITY COMPROMISED. CONTAINMENT FIELD AT 12%.",
    "DETECTING ANOMALOUS ENTITIES: [██████████] 100%",
    "INITIALIZING EMERGENCY PROTOCOL 'ELPIS'... FAILED.",
    "USER DETECTED. WELCOME, CONTAINMENT OFFICER.",
    "YOUR MISSION: NAVIGATE THE CHAOS. SURVIVE THE BOX.",
    "LOADING PROTOCOL  [F1:HELP] [F2:STATS] [F5:QUICKSAVE]"
};

    private Coroutine introCoroutine;

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
        // Assuming SaveLoadManager exists and works as expected
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


        if (Input.GetKeyDown(KeyCode.F1))
        {
            helpText.gameObject.SetActive(!helpText.gameObject.activeSelf);
        }

        // Floor counter
        floorCounter.text = $"FLOOR: {currentFloor}";

        // Check for save key press (e.g., F5) using legacy Input Manager
        if (Input.GetKeyDown(KeyCode.F5)) // [13, 26, 67, 103, 104, 20, 59, 105, 48]
        {
            TriggerSave();
        }

        // Optional: Check for skip input during typing (e.g., Space or Enter/Return)
        // Ensure dialogueTypewriter and the coroutine are active
        if (dialogueTypewriter != null && dialogueTypewriter.IsTyping())
        {
            // Use GetKeyDown to register the press only once per tap
            // Use || for logical OR
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                dialogueTypewriter.StopTyping(true); // Stop typing and reveal full line
                                                     // Note: This only skips the *current* line's typing animation.
                                                     // To skip the entire intro sequence, more logic would be needed (e.g., stopping the introCoroutine).
            }
        }
        // --- End Input Handling ---
    } // End of Update method

    /// <summary>
    /// Initializes the game state, potentially skipping the intro.
    /// </summary>
    /// <param name="skipIntro">Whether to skip the intro dialogue.</param>
    void InitializeGame(bool skipIntro)
    {
        Debug.Log($"Initializing game. Skip Intro: {skipIntro}");
        if (!skipIntro && introLines != null && introLines.Length > 0)
        {
            // Start the intro dialogue sequence if not skipping and lines exist
            if (introCoroutine != null) StopCoroutine(introCoroutine); // Stop if already running somehow
            introCoroutine = StartCoroutine(PlayIntroDialogue());
        }
        else
        {
            // If skipping intro or no intro lines, go directly to gameplay state
            StartGameplay();
        }
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

            // Optional delay between lines after one finishes
            // You could use dialogueTypewriter.delayBetweenLines here if needed
            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("Intro Dialogue Finished.");
        introCoroutine = null; // Clear the coroutine reference

        // Transition to main gameplay after intro
        StartGameplay();
    }

    /// <summary>
    /// Called when the intro is skipped or finished. Starts the main game loop/state.
    /// </summary>
    void StartGameplay()
    {
        Debug.Log("Starting Main Gameplay...");
        // --- Add your actual gameplay initialization logic here ---
        // Example: Enable player controls, spawn enemies, load level data etc.
        if (statusText != null)
        {
            statusText.text = "Gameplay Active.";
            StartCoroutine(ClearStatusText(3.0f));
        }
    }


    /// <summary>
    /// Handles the save game action.
    /// </summary>
    void TriggerSave()
    {
        Debug.Log("Save requested (F5 pressed).");

        void TriggerSave()
        {
            SaveLoadManager.SaveGameStartedState(true);
            // Add more save data as needed
            if (statusText != null)
            {
                statusText.text = "GAME SNAPSHOT STORED";
            }
        }

        if (statusText != null)
        {
            statusText.text = "Saving Game...";
            // Assuming saving is quick. If not, update text upon completion.
            StartCoroutine(ClearStatusText(2.0f));
        }
        else
        {
            Debug.Log("Game Saved (Placeholder - Implement actual saving!)");
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

} // End of MainGameController class