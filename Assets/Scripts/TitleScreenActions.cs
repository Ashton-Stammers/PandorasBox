using UnityEngine;
using UnityEngine.SceneManagement; // Needed for SceneManager

// Contains the functions executed when title screen menu options are selected.
public class TitleScreenActions : MonoBehaviour
{

    public string mainGameSceneName = "MainGame";

    // Called when the "New Game" option is selected
    public void ExecuteNewGame()
    {
        Debug.Log($"Attempting to load scene: {mainGameSceneName}");
        // Load the specified game scene, replacing the current title screen scene
        SceneManager.LoadScene(mainGameSceneName, LoadSceneMode.Single);
    }

    // Called when the "Load Game" option is selected
    public void ExecuteLoadGame()
    {
        // Placeholder: Implement game loading logic here
        Debug.Log("Load Game Selected - Functionality Not Implemented");
        // Example: SceneManager.LoadScene("LoadGameMenu");
    }

    // Called when the "Settings" option is selected
    public void ExecuteSettings()
    {
        // Placeholder: Implement settings menu logic here
        Debug.Log("Settings Selected - Functionality Not Implemented");
        // Example: SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Additive);
    }

    // Called when the "Exit" option is selected
    public void ExecuteExit()
    {
        Debug.Log("Exit command received.");
        // Quit the application (works in standalone builds)
        Application.Quit();

        // Provide feedback in the Unity Editor, as Application.Quit() is ignored
#if UNITY_EDITOR
        Debug.LogWarning("Application.Quit() does not work in the Editor. Stopping Play Mode.");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}