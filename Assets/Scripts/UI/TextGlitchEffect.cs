using UnityEngine;
using TMPro;
using System.Collections;

public class TextGlitchEffect : MonoBehaviour
{
    public float glitchIntensity = 0.1f;
    public float glitchRate = 0.1f;
    public float glitchDuration = 0.05f;

    private TextMeshProUGUI tmpText;
    private TMP_TextInfo textInfo;
    private Coroutine glitchCoroutine;
    private Vector3[][] originalVertices;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("TextGlitchEffect requires a TextMeshProUGUI component on the same GameObject.", this);
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (glitchCoroutine == null && tmpText != null)
        {
            StartCoroutine(ForceUpdateAndResetOnEnable());
        }
    }

    void OnDisable()
    {
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            glitchCoroutine = null;
        }

        if (tmpText != null && textInfo != null && originalVertices != null)
        {
            ResetVertices(false);
        }

        originalVertices = null;
    }

    IEnumerator ForceUpdateAndResetOnEnable()
    {
        yield return null;
        if (tmpText != null && enabled)
        {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;
            StoreOriginalVertices();
            ResetVertices(true);
            if (glitchCoroutine == null && enabled)
            {
                glitchCoroutine = StartCoroutine(GlitchRoutine());
            }
        }
    }

    IEnumerator GlitchRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(glitchRate * 0.8f, glitchRate * 1.2f);
            yield return new WaitForSeconds(waitTime);

            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;

            if (textInfo == null || textInfo.characterCount == 0 || textInfo.meshInfo == null || textInfo.meshInfo.Length == 0)
            {
                yield return new WaitForSeconds(glitchRate);
                continue;
            }

            StoreOriginalVertices();

            if (originalVertices == null) continue;

            int characterCount = textInfo.characterCount;
            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                if (materialIndex >= textInfo.meshInfo.Length ||
                    textInfo.meshInfo[materialIndex].vertices == null ||
                    vertexIndex + 3 >= textInfo.meshInfo[materialIndex].vertices.Length)
                {
                    continue;
                }

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3 offset = new Vector3(Random.Range(-glitchIntensity, glitchIntensity), Random.Range(-glitchIntensity, glitchIntensity), 0f);

                bool hasOriginals = originalVertices.Length > materialIndex &&
                                    originalVertices[materialIndex] != null &&
                                    vertexIndex + 3 < originalVertices[materialIndex].Length;

                Vector3 basePos0 = hasOriginals ? originalVertices[materialIndex][vertexIndex + 0] : destinationVertices[vertexIndex + 0];
                Vector3 basePos1 = hasOriginals ? originalVertices[materialIndex][vertexIndex + 1] : destinationVertices[vertexIndex + 1];
                Vector3 basePos2 = hasOriginals ? originalVertices[materialIndex][vertexIndex + 2] : destinationVertices[vertexIndex + 2];
                Vector3 basePos3 = hasOriginals ? originalVertices[materialIndex][vertexIndex + 3] : destinationVertices[vertexIndex + 3];

                destinationVertices[vertexIndex + 0] = basePos0 + offset;
                destinationVertices[vertexIndex + 1] = basePos1 + offset;
                destinationVertices[vertexIndex + 2] = basePos2 + offset;
                destinationVertices[vertexIndex + 3] = basePos3 + offset;
            }

            UpdateMeshVertices();

            yield return new WaitForSeconds(glitchDuration);

            ResetVertices(true);

        }
    }

    void StoreOriginalVertices()
    {
        if (textInfo == null || textInfo.meshInfo == null) return;

        if (originalVertices == null || originalVertices.Length != textInfo.meshInfo.Length)
        {
            originalVertices = new Vector3[textInfo.meshInfo.Length][];
        }

        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            if (meshInfo.vertices == null || meshInfo.vertices.Length == 0)
            {
                originalVertices[i] = null;
                continue;
            }

            if (originalVertices[i] == null || originalVertices[i].Length != meshInfo.vertices.Length)
            {
                originalVertices[i] = new Vector3[meshInfo.vertices.Length];
            }
            System.Array.Copy(meshInfo.vertices, originalVertices[i], meshInfo.vertices.Length);
        }
    }

    void ResetVertices(bool forceMeshUpdate = true)
    {
        if (originalVertices == null || tmpText == null || textInfo == null || textInfo.meshInfo == null)
        {
            return;
        }

        if (forceMeshUpdate)
        {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;
            if (textInfo == null || textInfo.meshInfo == null) return;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (originalVertices.Length <= i ||
                originalVertices[i] == null ||
                textInfo.meshInfo.Length <= i ||
                textInfo.meshInfo[i].vertices == null)
            {
                continue;
            }

            if (textInfo.meshInfo[i].vertices.Length != originalVertices[i].Length)
            {
                continue;
            }

            System.Array.Copy(originalVertices[i], textInfo.meshInfo[i].vertices, originalVertices[i].Length);
        }

        UpdateMeshVertices();
    }

    void UpdateMeshVertices()
    {
        if (tmpText == null || textInfo == null || textInfo.meshInfo == null) return;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (i >= textInfo.meshInfo.Length ||
                textInfo.meshInfo[i].mesh == null ||
                textInfo.meshInfo[i].vertexCount == 0 ||
                textInfo.meshInfo[i].vertices == null)
            {
                continue;
            }

            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}