using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int depthLevel;
    public List<string> activeCurses = new List<string>();
    public List<string> activeBlessings = new List<string>();
    public Vector2 playerPosition;
    // Add other necessary fields from your game design
}

public static class SaveLoadManager
{
    // Existing Save/Load logic from your Assets/Scripts/SaveLoadManager.cs
    // Define constants for PlayerPrefs keys to avoid typos
    private const string GameStartedKey = "Pandora_GameStarted";

    // New save keys
    private const string PlayerChoiceMadeKey = "Pandora_ChoiceMade";
    private const string ChosenPathYKey = "Pandora_ChosenY";
    private const string CurrentDialogueIndexKey = "Pandora_DialogueIndex";
    private const string CurrentLinesKey = "Pandora_CurrentLines";

    // Flag to indicate if the game was just loaded (for skipping intros, etc.)
    // This needs to be static to persist across scene loads within a session.
    public static bool JustLoadedGame { get; set; } = false;

    /// <summary>
    /// Saves the game started state (true/false).
    /// </summary>
    /// <param name="started">True if the game has been started, false otherwise.</param>
    public static void SaveGameStartedState(bool started)
    {
        int valueToSave = started ? 1 : 0; // Convert bool to int (1 for true, 0 for false)
        PlayerPrefs.SetInt(GameStartedKey, valueToSave);
        PlayerPrefs.Save(); // Force save to disk immediately
        Debug.Log($"SaveLoadManager: Game Started State Saved = {started}");
    }

    /// <summary>
    /// Loads the game started state.
    /// </summary>
    /// <returns>True if a game was previously started and saved, false otherwise.</returns>
    public static bool LoadGameStartedState()
    {
        if (PlayerPrefs.HasKey(GameStartedKey))
        {
            int savedValue = PlayerPrefs.GetInt(GameStartedKey, 0); // Default to 0 (false)
            bool started = savedValue == 1;
            Debug.Log($"SaveLoadManager: Loaded Game Started State = {started}");
            return started;
        }
        else
        {
            Debug.Log("SaveLoadManager: No Game Started State found. Returning default (false).");
            return false; // Default value if the key doesn't exist
        }
    }

    /// <summary>
    /// Checks if any save data (specifically the GameStarted flag) exists.
    /// </summary>
    /// <returns>True if save data exists, false otherwise.</returns>
    public static bool HasSaveData()
    {
        bool hasData = PlayerPrefs.HasKey(GameStartedKey);
        return hasData;
    }

    /// <summary>
    /// Deletes the saved game started state. Useful for testing.
    /// </summary>
    public static void DeleteGameStartedState()
    {
        if (PlayerPrefs.HasKey(GameStartedKey))
        {
            PlayerPrefs.DeleteKey(GameStartedKey);
            PlayerPrefs.Save(); // Ensure deletion is written to disk
            Debug.Log($"SaveLoadManager: Deleted key '{GameStartedKey}'.");
        }
        else
        {
            Debug.Log($"SaveLoadManager: Key '{GameStartedKey}' not found, nothing to delete.");
        }
    }

    /// <summary>
    /// Deletes ALL PlayerPrefs data for this application. Use with extreme caution!
    /// </summary>
    public static void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Ensure deletion is written to disk
        Debug.LogWarning("SaveLoadManager: Deleted ALL PlayerPrefs data!");
    }

    /// <summary>
    /// Saves choice-related data.
    /// </summary>
    public static void SaveChoiceState(bool choiceMade, bool choseY, int dialogueIndex, string[] currentLines)
    {
        PlayerPrefs.SetInt(PlayerChoiceMadeKey, choiceMade ? 1 : 0);
        PlayerPrefs.SetInt(ChosenPathYKey, choseY ? 1 : 0);
        PlayerPrefs.SetInt(CurrentDialogueIndexKey, dialogueIndex);
        PlayerPrefs.SetString(CurrentLinesKey, string.Join("|", currentLines));
        PlayerPrefs.Save();
        Debug.Log("SaveLoadManager: Choice State Saved.");
    }

    /// <summary>
    /// Loads choice-related data.
    /// </summary>
    public static void LoadChoiceState(out bool choiceMade, out bool choseY,
                                       out int dialogueIndex, out string[] currentLines)
    {
        choiceMade = PlayerPrefs.GetInt(PlayerChoiceMadeKey, 0) == 1;
        choseY = PlayerPrefs.GetInt(ChosenPathYKey, 0) == 1;
        dialogueIndex = PlayerPrefs.GetInt(CurrentDialogueIndexKey, 0);
        currentLines = PlayerPrefs.GetString(CurrentLinesKey, "").Split('|');
        Debug.Log("SaveLoadManager: Choice State Loaded.");
    }

    /// <summary>
    /// Deletes all choice-related data.
    /// </summary>
    public static void DeleteChoiceState()
    {
        PlayerPrefs.DeleteKey(PlayerChoiceMadeKey);
        PlayerPrefs.DeleteKey(ChosenPathYKey);
        PlayerPrefs.DeleteKey(CurrentDialogueIndexKey);
        PlayerPrefs.DeleteKey(CurrentLinesKey);
        PlayerPrefs.Save();
        Debug.Log("SaveLoadManager: Choice State Deleted.");
    }

    // New ASCII Save System from DSoutput.txt
    public static void SaveGameState(PlayerData data)
    {
        PlayerPrefs.SetString("playerData", JsonUtility.ToJson(data));
        PlayerPrefs.SetInt("depthLevel", data.depthLevel); //
        PlayerPrefs.SetString("curses", string.Join(",", data.activeCurses)); //
        PlayerPrefs.SetString("blessings", string.Join(",", data.activeBlessings)); //
        PlayerPrefs.Save(); //
    }

    public static PlayerData LoadGameState()
    {
        return new PlayerData
        {
            depthLevel = PlayerPrefs.GetInt("depthLevel", 0), //
            activeCurses = new List<string>(PlayerPrefs.GetString("curses").Split(',')), //
            activeBlessings = new List<string>(PlayerPrefs.GetString("blessings").Split(',')) //
            // Load other fields that you added to PlayerData
        };
    }
}