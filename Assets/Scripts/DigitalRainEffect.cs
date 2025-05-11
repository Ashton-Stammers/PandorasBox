using UnityEngine;
using TMPro; // Make sure this is TextMeshProUGUI if needed for specific UI properties elsewhere
using System.Collections.Generic;

public class DigitalRainEffect : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign a TMP_FontAsset. A monospace font is recommended.")]
    public TMP_FontAsset rainFont;
    [Tooltip("Optional: Assign a Material for specific TextMeshPro effects on the rain text.")]
    public Material rainMaterial; // Optional

    [Header("Rain Properties")]
    [Tooltip("How many columns of rain to generate across the screen.")]
    public int numberOfColumns = 60;
    [Tooltip("How fast the rain streams fall (units per second).")]
    public float fallSpeed = 50.0f;
    [Tooltip("How often characters in a stream change (seconds).")]
    public float characterChangeRate = 0.1f;
    [Tooltip("Minimum number of characters in a rain stream.")]
    public int minStreamLength = 8;
    [Tooltip("Maximum number of characters in a rain stream.")]
    public int maxStreamLength = 25;
    [Tooltip("Color of the main body of the rain stream.")]
    public Color streamColor = new Color(0, 1, 0, 0.7f); // Default: Green, somewhat transparent
    [Tooltip("Color of the leading (bottom-most) character in the rain stream.")]
    public Color leadingCharColor = new Color(0.8f, 1, 0.8f, 1f); // Default: Brighter green/white

    [Header("Character Set")]
    [Tooltip("The pool of characters to use for the rain effect.")]
    public string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ﾊﾐﾋｰｳｼﾅﾓﾆｻﾜﾂｵﾘｱﾎﾃﾏｹﾒｴｶｷﾑﾕﾗｾﾈｽﾀﾇﾍｦｲｸｺｿﾁﾄﾉﾌﾔﾖﾙﾚﾛﾝ@#$%^*()_+-=[]{}|;':\",./<>?";

    private List<RainStream> streams;
    private float screenWidthWorld;
    private float characterWidthApprox;
    private Canvas parentCanvas;

    void Start()
    {
        Debug.Log("DigitalRainEffect: Start() called.");

        if (rainFont == null)
        {
            Debug.LogError("DigitalRainEffect: Rain Font (TMP_FontAsset) is not assigned in the Inspector! Disabling script.");
            enabled = false;
            return;
        }
        Debug.Log($"DigitalRainEffect: Rain Font '{rainFont.name}' is assigned.");

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("DigitalRainEffect: No parent Canvas found! The GameObject with DigitalRainEffect must be a child of a Canvas. Disabling script.");
            enabled = false;
            return;
        }
        Debug.Log($"DigitalRainEffect: Parent Canvas '{parentCanvas.name}' found.");

        if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError($"DigitalRainEffect: Parent Canvas '{parentCanvas.name}' is not in ScreenSpace-Overlay mode (current mode: {parentCanvas.renderMode}). This script is designed for ScreenSpace-Overlay. Disabling script.");
            enabled = false;
            return;
        }
        Debug.Log($"DigitalRainEffect: Parent Canvas '{parentCanvas.name}' is in ScreenSpace-Overlay mode.");

        Debug.Log("DigitalRainEffect: Attempting to calculate character width.");
        GameObject testCharGO = null;
        try
        {
            testCharGO = new GameObject("Temp_DigitalRain_TestChar_For_Width_Calc");

            TextMeshProUGUI testChar = testCharGO.AddComponent<TextMeshProUGUI>(); // MODIFIED LINE

            if (testChar == null)
            {
                Debug.LogError("DigitalRainEffect: Failed to add TextMeshProUGUI component to temporary GameObject for width calculation. TextMeshPro might not be set up correctly or another issue occurred. Disabling script.");
                if (testCharGO != null) Destroy(testCharGO);
                enabled = false;
                return;
            }

            testChar.font = rainFont;

            float calculatedFontSize = CalculateFontSize();
            if (calculatedFontSize <= 0)
            {
                Debug.LogError($"DigitalRainEffect: CalculateFontSize returned an invalid size ({calculatedFontSize}). Cannot proceed. Disabling script.");
                if (testCharGO != null) Destroy(testCharGO);
                enabled = false;
                return;
            }
            testChar.fontSize = calculatedFontSize;

            testChar.text = "X";
            testChar.ForceMeshUpdate(true);

            characterWidthApprox = testChar.preferredWidth;
            Debug.Log($"DigitalRainEffect: Calculated preferredWidth for 'X' at size {calculatedFontSize}: {characterWidthApprox}");

            if (characterWidthApprox <= 0)
            {
                Debug.LogWarning($"DigitalRainEffect: PreferredWidth for character 'X' was zero or negative ({characterWidthApprox}). This might indicate issues with the font asset or TextMeshPro setup. Using heuristic fallback for characterWidthApprox.");
                characterWidthApprox = calculatedFontSize * 0.6f;
                if (characterWidthApprox <= 0)
                {
                    Debug.LogError("DigitalRainEffect: Fallback characterWidthApprox is also invalid. Cannot reliably space columns. Disabling script.");
                    if (testCharGO != null) Destroy(testCharGO);
                    enabled = false;
                    return;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DigitalRainEffect: Exception during character width calculation phase: {e.Message}\n{e.StackTrace}. Disabling script.");
            if (testCharGO != null) Destroy(testCharGO);
            enabled = false;
            return;
        }
        finally
        {
            if (testCharGO != null)
            {
                Destroy(testCharGO);
            }
        }
        Debug.Log($"DigitalRainEffect: Final characterWidthApprox to be used for column spacing: {characterWidthApprox}");

        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            Debug.LogError("DigitalRainEffect: parentCanvas RectTransform became null unexpectedly before stream setup. Disabling script.");
            enabled = false;
            return;
        }
        screenWidthWorld = canvasRect.rect.width;
        Debug.Log($"DigitalRainEffect: ScreenWidthWorld (Canvas Width): {screenWidthWorld}");

        streams = new List<RainStream>();
        if (numberOfColumns <= 0)
        {
            Debug.LogWarning("DigitalRainEffect: NumberOfColumns is zero or negative. No rain streams will be created.");
        }
        else if (characterWidthApprox <= 0)
        {
            Debug.LogError("DigitalRainEffect: Character width is invalid, cannot create streams. Disabling script.");
            enabled = false;
            return;
        }
        else
        {
            Debug.Log($"DigitalRainEffect: Creating {numberOfColumns} rain streams.");
        }

        for (int i = 0; i < numberOfColumns; i++)
        {
            CreateStream(i);
        }
        Debug.Log("DigitalRainEffect: Start() method completed successfully.");
    }

    float CalculateFontSize()
    {
        if (parentCanvas == null)
        {
            Debug.LogError("CalculateFontSize: parentCanvas is null! Cannot calculate font size. Returning default 12f.");
            return 12f;
        }

        RectTransform rt = parentCanvas.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("CalculateFontSize: parentCanvas does not have a RectTransform! Cannot calculate font size. Returning default 12f.");
            return 12f;
        }

        float denominator = (maxStreamLength > 0 ? maxStreamLength : 20) + 5;
        float calculatedSize = rt.rect.height / denominator;
        return Mathf.Max(8f, calculatedSize);
    }

    void CreateStream(int columnIndex)
    {
        if (parentCanvas == null || characterWidthApprox <= 0)
        {
            Debug.LogError($"CreateStream: Pre-requisites not met (parentCanvas or characterWidthApprox invalid). Cannot create stream {columnIndex}.");
            return;
        }

        GameObject streamObject = new GameObject($"RainStream_{columnIndex}");
        streamObject.transform.SetParent(transform, false);

        TextMeshProUGUI textComponent = streamObject.AddComponent<TextMeshProUGUI>(); // MODIFIED LINE
        textComponent.font = rainFont;
        if (rainMaterial != null) textComponent.fontSharedMaterial = rainMaterial;

        textComponent.fontSize = CalculateFontSize();
        textComponent.alignment = TextAlignmentOptions.Top;
        textComponent.enableWordWrapping = false;
        textComponent.overflowMode = TextOverflowModes.Overflow;
        textComponent.raycastTarget = false;

        RectTransform rt = textComponent.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);

        float columnSpacing = screenWidthWorld / numberOfColumns;
        float xPos = columnSpacing * columnIndex;

        float initialY = Random.Range(-20f, 20f);
        rt.anchoredPosition = new Vector2(xPos, initialY);

        RainStream newStream = new RainStream(textComponent, this); // Pass TextMeshProUGUI
        streams.Add(newStream);
        newStream.ResetStream(Random.Range(minStreamLength, maxStreamLength + 1), initialY);
    }

    void Update()
    {
        if (streams == null) return;

        foreach (RainStream stream in streams)
        {
            if (stream != null)
            {
                stream.Update(fallSpeed * Time.deltaTime, characterChangeRate);
            }
        }
    }

    private class RainStream
    {
        public TextMeshProUGUI TextMesh; // MODIFIED TYPE
        private DigitalRainEffect effectManager;
        private List<char> currentChars;
        private int streamLength;
        private float currentYPos;
        private float timeSinceLastCharChange;
        private float initialYOffset;

        public RainStream(TextMeshProUGUI tmpro, DigitalRainEffect manager) // MODIFIED CONSTRUCTOR
        {
            TextMesh = tmpro;
            effectManager = manager;
            currentChars = new List<char>();

            if (TextMesh == null) Debug.LogError("RainStream created with null TextMeshProUGUI component!");
            if (effectManager == null) Debug.LogError("RainStream created with null effectManager (DigitalRainEffect)!");
        }

        public void ResetStream(int length, float startYFromTop)
        {
            if (TextMesh == null || effectManager == null)
            {
                Debug.LogError("RainStream.ResetStream: TextMesh or effectManager is null. Cannot reset.");
                return;
            }

            streamLength = Mathf.Max(1, length);
            currentYPos = startYFromTop;
            initialYOffset = startYFromTop;

            RectTransform rt = TextMesh.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, currentYPos);
            }
            else
            {
                Debug.LogError("RainStream.ResetStream: TextMesh RectTransform is null!");
                return;
            }

            currentChars.Clear();
            if (string.IsNullOrEmpty(effectManager.characters))
            {
                Debug.LogWarning("DigitalRainEffect: Characters string is null or empty in ResetStream. Stream will use '?'.");
                for (int i = 0; i < streamLength; i++) currentChars.Add('?');
            }
            else
            {
                for (int i = 0; i < streamLength; i++)
                {
                    currentChars.Add(effectManager.characters[Random.Range(0, effectManager.characters.Length)]);
                }
            }
            timeSinceLastCharChange = 0f;
            UpdateTextContent();
        }

        public void Update(float fallAmount, float charChangeRate)
        {
            if (TextMesh == null || !TextMesh.gameObject.activeInHierarchy)
            {
                return;
            }
            if (effectManager == null)
            {
                Debug.LogError("RainStream.Update: effectManager is null! Cannot proceed.");
                return;
            }
            if (effectManager.parentCanvas == null)
            {
                Debug.LogError("DigitalRainEffect: ParentCanvas became null during update! Disabling effect.");
                if (effectManager.enabled) effectManager.enabled = false;
                return;
            }

            RectTransform textRectTransform = TextMesh.rectTransform;
            if (textRectTransform == null)
            {
                Debug.LogError("RainStream.Update: TextMesh.rectTransform is null! Skipping update for this stream.");
                return;
            }

            currentYPos -= fallAmount;
            textRectTransform.anchoredPosition = new Vector2(textRectTransform.anchoredPosition.x, currentYPos);

            timeSinceLastCharChange += Time.deltaTime;
            if (timeSinceLastCharChange >= charChangeRate)
            {
                if (string.IsNullOrEmpty(effectManager.characters))
                {
                    if (currentChars.Count > 0) currentChars.RemoveAt(currentChars.Count - 1);
                    currentChars.Insert(0, '?');
                }
                else
                {
                    if (currentChars.Count > 0) currentChars.RemoveAt(currentChars.Count - 1);
                    currentChars.Insert(0, effectManager.characters[Random.Range(0, effectManager.characters.Length)]);

                    for (int i = 1; i < currentChars.Count; i++)
                    {
                        if (Random.value < 0.1f)
                        {
                            currentChars[i] = effectManager.characters[Random.Range(0, effectManager.characters.Length)];
                        }
                    }
                }
                UpdateTextContent();
                timeSinceLastCharChange = 0f;
            }

            float textHeight = TextMesh.preferredHeight;
            RectTransform canvasRectTransform = effectManager.parentCanvas.GetComponent<RectTransform>();

            if (canvasRectTransform == null)
            {
                Debug.LogError("DigitalRainEffect: ParentCanvas RectTransform became null before reset check!");
                if (effectManager.enabled) effectManager.enabled = false;
                return;
            }
            if (currentYPos - textHeight < -canvasRectTransform.rect.height - 50)
            {
                ResetStream(Random.Range(effectManager.minStreamLength, effectManager.maxStreamLength + 1), initialYOffset);
            }
        }

        private void UpdateTextContent()
        {
            if (TextMesh == null || effectManager == null || currentChars == null) return;

            System.Text.StringBuilder sb = new System.Text.StringBuilder(streamLength * 20);
            for (int i = 0; i < currentChars.Count; i++)
            {
                Color charColor = (i == currentChars.Count - 1) ? effectManager.leadingCharColor : effectManager.streamColor;
                sb.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(charColor)}>{currentChars[i]}</color>\n");
            }
            TextMesh.text = sb.ToString();
        }
    }
}