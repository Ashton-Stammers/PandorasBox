using UnityEngine;
using System.Collections;
using System.Collections.Generic; // [cite: 28] Needed for List

public class Enemy // This can be in its own file (Enemy.cs) or here. For simplicity, it's here.
{
    public string enemyId; // [cite: 39]
    public int health; // [cite: 40]
    public int damage; // Added for basic combat logic
    public Vector2Int gridPosition; // Added based on EnemyAI.cs

    public Enemy(string id, int hp, int dmg, Vector2Int pos)
    {
        enemyId = id;
        health = hp;
        damage = dmg;
        gridPosition = pos;
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
        Debug.Log($"{enemyId} took {amount} damage. Health: {health}");
    }

    public virtual void PerformAction() // [cite: 40]
    {
        // Basic AI logic based on type, or could call methods from an EnemyAI component
        Debug.Log($"{enemyId} performs a generic action.");
    }
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance; // Optional: make it a singleton

    public enum CombatState { PlayerTurn, EnemyTurn, Resolving, CombatOver } // [cite: 28]

    private CombatState currentState; // [cite: 28]
    private List<Enemy> enemies = new List<Enemy>(); // [cite: 29]
    // You'll need a reference to the player as well
    // public PlayerController player; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartCombat(List<Enemy> combatEnemies) // [cite: 29]
    {
        enemies = combatEnemies; // [cite: 29]
        // if (player == null) player = FindObjectOfType<PlayerController>(); // Find player if not set
        Debug.Log("Combat Started!");
        StartCoroutine(CombatLoop()); // [cite: 30]
    }

    IEnumerator CombatLoop() // [cite: 30]
    {
        while (!CheckCombatEnd())
        {
            // Player turn
            currentState = CombatState.PlayerTurn; // [cite: 30]
            Debug.Log("Player's Turn.");
            // Here you would enable player input for combat actions
            // For this example, we'll just wait until an action is simulated
            yield return new WaitUntil(() => currentState == CombatState.Resolving); // [cite: 31]
            if (CheckCombatEnd()) break; // Check again after player action

            // Enemies turn
            currentState = CombatState.EnemyTurn; // [cite: 31]
            Debug.Log("Enemies' Turn.");
            foreach (Enemy enemy in enemies)
            { // [cite: 32]
                if (enemy.IsAlive())
                { // [cite: 32]
                    enemy.PerformAction(); // [cite: 32]
                    // Example: if enemy attacks player: player.TakeDamage(enemy.damage);
                    yield return new WaitForSeconds(0.5f); // [cite: 33]
                }
            }
            currentState = CombatState.Resolving; // [cite: 33]
            if (CheckCombatEnd()) break; // Check again after enemy actions
        }
        EndCombat(); // [cite: 34]
    }

    public void PlayerAction(string action) // [cite: 34]
    {
        if (currentState != CombatState.PlayerTurn) return; // [cite: 34]

        Debug.Log($"Player performs action: {action}");
        switch (action.ToLower())
        { // [cite: 35]
            case "attack": // [cite: 35]
                // --- Attack logic here ---
                // Example: Find the first alive enemy and attack it
                Enemy targetEnemy = enemies.Find(e => e.IsAlive());
                if (targetEnemy != null)
                {
                    // Assume player has some attack power
                    int playerAttackPower = 10; // Example value
                    targetEnemy.TakeDamage(playerAttackPower);
                    Debug.Log($"Player attacks {targetEnemy.enemyId} for {playerAttackPower} damage.");
                }
                else
                {
                    Debug.Log("No enemies left to attack!");
                }
                break; // [cite: 35]
            case "item": // [cite: 36]
                // --- Item usage logic here ---
                Debug.Log("Player uses an item (not implemented).");
                break; // [cite: 36]
            // Add other actions like "defend", "skill", "run"
            default:
                Debug.LogWarning($"Unknown player action: {action}");
                break;
        }
        currentState = CombatState.Resolving; // [cite: 37]
    }

    bool CheckCombatEnd() // [cite: 38]
    {
        // Check player alive (you'll need a player health reference)
        // For now, let's assume player is always alive for this example.
        // bool playerAlive = player != null && player.IsAlive(); // Assuming PlayerController has IsAlive()
        bool playerAlive = true; // Placeholder

        bool anyEnemyAlive = false;
        foreach (Enemy enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                anyEnemyAlive = true;
                break;
            }
        }

        if (!playerAlive)
        {
            Debug.Log("Combat Over. Player Defeated!");
            currentState = CombatState.CombatOver;
            return true;
        }
        if (!anyEnemyAlive)
        {
            Debug.Log("Combat Over. All Enemies Defeated!");
            currentState = CombatState.CombatOver;
            return true;
        }

        return false; // [cite: 38]
    }

    void EndCombat() // [cite: 39]
    {
        Debug.Log("Combat has ended.");
        // --- Loot, experience, or continue game logic here ---
        // Example: GameManager.Instance.ReturnToExploration();
    }

    // --- Helper for testing ---
    void Update()
    {
        if (currentState == CombatState.PlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) // Press '1' to simulate attack
            {
                PlayerAction("attack");
            }
        }
    }
}