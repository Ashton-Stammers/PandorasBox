using UnityEngine;
using UnityEngine.SceneManagement; // [cite: 103]

public class MenuNavigation : MonoBehaviour
{
    // These methods will be called by UI Buttons' OnClick() events.
    // Ensure you have scenes named "MainMenu" and "GameScene" in your Build Settings.

    public void StartNewGame() // Renamed from NewGame to be more descriptive for button
    {
        Debug.Log("Starting New Game...");
        SaveLoadManager.DeleteAllSaveData(); // [cite: 105] // Clear previous save
        // SaveLoadManager.JustLoadedGame = false; // Ensure this flag is reset if you use it
        SceneManager.LoadScene("GameScene"); // [cite: 106] // Or your main game scene name
    }

    public void LoadSavedGame() // Renamed from LoadGame
    {
        Debug.Log("Loading Game...");
        if (SaveLoadManager.HasSaveData()) // Check if save data actually exists
        {
            // The LoadGameState from the AI output seems to be for the ASCII system
            // PlayerData data = SaveLoadManager.LoadGameState(); // [cite: 104]
            // If you are using the more complex SaveLoadManager from your assets:
            // SaveLoadManager.JustLoadedGame = true; // Set flag to skip intro
            SceneManager.LoadScene("GameScene"); // [cite: 105]
        }
        else
        {
            Debug.LogWarning("No save data found to load!");
            // Optionally, disable the Load Game button if no save data exists,
            // or show a message to the player.
        }
    }

    public void SaveAndQuitToMainMenu() // Renamed from SaveAndQuit [cite: 103]
    {
        Debug.Log("Saving and Quitting to Main Menu...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame(); // [cite: 103]
        }
        SceneManager.LoadScene("MainMenu"); // [cite: 104] // Or your main menu scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Application...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#endif
    }
}