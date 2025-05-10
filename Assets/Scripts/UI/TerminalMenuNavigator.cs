using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Only needed if actions were directly in this script

// Handles keyboard navigation (Up/Down/Enter) for a terminal-style menu
// Updates TextMeshPro elements to show selection and positions a blinking cursor.
public class TerminalMenuNavigator : MonoBehaviour
{


    public List<TextMeshProUGUI> menuOptions;

    public TextMeshProUGUI blinkingCursor;



    public string selectionPrefix = "> ";

    public float cursorXOffset = -20f;

    [Header("Action Handler")]

    public TitleScreenActions titleScreenActions;

    // Internal state
    private int selectedIndex = 0;
    private List<string> originalOptionTexts = new List<string>(); // Stores original text without prefix

    void Start()
    {
        if (menuOptions == null || menuOptions.Count == 0) // Use || on one line
        {
            Debug.LogError("TerminalMenuNavigator: No menu options assigned!");
            enabled = false; // Disable script if no options
            return;
        }
        if (blinkingCursor != null)
        {
            blinkingCursor.gameObject.SetActive(true); // Ensure cursor is active initially
        }

        CacheOriginalTexts();
        UpdateVisuals();
    }

    void Update()
    {
        HandleNavigationInput();
        HandleSelectionInput();
    }

    // Stores the initial text of each menu option before adding prefixes
    void CacheOriginalTexts()
    {
        originalOptionTexts.Clear();
        foreach (TextMeshProUGUI option in menuOptions)
        {
            if (option != null)
            {
                // Store the text as is, UpdateVisuals will handle prefix removal if needed on first run
                originalOptionTexts.Add(option.text);
            }
            else
            {
                originalOptionTexts.Add(""); // Add placeholder for safety
                Debug.LogWarning("TerminalMenuNavigator: A menu option is not assigned in the list.");
            }
        }
        // Correct cached text if it already starts with the prefix (e.g., after script reload)
        for (int i = 0; i < originalOptionTexts.Count; ++i)
        {
            if (menuOptions[i] != null && menuOptions[i].text.StartsWith(selectionPrefix))
            {
                originalOptionTexts[i] = menuOptions[i].text.Substring(selectionPrefix.Length);
            }
        }
    }

    // Handles Up/Down arrow key presses for changing selection
    void HandleNavigationInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0)
            {
                selectedIndex = menuOptions.Count - 1; // Wrap to bottom
            }
            UpdateVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= menuOptions.Count)
            {
                selectedIndex = 0; // Wrap to top
            }
            UpdateVisuals();
        }
    }

    // Handles Enter/Return key press for confirming selection
    void HandleSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) // Use || on one line
        {
            ExecuteSelectedOption();
        }
    }

    // Updates the text of menu options to show the selection prefix and positions the cursor
    void UpdateVisuals()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (menuOptions[i] != null && originalOptionTexts.Count > i)
            {
                // Use cached original text
                string baseText = originalOptionTexts[i];

                if (i == selectedIndex)
                {
                    // Add prefix only if it's not already there (handles initial state)
                    if (!menuOptions[i].text.StartsWith(selectionPrefix))
                    {
                        menuOptions[i].text = selectionPrefix + baseText;
                    }
                }
                else
                {
                    // Set text to original if it currently has prefix
                    if (menuOptions[i].text.StartsWith(selectionPrefix))
                    {
                        menuOptions[i].text = baseText;
                    }
                    // Ensure it's the base text otherwise too
                    else if (menuOptions[i].text != baseText)
                    {
                        menuOptions[i].text = baseText;
                    }
                }
            }
        }
        PositionCursor();
    }

    // Positions the blinking cursor GameObject next to the currently selected menu option
    void PositionCursor()
    {
        if (blinkingCursor == null) return; // No cursor assigned

        if (selectedIndex >= 0 && selectedIndex < menuOptions.Count && menuOptions[selectedIndex] != null)
        {
            blinkingCursor.gameObject.SetActive(true); // Ensure cursor is visible

            RectTransform selectedOptionRect = menuOptions[selectedIndex].rectTransform;
            RectTransform cursorRect = blinkingCursor.rectTransform;

            // --- Cursor Positioning Logic ---
            // Attempt to position cursor to the left of the text start.
            // This relies on the TMP object having generated its mesh/text info.
            TMP_TextInfo textInfo = menuOptions[selectedIndex].textInfo;
            Vector3 targetPos;

            if (textInfo != null && textInfo.characterCount > 0 && textInfo.lineCount > 0)
            {
                // Get the position of the first character's bottom-left corner in local space
                Vector3 firstCharBottomLeft = textInfo.characterInfo[textInfo.lineInfo[0].firstCharacterIndex].bottomLeft;

                // Convert local position to world position relative to the selected option's transform
                targetPos = selectedOptionRect.TransformPoint(new Vector3(firstCharBottomLeft.x + cursorXOffset, firstCharBottomLeft.y, 0));

                // Adjust Y based on line height or center - using selected option's Y for simplicity here
                targetPos.y = selectedOptionRect.position.y;
            }
            else
            {
                // Fallback: Position based on the RectTransform's left edge (approximated)
                Vector3 selectedPos = selectedOptionRect.position;
                float pivotOffsetX = selectedOptionRect.rect.width * selectedOptionRect.pivot.x;
                targetPos = selectedOptionRect.TransformPoint(new Vector3(selectedOptionRect.rect.xMin + cursorXOffset, 0, 0));
                targetPos.y = selectedPos.y; // Match Y position
            }

            cursorRect.position = targetPos;
            // Ensure cursor uses same Z as options (relevant for non-overlay canvas)
            cursorRect.position = new Vector3(cursorRect.position.x, cursorRect.position.y, selectedOptionRect.position.z);

        }
        else
        {
            blinkingCursor.gameObject.SetActive(false); // Hide cursor if selection is invalid
        }
    }


    // Calls the appropriate action method based on the selected index
    void ExecuteSelectedOption()
    {
        if (titleScreenActions == null)
        {
            Debug.LogError("TerminalMenuNavigator: TitleScreenActions reference not set!");
            return;
        }

        // Check index bounds before accessing actions
        if (selectedIndex >= 0 && selectedIndex < menuOptions.Count)
        {
            Debug.Log($"Executing action for index: {selectedIndex}"); // Debug log
            switch (selectedIndex)
            {
                case 0: titleScreenActions.ExecuteNewGame(); break;
                case 1: titleScreenActions.ExecuteLoadGame(); break;
                case 2: titleScreenActions.ExecuteSettings(); break;
                case 3: titleScreenActions.ExecuteExit(); break;
                default: Debug.LogWarning($"TerminalMenuNavigator: No action defined for index {selectedIndex}"); break;
            }
        }
        else
        {
            Debug.LogWarning($"TerminalMenuNavigator: Selected index {selectedIndex} is out of bounds.");
        }
    }
}