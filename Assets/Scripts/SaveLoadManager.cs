using UnityEngine;

// Static class to manage simple game saving/loading using PlayerPrefs.
public static class SaveLoadManager
{
    // Define constants for PlayerPrefs keys to avoid typos
    private const string GameStartedKey = "Pandora_GameStarted";
    // Flag to indicate if the game was just loaded (for skipping intros, etc.)
    // This needs to be static to persist across scene loads within a session.
    public static bool JustLoadedGame { get; set; } = false;

    /// <summary>
    /// Saves the game started state (true/false).
    /// </summary>
    /// <param name="started">True if the game has been started, false otherwise.</param>
    public static void SaveGameStartedState(bool started)
    {
        int valueToSave = started ? 1 : 0; // Convert bool to int (1 for true, 0 for false) [19, 78, 62]
        PlayerPrefs.SetInt(GameStartedKey, valueToSave); // [19, 74, 10, 76, 77, 78, 62]
        PlayerPrefs.Save(); // Force save to disk immediately [25, 19, 27, 74, 10, 76, 77, 78, 62, 83]
        Debug.Log($"SaveLoadManager: Game Started State Saved = {started}");
    }

    /// <summary>
    /// Loads the game started state.
    /// </summary>
    /// <returns>True if a game was previously started and saved, false otherwise.</returns>
    public static bool LoadGameStartedState()
    {
        if (PlayerPrefs.HasKey(GameStartedKey)) // [19, 27, 74, 10, 76, 77, 78, 62, 64]
        {
            int savedValue = PlayerPrefs.GetInt(GameStartedKey, 0); // Default to 0 (false) if key exists but value is odd [19, 27, 74, 10, 76, 77, 78, 62, 64]
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
        bool hasData = PlayerPrefs.HasKey(GameStartedKey); // [19, 27, 74, 10, 76, 77, 78, 62, 64]
        // Debug.Log($"SaveLoadManager: HasSaveData Check = {hasData}"); // Can be noisy
        return hasData;
    }

    /// <summary>
    /// Deletes the saved game started state. Useful for testing.
    /// </summary>
    public static void DeleteGameStartedState()
    {
        if (PlayerPrefs.HasKey(GameStartedKey))
        {
            PlayerPrefs.DeleteKey(GameStartedKey); // [85, 89]
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
        PlayerPrefs.DeleteAll(); // [85, 89, 92]
        PlayerPrefs.Save(); // Ensure deletion is written to disk
        Debug.LogWarning("SaveLoadManager: Deleted ALL PlayerPrefs data!");
    }

    // --- Add methods for other saved data here ---
    // Example: Saving player level
    // private const string PlayerLevelKey = "Pandora_PlayerLevel";
    // public static void SavePlayerLevel(int level) { PlayerPrefs.SetInt(PlayerLevelKey, level); PlayerPrefs.Save(); }
    // public static int LoadPlayerLevel() { return PlayerPrefs.GetInt(PlayerLevelKey, 1); } // Default level 1
}