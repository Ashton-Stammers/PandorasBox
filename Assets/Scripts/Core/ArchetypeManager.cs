using UnityEngine;

[System.Serializable] // [cite: 91]
public class Archetype
{
    public string name; // [cite: 91]
    public string ascii; // [cite: 91] // For display if needed
    public int baseHealth; // [cite: 91]
    public int baseStrength; // [cite: 91]
    public string startingItem; // [cite: 92] // ID of the starting item
}

public class ArchetypeManager : MonoBehaviour
{
    public static ArchetypeManager Instance; // Optional: Singleton

    public Archetype[] archetypes; // Assign these in the Inspector [cite: 92]

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (archetypes == null || archetypes.Length == 0)
        {
            Debug.LogWarning("No archetypes defined in ArchetypeManager. Creating default examples.");
            archetypes = new Archetype[] {
                new Archetype { name = "Warrior", ascii = "@", baseHealth = 120, baseStrength = 15, startingItem = "sword" },
                new Archetype { name = "Mage", ascii = "&", baseHealth = 80, baseStrength = 8, startingItem = "staff_of_embers" }
            };
        }
    }

    public void ApplyArchetype(int index, PlayerController player) // Modified to take PlayerController [cite: 93]
    {
        if (player == null)
        {
            Debug.LogError("PlayerController is null. Cannot apply archetype.");
            return;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>(); // [cite: 93]
        if (stats == null)
        {
            Debug.LogError("PlayerController does not have a PlayerStats component. Cannot apply archetype stats.");
            return;
        }

        if (index < 0 || index >= archetypes.Length)
        {
            Debug.LogError($"Archetype index {index} is out of bounds.");
            return;
        }

        Archetype selected = archetypes[index]; // [cite: 93]

        stats.maxHealth = selected.baseHealth; // [cite: 94]
        stats.currentHealth = selected.baseHealth; // Also set current health
        stats.strength = selected.baseStrength; // [cite: 94]

        Debug.Log($"Applied archetype '{selected.name}' to player.");

        if (!string.IsNullOrEmpty(selected.startingItem))
        {
            player.AddItem(selected.startingItem); // [cite: 94]
            Debug.Log($"Gave starting item: {selected.startingItem}");
        }
    }

    // Example: Call this at the start of a new game
    public void SelectAndApplyArchetypeForPlayer(PlayerController player, int archetypeIndex = 0)
    {
        if (player != null && archetypes.Length > 0)
        {
            ApplyArchetype(archetypeIndex, player);
        }
        else if (player == null)
        {
            Debug.LogError("Cannot apply archetype: Player is null.");
        }
        else
        {
            Debug.LogError("Cannot apply archetype: No archetypes defined.");
        }
    }
}