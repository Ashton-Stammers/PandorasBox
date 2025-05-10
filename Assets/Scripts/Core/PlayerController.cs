using UnityEngine;
using System.Collections.Generic; // [cite: 60]

public class PlayerController : MonoBehaviour
{
    public Vector2Int gridPosition; // [cite: 60]
    public float moveDelay = 0.2f; // [cite: 60]
    public List<string> inventory = new List<string>(); // [cite: 61]

    private float lastMoveTime; // [cite: 61]
    private DungeonGenerator dungeon; // [cite: 61]
    // public PlayerStats playerStats; // You'll need a PlayerStats component/script

    void Start() // [cite: 62]
    {
        dungeon = FindObjectOfType<DungeonGenerator>(); // [cite: 62]
        if (dungeon != null)
        {
            gridPosition = dungeon.GetStartingPosition(); // [cite: 62]
            transform.position = new Vector3(gridPosition.x, transform.position.y, gridPosition.y); // Update visual position
        }
        else
        {
            Debug.LogError("DungeonGenerator not found in scene for PlayerController.");
        }
        // playerStats = GetComponent<PlayerStats>(); // If you have a PlayerStats script
        lastMoveTime = Time.time;
    }

    void Update() // [cite: 63]
    {
        if (Time.time - lastMoveTime < moveDelay) return; // [cite: 63]

        Vector2Int moveDir = GetMoveInput(); // [cite: 64]
        if (moveDir != Vector2Int.zero) // [cite: 64]
        {
            Vector2Int newPos = gridPosition + moveDir; // [cite: 64]
            if (dungeon != null && dungeon.IsWalkable(newPos)) // [cite: 65]
            {
                gridPosition = newPos; // [cite: 65]
                transform.position = new Vector3(gridPosition.x, transform.position.y, gridPosition.y); // Update visual position
                lastMoveTime = Time.time; // [cite: 66]
                dungeon.HandleTileEvents(newPos); // [cite: 66]
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) // [cite: 66]
        {
            TryOpenBox(); // [cite: 66]
        } // [cite: 67]
    }

    Vector2Int GetMoveInput() // [cite: 67]
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) return Vector2Int.up; // [cite: 67]
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) return Vector2Int.down; // [cite: 68]
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) return Vector2Int.left; // [cite: 68]
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) return Vector2Int.right; // [cite: 68]
        return Vector2Int.zero; // [cite: 68]
    }

    void TryOpenBox() // [cite: 69]
    {
        if (dungeon != null && dungeon.IsBoxTile(gridPosition)) // [cite: 69]
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OpenPandorasBox(); // [cite: 69]
            else
                Debug.LogError("GameManager instance not found to open Pandora's Box.");
        } // [cite: 70]
    }

    public void AddItem(string itemID) // [cite: 70]
    {
        inventory.Add(itemID); // [cite: 70]
        Debug.Log($"Item Added: {itemID}");
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateInventoryDisplay(); // [cite: 71]
    }

    // Example for PlayerStats integration if needed
    /*
    public class PlayerStats : MonoBehaviour // Could be a separate script
    {
        public int maxHealth = 100;
        public int currentHealth;
        public int strength = 10;
        // public int hope = 10; // From EnemyAI

        void Start()
        {
            currentHealth = maxHealth;
        }

        public void ModifyHope(int amount)
        {
            // hope += amount;
            // Debug.Log($"Player hope changed by {amount}. Current hope: {hope}");
        }
        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
        }
        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth < 0) currentHealth = 0;
            Debug.Log($"Player took {amount} damage. Current health: {currentHealth}");
        }
        public bool IsAlive()
        {
            return currentHealth > 0;
        }
    }
    */
}