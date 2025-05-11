using UnityEngine;
using UnityEngine.UI; // Required for RawImage

[RequireComponent(typeof(RawImage))]
public class UIScrollingTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.0f;
    public float scrollSpeedY = 0.05f; // Adjust for desired speed and direction

    private RawImage rawImage;
    private Rect uvRect;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("UIScrollingTexture requires a RawImage component.", this);
            enabled = false;
            return;
        }
        // Initialize uvRect with the current RawImage UV rect
        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        if (rawImage != null)
        {
            uvRect.x += scrollSpeedX * Time.deltaTime;
            uvRect.y += scrollSpeedY * Time.deltaTime;

            // Keep UVs within 0-1 range to ensure continuous tiling
            if (uvRect.x > 1.0f || uvRect.x < -1.0f) uvRect.x %= 1.0f;
            if (uvRect.y > 1.0f || uvRect.y < -1.0f) uvRect.y %= 1.0f;

            rawImage.uvRect = uvRect;
        }
    }
}