using UnityEngine;
using TMPro;

public class ASCIIArtManager : MonoBehaviour
{
    public TMP_Text asciiDisplay;
    private string[] enemyArt = {
        "  (•_•)  \n  /) ))  \n  ︶ ︶︶", // Rat swarm
        "  /\\_/\\  \n ( o.o ) \n  > ^ <", // Chaos beast
        "  ░▒▓███ \n▓██▓▒░░▒▓\n ██▓▒░    " // Glitch entity
    };

    public void ShowEnemyArt(int index)
    {
        if (index >= 0 && index < enemyArt.Length)
        {
            asciiDisplay.text = enemyArt[index];
        }
    }
}