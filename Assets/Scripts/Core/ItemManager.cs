using UnityEngine;
using System; // Required for System.Action
using System.Collections.Generic;

// ItemData class should be defined here or in its own file (e.g., ItemData.cs)
[System.Serializable] // [cite: 78]
public class ItemData
{
    public string id; // [cite: 78]
    public string ascii; // [cite: 78] // For display in ASCII UI if needed
    public string useText; // [cite: 79] // Text to display when item is used
    public System.Action<PlayerController> effect; // [cite: 79] // Action to perform when used
    public string description; // Good to have for UI tooltips or menu
}

// ScriptableObject for the database (from AI output [cite: 107])
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")] // This line is key
public class ItemDatabase : ScriptableObject
{
    public ItemData[] items;
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance; // Optional: Singleton

    public ItemDatabase itemDatabaseAsset; // Assign this in the Inspector [cite: 79]
    private Dictionary<string, ItemData> itemDatabase = new Dictionary<string, ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadItemDatabase();
    }

    void LoadItemDatabase()
    {
        if (itemDatabaseAsset == null)
        {
            Debug.LogError("ItemDatabase asset not assigned to ItemManager!");
            return;
        }

        foreach (ItemData item in itemDatabaseAsset.items) // [cite: 80]
        {
            if (!itemDatabase.ContainsKey(item.id))
            {
                // Assign predefined effects based on ID
                // This is one way to link item IDs to actual functions
                if (item.id == "health_potion") item.effect = UseHealthPotion;
                else if (item.id == "scroll_of_escape") item.effect = UseScrollOfEscape;
                // Add more items and their effects here

                itemDatabase.Add(item.id, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate item ID '{item.id}' found in ItemDatabase asset.");
            }
        }
        Debug.Log("Item Database Loaded with " + itemDatabase.Count + " items.");
    }

    public ItemData GetItem(string searchID) // [cite: 80]
    {
        if (itemDatabase.TryGetValue(searchID, out ItemData item)) // [cite: 80]
        {
            return item; // [cite: 80]
        }
        Debug.LogWarning($"Item with ID '{searchID}' not found in database.");
        return null; // [cite: 81]
    }

    // --- Item Effect Implementations ---
    // These are static as per the AI's design, but could also be instance methods
    // if they needed access to ItemManager instance variables.
    public static void UseHealthPotion(PlayerController player) // [cite: 82]
    {
        if (player == null) return;
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.Heal(10); // [cite: 82]
            Debug.Log("Player used Health Potion and healed 10 HP.");
            if (GameManager.Instance != null) GameManager.Instance.UpdateASCIIDisplay("Used Health Potion. +10 HP");
        }
        else
        {
            Debug.LogWarning("Player tried to use Health Potion, but no PlayerStats component found.");
        }
    }

    public static void UseScrollOfEscape(PlayerController player) // [cite: 83]
    {
        if (player == null) return; // Not strictly needed if not interacting with player directly for this item
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GenerateNewFloor(); // [cite: 83]
            Debug.Log("Player used Scroll of Escape. New floor generated.");
            if (GameManager.Instance != null) GameManager.Instance.UpdateASCIIDisplay("Used Scroll of Escape!");
        }
        else
        {
            Debug.LogError("GameManager instance not found for Scroll of Escape.");
        }
    }

    public void UseItem(string itemID, PlayerController player)
    {
        ItemData item = GetItem(itemID);
        if (item != null && item.effect != null)
        {
            Debug.Log($"Using item: {item.id} - {item.useText}");
            item.effect.Invoke(player);
            player.inventory.Remove(itemID); // Consume the item
            if (GameManager.Instance != null) GameManager.Instance.UpdateInventoryDisplay();
        }
        else
        {
            Debug.LogWarning($"Cannot use item '{itemID}'. Not found or no effect defined.");
        }
    }


}