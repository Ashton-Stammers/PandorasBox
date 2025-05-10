using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    private List<string> currentOptions = new List<string>();

    private string[][] npcResponses = {
        new string[] { "The walls bleed... you see it too?", "Run before it takes you!", "Bring me 3 shards for safe passage." },
        new string[] { "The Box hungers... feed it.", "I've seen your fate - it ends in fire.", "Seek the blue flame." }
    };

    public void StartInteraction(int npcID)
    {
        currentOptions = new List<string>(npcResponses[npcID]);
        ShowOptions();
    }

    void ShowOptions()
    {
        string displayText = "";
        for (int i = 0; i < currentOptions.Count; i++)
        {
            displayText += $"[{i + 1}] {currentOptions[i]}\n";
        }
        dialogueText.text = displayText;
    }

    public string GetResponse(int choice)
    {
        return currentOptions[choice];
    }
}