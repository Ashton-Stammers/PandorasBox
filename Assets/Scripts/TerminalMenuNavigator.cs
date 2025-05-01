using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TerminalMenuNavigator : MonoBehaviour
{
    public List<TextMeshProUGUI> menuOptions;
    public TextMeshProUGUI blinkingCursor;
    public string selectionPrefix = "> ";
    public float cursorXOffset = -20f;

    [Header("Action Handler")]
    public TitleScreenActions titleScreenActions;

    private int selectedIndex = 0;
    private List<string> originalOptionTexts = new List<string>();

    void Start()
    {
        if (menuOptions == null || menuOptions.Count == 0)
        {
            Debug.LogError("TerminalMenuNavigator: No menu options assigned!");
            enabled = false;
            return;
        }
        if (blinkingCursor != null)
        {
            blinkingCursor.gameObject.SetActive(true);
        }

        CacheOriginalTexts();
        UpdateVisuals();
    }

    void Update()
    {
        HandleNavigationInput();
        HandleSelectionInput();
    }

    void CacheOriginalTexts()
    {
        originalOptionTexts.Clear();
        foreach (TextMeshProUGUI option in menuOptions)
        {
            if (option != null)
            {
                originalOptionTexts.Add(option.text);
            }
            else
            {
                originalOptionTexts.Add("");
                Debug.LogWarning("TerminalMenuNavigator: A menu option is not assigned in the list.");
            }
        }
        for (int i = 0; i < originalOptionTexts.Count; ++i)
        {
            if (menuOptions[i] != null && menuOptions[i].text.StartsWith(selectionPrefix))
            {
                originalOptionTexts[i] = menuOptions[i].text.Substring(selectionPrefix.Length);
            }
        }
    }

    void HandleNavigationInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0)
            {
                selectedIndex = menuOptions.Count - 1;
            }
            UpdateVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= menuOptions.Count)
            {
                selectedIndex = 0;
            }
            UpdateVisuals();
        }
    }

    void HandleSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteSelectedOption();
        }
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (menuOptions[i] != null && originalOptionTexts.Count > i)
            {
                string baseText = originalOptionTexts[i];

                if (i == selectedIndex)
                {
                    if (!menuOptions[i].text.StartsWith(selectionPrefix))
                    {
                        menuOptions[i].text = selectionPrefix + baseText;
                    }
                }
                else
                {
                    if (menuOptions[i].text.StartsWith(selectionPrefix))
                    {
                        menuOptions[i].text = baseText;
                    }
                    else if (menuOptions[i].text != baseText)
                    {
                        menuOptions[i].text = baseText;
                    }
                }
            }
        }
        PositionCursor();
    }

    void PositionCursor()
    {
        if (blinkingCursor == null) return;

        if (selectedIndex >= 0 && selectedIndex < menuOptions.Count && menuOptions[selectedIndex] != null)
        {
            blinkingCursor.gameObject.SetActive(true);

            RectTransform selectedOptionRect = menuOptions[selectedIndex].rectTransform;
            RectTransform cursorRect = blinkingCursor.rectTransform;

            TMP_TextInfo textInfo = menuOptions[selectedIndex].textInfo;
            Vector3 targetPos;

            if (textInfo != null && textInfo.characterCount > 0 && textInfo.lineCount > 0)
            {
                Vector3 firstCharBottomLeft = textInfo.characterInfo[textInfo.lineInfo[0].firstCharacterIndex].bottomLeft;
                targetPos = selectedOptionRect.TransformPoint(new Vector3(firstCharBottomLeft.x + cursorXOffset, firstCharBottomLeft.y, 0));
                targetPos.y = selectedOptionRect.position.y;
            }
            else
            {
                Vector3 selectedPos = selectedOptionRect.position;
                float pivotOffsetX = selectedOptionRect.rect.width * selectedOptionRect.pivot.x;
                targetPos = selectedOptionRect.TransformPoint(new Vector3(selectedOptionRect.rect.xMin + cursorXOffset, 0, 0));
                targetPos.y = selectedPos.y;
            }

            cursorRect.position = targetPos;
            cursorRect.position = new Vector3(cursorRect.position.x, cursorRect.position.y, selectedOptionRect.position.z);
        }
        else
        {
            blinkingCursor.gameObject.SetActive(false);
        }
    }

    void ExecuteSelectedOption()
    {
        if (titleScreenActions == null)
        {
            Debug.LogError("TerminalMenuNavigator: TitleScreenActions reference not set!");
            return;
        }

        if (selectedIndex >= 0 && selectedIndex < menuOptions.Count)
        {
            Debug.Log($"Executing action for index: {selectedIndex}");
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