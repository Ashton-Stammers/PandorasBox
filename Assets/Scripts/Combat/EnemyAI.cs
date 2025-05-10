using UnityEngine;
using System.Collections.Generic; // Not strictly needed by this script but good practice if extending

public enum EnemyType { Gorgon, Bandit, Despair } // [cite: 71]

public class EnemyAI : MonoBehaviour // This script would be a component on an Enemy GameObject
{
    public EnemyType type; // [cite: 71]
    public Vector2Int gridPosition; // [cite: 72]
    public int health = 50; // [cite: 72] Default value
    public int damage = 5;  // [cite: 72] Default value

    private Enemy enemyData; // Link to the Enemy data class for stats

    void Start()
    {
        // Initialize enemyData, perhaps from a spawner or EnemyManager
        // For example:
        // enemyData = new Enemy(type.ToString(), health, damage, gridPosition);
        // Or, if this script is on a prefab that's instantiated by a spawner,
        // the spawner could pass the Enemy data object.
    }

    public void TakeTurn(PlayerController player) // [cite: 72]
    {
        if (player == null)
        {
            Debug.LogWarning($"{name} ({type}) cannot take turn: Player reference is null.");
            return;
        }
        Debug.Log($"{name} ({type}) is taking its turn. Player at {player.gridPosition}");

        switch (type)
        {
            case EnemyType.Gorgon: // [cite: 72]
                ChargePlayer(player.gridPosition); // [cite: 72]
                break; // [cite: 73]

            case EnemyType.Bandit: // [cite: 73]
                if (Random.value > 0.7f)  // [cite: 73]
                    ThrowKnife(player.gridPosition); // [cite: 73]
                else // [cite: 74]
                    MoveTowardPlayer(player.gridPosition); // [cite: 74]
                break; // [cite: 75]

            case EnemyType.Despair: // [cite: 75]
                DrainHope(player); // [cite: 75]
                break; // [cite: 75]
        } // [cite: 76]
    }

    void ChargePlayer(Vector2Int playerPos) // [cite: 76]
    {
        // Simplified movement: gets normalized direction, but Vector2Int can't truly normalize.
        // This will move 1 unit in x and/or 1 unit in y towards player.
        Vector2 directionFloat = ((Vector2)playerPos - (Vector2)gridPosition).normalized;
        Vector2Int dir = new Vector2Int(Mathf.RoundToInt(directionFloat.x), Mathf.RoundToInt(directionFloat.y));

        // TODO: Check if new position is valid/walkable via DungeonGenerator before moving
        gridPosition += dir; // [cite: 77]
        transform.position = new Vector3(gridPosition.x, transform.position.y, gridPosition.y); // Update visual
        Debug.Log($"{name} ({type}) charges towards {playerPos}. New position: {gridPosition}");
    }

    void ThrowKnife(Vector2Int playerPos)
    {
        Debug.Log($"{name} ({type}) throws a knife towards {playerPos} (effect not implemented).");
        // TODO: Implement projectile logic or direct damage to player
    }

    void MoveTowardPlayer(Vector2Int playerPos)
    {
        Vector2 directionFloat = ((Vector2)playerPos - (Vector2)gridPosition).normalized;
        Vector2Int dir = new Vector2Int(Mathf.RoundToInt(directionFloat.x), Mathf.RoundToInt(directionFloat.y));
        // TODO: Check if new position is valid/walkable
        gridPosition += dir;
        transform.position = new Vector3(gridPosition.x, transform.position.y, gridPosition.y); // Update visual
        Debug.Log($"{name} ({type}) moves towards {playerPos}. New position: {gridPosition}");
    }

    void DrainHope(PlayerController player) // [cite: 77]
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            // stats.ModifyHope(-2); // [cite: 77] // Assuming PlayerStats has this method
            Debug.Log($"{name} ({type}) drains hope from the player (ModifyHope needs implementation in PlayerStats).");
        }
        else
        {
            Debug.LogWarning($"{name} ({type}) tried to drain hope, but PlayerController has no PlayerStats component.");
        }
    }
}