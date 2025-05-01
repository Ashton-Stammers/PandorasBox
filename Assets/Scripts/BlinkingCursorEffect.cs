using UnityEngine;
using TMPro; // Required if toggling text content

public class BlinkingCursorEffect : MonoBehaviour
{
    public float blinkRate = 0.5f;
    private TextMeshProUGUI cursorText;
    private bool isCursorVisible = true;

    void Start()
    {
        cursorText = GetComponent<TextMeshProUGUI>();
        if (cursorText == null)
        {
            Debug.LogError("BlinkingCursorEffect requires a TextMeshProUGUI component on the same GameObject.");
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(ToggleCursorVisibility), 0f, blinkRate);
    }

    void ToggleCursorVisibility()
    {
        isCursorVisible = !isCursorVisible;

        if (cursorText != null)
        {
            cursorText.enabled = isCursorVisible;
        }
    }

    void OnDisable()
    {
        CancelInvoke(nameof(ToggleCursorVisibility));
        if (cursorText != null) cursorText.enabled = true;
    }

    void OnEnable()
    {
        if (cursorText != null)
        {
            isCursorVisible = true;
            cursorText.enabled = true;
            CancelInvoke(nameof(ToggleCursorVisibility));
            InvokeRepeating(nameof(ToggleCursorVisibility), blinkRate, blinkRate);
        }
    }
}