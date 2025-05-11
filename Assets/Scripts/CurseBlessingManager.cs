using UnityEngine;
using TMPro;

public class CurseBlessingManager : MonoBehaviour
{
    public TMP_Text effectLog;
    // private string[] activeEffects = new string[5]; // Not currently used, could be for stacking

    public void ApplyEffect(bool isCurse)
    {
        string effect = isCurse ? GetRandomCurse() : GetRandomBlessing();
        if (effectLog != null)
        {
            effectLog.text = effect; // Set text for clarity
        }
        else
        {
            Debug.LogWarning("CurseBlessingManager: effectLog not assigned. Effect: " + effect);
        }
        // TODO: Add actual gameplay impact logic here based on the effect
        // e.g., if (effect.Contains("Max HP reduced")) playerStats.MaxHP *= 0.8f;
        Debug.Log($"Applied Effect: {effect}");
    }

    string GetRandomCurse()
    {
        return new string[] {
            "CURSE: WORLD FLOOD - Movement feels sluggish!", // Descriptive, actual effect needs coding
            "CURSE: WITHERING TOUCH - Your vitality feels drained!", // Max HP reduced
            "CURSE: REFLECTIVE SCALES - Physical attacks sometimes backfire!" // Projectiles dangerous
        }[Random.Range(0, 3)];
    }

    string GetRandomBlessing()
    {
        return new string[] {
            "BLESSING: DIVINE VIGOR - You feel a surge of resilience!", // HP increased
            "BLESSING: EAGLE EYE - Your focus sharpens!", // Critical chance up
            "BLESSING: SANCTIFIED GROUND - A brief moment of calm washes over you." // Safe zones appear (conceptual)
        }[Random.Range(0, 3)];
    }
}