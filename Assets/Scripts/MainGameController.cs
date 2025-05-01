using UnityEngine;
using System.Collections;
using TMPro;

public class MainGameController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the GameObject with the TypewriterEffect script for dialogue.")]
    public TypewriterEffect dialogueTypewriter;
    [Tooltip("Optional TextMeshProUGUI component to display save/load status.")]
    public TextMeshProUGUI statusText;

    [Header("Dialogue Content")]
    private string[] introLines = {
        "SYSTEM BOOTING...",
        "ACCESSING ARCHIVE :: PANDORA.BOX",
        "WARNING: CORE INTEGRITY COMPROMISED. CONTAINMENT FIELD FLUCTUATING.",
        "INITIATING INTERFACE...",
        "Welcome, User."
    };

    private Coroutine introCoroutine;

    void Start()
    {
        if (dialogueTypewriter == null)
        {
            Debug.LogError("DialogueTypewriter reference not set in MainGameController! Assign it in the Inspector.");
            enabled = false;
            return;
        }

        if (statusText != null)
        {
            statusText.text = "";
        }

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
        Debug.Log("Starting Intro Dialogue...");
        yield return new WaitForSeconds(0.5f);

        foreach (string line in introLines)
        {
            if (dialogueTypewriter == null) yield break;

            dialogueTypewriter.StartTyping(line);

            while (dialogueTypewriter.IsTyping())
            {
                yield return null;
            }

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
            StartCoroutine(ClearStatusText(3.0f));
        }
    }

    void TriggerSave()
    {
        Debug.Log("Save requested (F5 pressed).");

        if (statusText != null)
        {
            statusText.text = "Saving Game...";
            StartCoroutine(ClearStatusText(2.0f));
        }
        else
        {
            Debug.Log("Game Saved (Placeholder - Implement actual saving!)");
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