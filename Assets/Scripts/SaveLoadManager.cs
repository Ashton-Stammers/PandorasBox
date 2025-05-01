using UnityEngine;

public static class SaveLoadManager
{
    private const string GameStartedKey = "Pandora_GameStarted";
    public static bool JustLoadedGame { get; set; } = false;

    public static void SaveGameStartedState(bool started)
    {
        int valueToSave = started ? 1 : 0;
        PlayerPrefs.SetInt(GameStartedKey, valueToSave);
        PlayerPrefs.Save();
        Debug.Log($"SaveLoadManager: Game Started State Saved = {started}");
    }

    public static bool LoadGameStartedState()
    {
        if (PlayerPrefs.HasKey(GameStartedKey))
        {
            int savedValue = PlayerPrefs.GetInt(GameStartedKey, 0);
            bool started = savedValue == 1;
            Debug.Log($"SaveLoadManager: Loaded Game Started State = {started}");
            return started;
        }
        else
        {
            Debug.Log("SaveLoadManager: No Game Started State found. Returning default (false).");
            return false;
        }
    }

    public static bool HasSaveData()
    {
        bool hasData = PlayerPrefs.HasKey(GameStartedKey);
        return hasData;
    }

    public static void DeleteGameStartedState()
    {
        if (PlayerPrefs.HasKey(GameStartedKey))
        {
            PlayerPrefs.DeleteKey(GameStartedKey);
            PlayerPrefs.Save();
            Debug.Log($"SaveLoadManager: Deleted key '{GameStartedKey}'.");
        }
        else
        {
            Debug.Log($"SaveLoadManager: Key '{GameStartedKey}' not found, nothing to delete.");
        }
    }

    public static void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.LogWarning("SaveLoadManager: Deleted ALL PlayerPrefs data!");
    }
}