using UnityEngine;
using TMPro; // [cite: 7]
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // [cite: 7]

    [Header("ASCII Display")] // [cite: 8]
    public TextMeshProUGUI asciiDisplay; // [cite: 8]
    public TextMeshProUGUI hudText; // [cite: 8]

    [Header("Config")] // [cite: 9]
    public List<string> cursePool = new List<string>(); // [cite: 9]
    public List<string> blessingPool = new List<string>(); // [cite: 9]

    public PlayerData currentData; // [cite: 9]
    public List<string> activeCurses = new List<string>(); // [cite: 10]
    public List<string> activeBlessings = new List<string>(); // [cite: 10]
    public int currentDepth = 0; // [cite: 10]

    void Awake() // [cite: 11]
    {
        Instance = this; // [cite: 11]
        LoadOrInitialize(); // [cite: 11]
    }

    void LoadOrInitialize() // [cite: 12]
    {
        if (SaveLoadManager.HasSaveData())
        { // Check your existing HasSaveData logic
            currentData = SaveLoadManager.LoadGameState(); // [cite: 12]
            currentDepth = currentData.depthLevel; // [cite: 13]
            UpdateASCIIDisplay($"LOADED DEPTH {currentDepth}"); // [cite: 13]
        }
        else
        {
            currentData = new PlayerData(); // [cite: 13]
            GenerateNewFloor(); // [cite: 14]
        }
    }

    public void GenerateNewFloor() // [cite: 14]
    {
        // --- Your procedural generation logic will go here ---
        // This should ideally call your DungeonGenerator
        // For now, it just updates the display and depth
        UpdateASCIIDisplay(GetFloorASCII()); // [cite: 14]
        currentDepth++; // [cite: 15]
        if (hudText != null) hudText.text = $"Depth: {currentDepth}"; // [cite: 15]
    }

    public void OpenPandorasBox() // [cite: 15]
    {
        StartCoroutine(BoxOpeningSequence()); // [cite: 15]
    }

    IEnumerator BoxOpeningSequence() // [cite: 16]
    {
        bool isCurse = Random.Range(0f, 1f) > 0.4f; // [cite: 16]
        string effect = GetRandomEffect(isCurse); // [cite: 17]

        if (asciiDisplay != null) asciiDisplay.text = GetBoxArt(); // [cite: 17]
        yield return new WaitForSeconds(1f); // [cite: 17]

        if (isCurse)
        { // [cite: 18]
            activeCurses.Add(effect); // [cite: 18]
            ApplyCurse(effect); // [cite: 18]
        }
        else
        { // [cite: 19]
            activeBlessings.Add(effect); // [cite: 19]
            ApplyBlessing(effect); // [cite: 19]
        }

        UpdateASCIIDisplay(GetFloorASCII()); // [cite: 20]
        SaveGame(); // [cite: 20]
    }

    string GetRandomEffect(bool isCurse)
    {
        if (isCurse)
        {
            if (cursePool.Count > 0)
                return cursePool[Random.Range(0, cursePool.Count)];
            return "default_curse"; // Fallback
        }
        else
        {
            if (blessingPool.Count > 0)
                return blessingPool[Random.Range(0, blessingPool.Count)];
            return "default_blessing"; // Fallback
        }
    }

    void ApplyCurse(string curseId) // [cite: 21]
    {
        switch (curseId)
        {
            case "world_flood": // [cite: 21]
                StartCoroutine(ApplyWorldFlood()); // [cite: 21]
                break; // [cite: 22]
            // Add other curses cases here [cite: 22]
            default:
                Debug.LogWarning($"Curse '{curseId}' not implemented.");
                break;
        }
    }

    void ApplyBlessing(string blessingId)
    {
        // --- Implement blessing effects here ---
        Debug.Log($"Applying blessing: {blessingId}");
        // Example:
        // switch(blessingId) {
        //     case "player_speed_up":
        //         // Modify player speed
        //         break;
        // }
    }

    IEnumerator ApplyWorldFlood() // [cite: 22]
    {
        string[] floodFrames = { // [cite: 22]
            "~~~ Slowly rising water appears... ~~~", // [cite: 22]
            "!!! Movement becomes sluggish !!!", // [cite: 22]
            "Enemies adapt to the flooded environment" // [cite: 22]
        };
        foreach (string frame in floodFrames)
        { // [cite: 23]
            UpdateASCIIDisplay(frame); // [cite: 23]
            yield return new WaitForSeconds(1.5f); // [cite: 24]
        }
    }

    public void SaveGame() // [cite: 24]
    {
        if (currentData == null) currentData = new PlayerData();
        currentData.depthLevel = currentDepth; // [cite: 24]
        currentData.activeCurses = activeCurses; // [cite: 25]
        currentData.activeBlessings = activeBlessings; // [cite: 25]
        // currentData.playerPosition = GetPlayerPosition(); // You'll need a way to get this
        SaveLoadManager.SaveGameState(currentData); // [cite: 25]
    }

    public void UpdateASCIIDisplay(string textToDisplay)
    {
        if (asciiDisplay != null)
        {
            asciiDisplay.text = textToDisplay;
        }
    }

    public string GetFloorASCII() // [cite: 25]
    {
        // This should be generated by your DungeonGenerator based on the actual map
        // For now, a placeholder:
        return @"
            ##...##  
            #....@.  
            ##..D##  
            ########
        "; // [cite: 25]
    }

    string GetBoxArt() // [cite: 26]
    {
        return @"
            ░░░▒▒▓▓▓▓▓▒▒░░░        
            ░░▓▓▓▓▓▓▓▓▓▓▓▓░        
              ▓▓▓▓▓▓▓▓▓▓▓▓▓        
            PANDORA'S BOX          
              ▓▓▓▓▓▓▓▓▓▓▓▓▓        
            ░░▓▓▓▓▓▓▓▓▓▓▓▓░        
            ░░░▒▒▓▓▓▓▓▒▒░░░
        "; // [cite: 27]
    }

    // Placeholder for a function you'll need in PlayerController.cs
    public void UpdateInventoryDisplay()
    {
        Debug.Log("Inventory display update requested.");
        // Actual logic to update HUD with inventory items
    }
}