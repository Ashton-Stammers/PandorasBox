using UnityEngine;
using TMPro; // [cite: 94]
using System.Collections; // Required for IEnumerator

public class ASCIIEffectsManager : MonoBehaviour
{
    public static ASCIIEffectsManager Instance; // Optional: Singleton

    public TextMeshProUGUI mainDisplay; // Assign your main ASCII display TextMeshProUGUI here [cite: 95]
    public float glitchInterval = 0.1f; // [cite: 95]

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (mainDisplay == null)
        {
            Debug.LogError("MainDisplay (TextMeshProUGUI) not assigned to ASCIIEffectsManager!");
        }
    }

    public void ShowAnimatedEffect(string[] frames, float frameDuration) // Renamed duration parameter [cite: 96]
    {
        if (mainDisplay == null) return;
        StartCoroutine(AnimateEffect(frames, frameDuration)); // [cite: 96]
    }

    IEnumerator AnimateEffect(string[] frames, float frameDuration) // [cite: 97]
    {
        if (mainDisplay == null) yield break;
        foreach (string frame in frames) // [cite: 97]
        {
            mainDisplay.text = frame; // [cite: 97]
            yield return new WaitForSeconds(frameDuration); // [cite: 98]
        }
    }

    public void GlitchText(string text) // [cite: 98]
    {
        if (mainDisplay == null) return;
        StartCoroutine(GlitchEffect(text)); // [cite: 98]
    }

    IEnumerator GlitchEffect(string original) // [cite: 99]
    {
        if (mainDisplay == null) yield break;

        char[] glitchChars = { '#', '@', '*', '&', '%' }; // [cite: 99]
        string glitchedText = original; // [cite: 100] // Changed variable name to avoid conflict

        mainDisplay.text = original; // Show original briefly
        yield return new WaitForSeconds(glitchInterval * 2);


        for (int k = 0; k < 5; k++) // AI used i, using k to avoid outer loop conflict if any [cite: 100]
        {
            glitchedText = original; // Reset to original before applying glitches each iteration
            for (int charIndex = 0; charIndex < original.Length; charIndex++)
            {
                if (Random.value < 0.15f) // 15% chance to glitch a character
                {
                    int pos = charIndex; // [cite: 100]
                    char randomChar = glitchChars[Random.Range(0, glitchChars.Length)];
                    // Make sure to handle string modification carefully if performance is key
                    glitchedText = glitchedText.Remove(pos, 1).Insert(pos, randomChar.ToString()); // [cite: 101]
                }
            }
            mainDisplay.text = glitchedText; // [cite: 102]
            yield return new WaitForSeconds(glitchInterval); // [cite: 102]
        }

        mainDisplay.text = original; // [cite: 102]
    }
}