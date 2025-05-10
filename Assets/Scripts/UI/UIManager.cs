using UnityEngine;
using TMPro; // [cite: 41]
using System.Collections; // Required for IEnumerator

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Optional: Singleton

    [Header("UI Panels")]
    public GameObject menuPanel; // [cite: 41]
    // public GameObject inventoryPanel; // Example for future use
    // public GameObject statsPanel; // Example for future use

    [Header("Text Elements")]
    public TextMeshProUGUI helpText; // [cite: 41]
    public TextMeshProUGUI depthCounter; // [cite: 41]
    public TextMeshProUGUI activeCursesText; // For displaying curses in menu
    public TextMeshProUGUI activeBlessingsText; // For displaying blessings in menu


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (menuPanel != null) menuPanel.SetActive(false); // Start with menu closed
        if (helpText != null) helpText.text = ""; // Start with help text empty
    }

    void Update() // [cite: 42]
    {
        if (Input.GetKeyDown(KeyCode.E))  // [cite: 42]
        {
            ToggleMenu(); // [cite: 42]
        } // [cite: 43]

        if (Input.GetKeyDown(KeyCode.F1))  // [cite: 43]
        {
            ToggleHelp(); // [cite: 43]
        } // [cite: 44]
    }

    void ToggleMenu() // [cite: 44]
    {
        if (menuPanel == null) return;
        bool isActive = !menuPanel.activeSelf;
        menuPanel.SetActive(isActive); // [cite: 44]
        Time.timeScale = isActive ? 0 : 1; // [cite: 45] Pause game when menu is open
        if (isActive) UpdateMenuStats(); // [cite: 45]
    }

    void UpdateMenuStats() // [cite: 45]
    {
        // Display current curses/blessings from GameManager
        if (GameManager.Instance != null)
        {
            if (activeCursesText != null)
                activeCursesText.text = "Curses: " + string.Join(", ", GameManager.Instance.activeCurses); // You'll need public access or a getter in GameManager

            if (activeBlessingsText != null)
                activeBlessingsText.text = "Blessings: " + string.Join(", ", GameManager.Instance.activeBlessings); // Same as above
        }
    }

    void ToggleHelp() // [cite: 45]
    {
        if (helpText == null) return;

        if (string.IsNullOrEmpty(helpText.text))
        {
            helpText.text = @"
            CONTROLS:
            WASD - Move
            E - Menu
            Space - Confirm / Interact
            F1 - Toggle Help
            F5 - Quick Save
            Q - Query Object (Example) 
            "; // [cite: 46]
            StartCoroutine(HideHelpAfterDelay(5f)); // [cite: 47]
        }
        else
        {
            helpText.text = ""; // Hide help if already visible
        }
    }

    IEnumerator HideHelpAfterDelay(float delay) // Renamed from HideHelp [cite: 47]
    {
        yield return new WaitForSeconds(delay); // [cite: 47]
        if (helpText != null) helpText.text = ""; // [cite: 48]
    }

    public void UpdateDepthDisplay(int depth)
    {
        if (depthCounter != null)
        {
            depthCounter.text = $"Depth: {depth}";
        }
    }
}