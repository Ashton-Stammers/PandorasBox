using UnityEngine;
using TMPro;

public class CurseBlessingManager : MonoBehaviour
{
    public TMP_Text effectLog;
    private string[] activeEffects = new string[5];

    public void ApplyEffect(bool isCurse)
    {
        string effect = isCurse ? GetRandomCurse() : GetRandomBlessing();
        effectLog.text += $"\n{effect}";
        // Add gameplay impact logic here
    }

    string GetRandomCurse()
    {
        return new string[] {
            "CURSE: WORLD FLOOD - Movement restricted!",
            "CURSE: WITHERING TOUCH - Max HP reduced!",
            "CURSE: REFLECTIVE SCALES - Projectiles dangerous!"
        }[Random.Range(0, 3)];
    }

    string GetRandomBlessing()
    {
        return new string[] {
            "BLESSING: DIVINE VIGOR - HP increased!",
            "BLESSING: EAGLE EYE - Critical chance up!",
            "BLESSING: SANCTIFIED GROUND - Safe zones appear!"
        }[Random.Range(0, 3)];
    }
}