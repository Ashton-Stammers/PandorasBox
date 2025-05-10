// BlinkingCursorEffect.cs
using UnityEngine;
using TMPro; // Required if toggling text content

public class BlinkingCursorEffect : MonoBehaviour
{
    // Option 1: Toggle GameObject enabled state
    // public float blinkRate = 0.5f;
    // private bool isCursorVisible = true;

    // Option 2: Toggle TextMeshPro component enabled state
    public float blinkRate = 0.5f;
    private TextMeshProUGUI cursorText;
    private bool isCursorVisible = true;

    void Start()
    {
        // Option 2 Setup: Get the component
        cursorText = GetComponent<TextMeshProUGUI>();
        if (cursorText == null)
        {
            Debug.LogError("BlinkingCursorEffect requires a TextMeshProUGUI component on the same GameObject.");
            enabled = false; // Disable script if component is missing
            return;
        }

        // Start the blinking using InvokeRepeating
        InvokeRepeating(nameof(ToggleCursorVisibility), 0f, blinkRate);
    }

    void ToggleCursorVisibility()
    {
        isCursorVisible = !isCursorVisible;

        // Option 1: Toggle GameObject
        // gameObject.SetActive(isCursorVisible);

        // Option 2: Toggle TMP Component
        if (cursorText != null)
        {
            cursorText.enabled = isCursorVisible;
        }
    }

    // Optional: Ensure cursor is visible when the script is disabled/enabled
    void OnDisable()
    {
        CancelInvoke(nameof(ToggleCursorVisibility));
        // Ensure cursor is visible when script stops
        if (cursorText != null) cursorText.enabled = true;
    }

    void OnEnable()
    {
        // Restart blinking if re-enabled, checking component exists
        if (cursorText != null)
        {
            isCursorVisible = true;
            cursorText.enabled = true;
            // Prevent duplicate invokes if already running
            CancelInvoke(nameof(ToggleCursorVisibility));
            InvokeRepeating(nameof(ToggleCursorVisibility), blinkRate, blinkRate);
        }
    }
}