using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections; // Required for Coroutines

// Displays text character by character using TMP_Text.maxVisibleCharacters
[RequireComponent(typeof(TextMeshProUGUI))] // Ensure component exists
public class TypewriterEffect : MonoBehaviour
{
    public float charactersPerSecond = 20f;
    public float delayBetweenLines = 0.5f; // Not used internally; for external control logic

    private TextMeshProUGUI tmpText;
    private string currentTextToType;
    private Coroutine typewritingCoroutine;
    private bool isTyping = false;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        // Thanks to [RequireComponent], tmpText is guaranteed to be assigned here.
    }

    /// <summary>
    /// Starts displaying the given text with the typewriter effect.
    /// Stops any currently running typewriter effect.
    /// </summary>
    /// <param name="textToType">The full string to display.</param>
    public void StartTyping(string textToType)
    {
        StopTyping(false); // Stop previous typing without revealing text

        currentTextToType = textToType;
        tmpText.text = currentTextToType; // Set the full text immediately for layout [8, 20, 57, 65]
        tmpText.maxVisibleCharacters = 0; // Hide all characters initially [8, 54, 11, 52, 56]

        // Force mesh update is essential *before* reading textInfo to get accurate character count
        tmpText.ForceMeshUpdate(); // Ensure textInfo is populated for accurate count [8, 56, 58]
        TMP_TextInfo textInfo = tmpText.textInfo;

        // Use textInfo.characterCount as it aligns with how maxVisibleCharacters handles source text indices, including tags.
        int totalVisibleCharacters = textInfo.characterCount;

        // Only start if there are characters to type
        if (totalVisibleCharacters > 0)
        {
            typewritingCoroutine = StartCoroutine(TypeTextRoutine(totalVisibleCharacters));
        }
        else
        {
            isTyping = false; // No characters, not typing
            currentTextToType = null; // Clear stored text if empty
        }
    }

    /// <summary>
    /// Stops the current typewriter effect immediately.
    /// Optionally reveals the full text.
    /// </summary>
    /// <param name="showFullText">If true, reveals the entire text instantly.</param>
    public void StopTyping(bool showFullText = true)
    {
        if (typewritingCoroutine != null)
        {
            StopCoroutine(typewritingCoroutine);
            typewritingCoroutine = null;
        }

        // Check isTyping state *before* potentially modifying text display
        bool wasTyping = isTyping;
        isTyping = false; // Set typing state to false regardless

        // Reveal text only if requested, it was typing previously, and component/text are valid
        if (showFullText && wasTyping && tmpText != null && !string.IsNullOrEmpty(tmpText.text))
        {
            // No need to get currentTextToType, just reveal all of the text component's current characters.
            // Ensure mesh is updated to get the correct count for the current text.
            tmpText.ForceMeshUpdate();
            // Reveal all characters based on the current text's info
            tmpText.maxVisibleCharacters = tmpText.textInfo.characterCount; // Reveal all characters [8, 54, 11, 52, 56]
        }
    }

    /// <summary>
    /// Checks if the typewriter is currently revealing text.
    /// </summary>
    /// <returns>True if typing, false otherwise.</returns>
    public bool IsTyping()
    {
        return isTyping;
    }

    // Coroutine to reveal characters one by one
    private IEnumerator TypeTextRoutine(int totalVisibleChars)
    {
        isTyping = true;
        // tmpText component null check (paranoid, but safe if object destroyed mid-coroutine)
        if (tmpText == null)
        {
            isTyping = false;
            yield break;
        }

        int visibleCount = 0;
        float delay = 0f;

        // Prevent division by zero and calculate delay
        if (charactersPerSecond > 0)
        {
            delay = 1.0f / charactersPerSecond;
        }
        else
        {
            // If speed is 0 or less, reveal instantly (delay = 0)
            delay = 0;
        }


        while (visibleCount < totalVisibleChars)
        {
            // Exit if component becomes null (e.g., object destroyed)
            if (tmpText == null)
            {
                isTyping = false;
                yield break;
            }

            visibleCount++;
            tmpText.maxVisibleCharacters = visibleCount; // Incrementally reveal characters [8, 20, 54, 11, 55, 52, 56]

            // ---- Optional: Punctuation Delay ----
            // Uncomment and adjust logic if needed.
            // Note: Accessing currentTextToType could be slightly out of sync if text is changed externally mid-type.
            // Using textInfo might be safer but more complex if needed.
            /*
            if (visibleCount > 0 && visibleCount <= currentTextToType.Length) // Basic bounds check
            {
                char lastChar = currentTextToType[visibleCount - 1];
                // Use || for logical OR
                if (lastChar == '.' || lastChar == ',' || lastChar == '?' || lastChar == '!')
                {
                    if (delay > 0) yield return new WaitForSeconds(delay * 1.5f); // Example: 1.5x delay
                }
            }
            */
            // ---- End Optional Delay ----


            // Wait for the calculated delay per character
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            else
                yield return null; // Yield for one frame if revealing instantly (delay <= 0)
        }

        // Ensure the full text is definitely visible at the end (in case of rounding errors or interruptions)
        if (tmpText != null)
        {
            tmpText.maxVisibleCharacters = totalVisibleChars;
        }


        // Typing finished
        isTyping = false;
        typewritingCoroutine = null;

        // Note: delayBetweenLines is not handled here; the calling script should wait if needed.
    }
}