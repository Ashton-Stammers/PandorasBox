using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterEffect : MonoBehaviour
{
    public float charactersPerSecond = 20f;
    public float delayBetweenLines = 0.5f;

    private TextMeshProUGUI tmpText;
    private string currentTextToType;
    private Coroutine typewritingCoroutine;
    private bool isTyping = false;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void StartTyping(string textToType)
    {
        StopTyping(false);

        currentTextToType = textToType;
        tmpText.text = currentTextToType;
        tmpText.maxVisibleCharacters = 0;

        tmpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpText.textInfo;

        int totalVisibleCharacters = textInfo.characterCount;

        if (totalVisibleCharacters > 0)
        {
            typewritingCoroutine = StartCoroutine(TypeTextRoutine(totalVisibleCharacters));
        }
        else
        {
            isTyping = false;
            currentTextToType = null;
        }
    }

    public void StopTyping(bool showFullText = true)
    {
        if (typewritingCoroutine != null)
        {
            StopCoroutine(typewritingCoroutine);
            typewritingCoroutine = null;
        }

        bool wasTyping = isTyping;
        isTyping = false;

        if (showFullText && wasTyping && tmpText != null && !string.IsNullOrEmpty(tmpText.text))
        {
            tmpText.ForceMeshUpdate();
            tmpText.maxVisibleCharacters = tmpText.textInfo.characterCount;
        }
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    private IEnumerator TypeTextRoutine(int totalVisibleChars)
    {
        isTyping = true;
        if (tmpText == null)
        {
            isTyping = false;
            yield break;
        }

        int visibleCount = 0;
        float delay = 0f;

        if (charactersPerSecond > 0)
        {
            delay = 1.0f / charactersPerSecond;
        }
        else
        {
            delay = 0;
        }

        while (visibleCount < totalVisibleChars)
        {
            if (tmpText == null)
            {
                isTyping = false;
                yield break;
            }

            visibleCount++;
            tmpText.maxVisibleCharacters = visibleCount;

            if (delay > 0)
                yield return new WaitForSeconds(delay);
            else
                yield return null;
        }

        if (tmpText != null)
        {
            tmpText.maxVisibleCharacters = totalVisibleChars;
        }

        isTyping = false;
        typewritingCoroutine = null;
    }
}