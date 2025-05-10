using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int strength = 10; // Base strength, can be modified by archetypes/items

    [Header("Resistances & Other Stats")]
    // Example: public int defense = 5;
    // Example: public float movementSpeed = 5f;

    [Header("Pandora Specific Stats")]
    public int hope = 10; // As mentioned in EnemyAI.cs for the "Despair" enemy

    // --- Events ---
    // Optional: You can add events to notify other systems when stats change
    // public static event System.Action OnPlayerHealthChanged;
    // public static event System.Action OnPlayerDied;

    void Awake()
    {
        // Initialize stats, could be loaded or set by an ArchetypeManager later
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Example: If ArchetypeManager sets stats, it might do so after Awake.
        // If you have an ArchetypeManager applying stats, ensure currentHealth is also set there.
        // If not, initialize here or in Awake.
        // currentHealth = maxHealth; // Moved to Awake for earlier initialization
    }

    public void InitializeStats(int health, int str, int initialHope)
    {
        maxHealth = health;
        currentHealth = maxHealth; // Start with full health based on archetype
        strength = str;
        hope = initialHope;
        Debug.Log($"PlayerStats Initialized: HP: {currentHealth}/{maxHealth}, STR: {strength}, Hope: {hope}");
    }

    public void TakeDamage(int amount)
    {
        if (amount < 0) amount = 0; // Prevent negative damage (healing through damage)

        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Current Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        // OnPlayerHealthChanged?.Invoke(); // Optional event
    }

    public void Heal(int amount)
    {
        if (amount < 0) amount = 0; // Prevent negative healing

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"Player healed for {amount}. Current Health: {currentHealth}/{maxHealth}");
        // OnPlayerHealthChanged?.Invoke(); // Optional event
    }

    public void ModifyStrength(int amount)
    {
        strength += amount;
        if (strength < 0) strength = 0; // Prevent negative strength
        Debug.Log($"Player strength modified by {amount}. New Strength: {strength}");
    }

    public void ModifyHope(int amount)
    {
        hope += amount;
        // Add clamps if hope has min/max values (e.g., hope < 0 ? 0 : hope)
        Debug.Log($"Player hope modified by {amount}. New Hope: {hope}");
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    private void Die()
    {
        Debug.Log("Player has died. Game Over sequence should start.");
        // OnPlayerDied?.Invoke(); // Optional event
        // --- Add game over logic here ---
        // For example:
        // Time.timeScale = 0; // Pause the game
        // UIManager.Instance.ShowGameOverScreen();
        // GameManager.Instance.HandlePlayerDeath();
    }

    // --- Getters (optional, but good practice if other scripts only need to read values) ---
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetStrength() => strength;
    public int GetHope() => hope;
}