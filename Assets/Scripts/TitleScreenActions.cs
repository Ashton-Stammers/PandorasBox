using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenActions : MonoBehaviour
{
    public string mainGameSceneName = "MainGame";

    public void ExecuteNewGame()
    {
        Debug.Log($"Attempting to load scene: {mainGameSceneName}");
        SceneManager.LoadScene(mainGameSceneName, LoadSceneMode.Single);
    }

    public void ExecuteLoadGame()
    {
        Debug.Log("Load Game Selected - Functionality Not Implemented");
    }

    public void ExecuteSettings()
    {
        Debug.Log("Settings Selected - Functionality Not Implemented");
    }

    public void ExecuteExit()
    {
        Debug.Log("Exit command received.");
        Application.Quit();

#if UNITY_EDITOR
        Debug.LogWarning("Application.Quit() does not work in the Editor. Stopping Play Mode.");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}