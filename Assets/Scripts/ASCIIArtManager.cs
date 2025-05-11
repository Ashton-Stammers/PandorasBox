using UnityEngine;
using TMPro;
using System.Collections.Generic; // For using Dictionary

public class ASCIIArtManager : MonoBehaviour
{
    [Tooltip("Assign the TextMeshProUGUI element for displaying ASCII art.")]
    public TMP_Text asciiDisplay;

    // Using a Dictionary to map enemy type names (or IDs) to their art.
    // This makes it easier to add/change art for specific enemies.
    private Dictionary<string, string> enemyArtLibrary;

    // You can also keep a simple array for random selection if you prefer not to use names yet
    private string[] randomEnemyArt = new string[3];


    void Awake()
    {
        InitializeArtLibrary();
        if (asciiDisplay == null)
        {
            Debug.LogError("ASCIIArtManager: asciiDisplay (TMP_Text) not assigned in the Inspector!");
            // Consider disabling the component or GameObject if this is critical
            // this.enabled = false;
        }
    }

    void InitializeArtLibrary()
    {
        enemyArtLibrary = new Dictionary<string, string>();

        // --- Plague Rats ---
        // Option 1 (Compact)
        string plagueRatsArt1 =
              "  _(\")_  \n"
            + " /@( )\\ \n"
            + "/.' `--`'.\\\n"
            + "`~\"\"\"\"\"~`";
        enemyArtLibrary.Add("PLAGUE_RATS_1", plagueRatsArt1);

        // Option 2 (Swarm idea, more abstract)
        string plagueRatsArt2 =
              ".· '_\n"
            + ":`.·¸.'.·\n"
            + " `·¸.'.·'\n"
            + "   `··`";
        enemyArtLibrary.Add("PLAGUE_RATS_2", plagueRatsArt2);


        // --- Chaos Beast ---
        // Option 1 (Simple, menacing)
        string chaosBeastArt1 =
              "  ██████ \n"
            + " ████████ \n"
            + "███(OO)███\n"
            + "██████████\n"
            + " (\\__/)\n"
            + "  `--`";
        enemyArtLibrary.Add("CHAOS_BEAST_1", chaosBeastArt1);

        // Option 2 (More tentacled/amorphous)
        string chaosBeastArt2 =
              "   .-~~~-.\n"
            + "  /`( OVU )`\\\n"
            + " /(       )\\\n"
            + " \\ `~\"\"\"~` /\n"
            + "  `\\ VVV /`\n"
            + "    `---`";
        enemyArtLibrary.Add("CHAOS_BEAST_2", chaosBeastArt2);


        // --- Glitch Entity ---
        // Option 1 (Classic glitch block)
        string glitchEntityArt1 =
              "▓▓▓▓░░░░\n"
            + "░░▓▓▓▓░░\n"
            + "▓▓░░▓▓▓▓\n"
            + "▓▓▓▓░░▓▓\n"
            + "░░▓▓▓▓░░";
        enemyArtLibrary.Add("GLITCH_ENTITY_1", glitchEntityArt1);

        // Option 2 (More fragmented)
        string glitchEntityArt2 =
              "█ ░ ▓ ▒\n"
            + " ▒▓ █ ░\n"
            + "░ █ ▒ ▓\n"
            + "▓▒ ░ █ \n"
            + "█ ░▓ ▒ ";
        enemyArtLibrary.Add("GLITCH_ENTITY_2", glitchEntityArt2);

        // Populate the random array with one option for each type for now
        // This part is for your current BattleSystem that picks randomly by index
        randomEnemyArt[0] = plagueRatsArt1;   // For "PLAGUE RATS"
        randomEnemyArt[1] = chaosBeastArt1;   // For "CHAOS BEAST"
        randomEnemyArt[2] = glitchEntityArt1; // For "GLITCH ENTITY"
    }

    /// <summary>
    /// Displays ASCII art for a given enemy type string (key in the library).
    /// </summary>
    /// <param name="enemyKey">The key corresponding to the enemy art (e.g., "PLAGUE_RATS_1").</param>
    public void ShowEnemyArtByKey(string enemyKey)
    {
        if (asciiDisplay == null) return;

        if (enemyArtLibrary.TryGetValue(enemyKey, out string artToShow))
        {
            asciiDisplay.text = artToShow;
        }
        else
        {
            Debug.LogWarning($"ASCIIArtManager: Art key '{enemyKey}' not found in library. Displaying fallback.");
            asciiDisplay.text = "ENTITY\nUNKNOWN"; // Fallback art
        }
    }

    /// <summary>
    /// Displays ASCII art randomly based on a simple index.
    /// This matches your current BattleSystem's random enemy generation.
    /// </summary>
    /// <param name="index">The index for the randomEnemyArt array.</param>
    public void ShowEnemyArtByIndex(int index)
    {
        if (asciiDisplay == null) return;

        if (index >= 0 && index < randomEnemyArt.Length && randomEnemyArt[index] != null)
        {
            asciiDisplay.text = randomEnemyArt[index];
        }
        else
        {
            Debug.LogWarning($"ASCIIArtManager: Invalid art index {index} or art not set. Displaying fallback.");
            asciiDisplay.text = "SYSTEM\nERROR"; // Fallback art
        }
    }

    /// <summary>
    /// Clears the ASCII art display.
    /// </summary>
    public void ClearArt()
    {
        if (asciiDisplay != null)
        {
            asciiDisplay.text = "";
        }
    }
}