using UnityEngine;
using TMPro;
using System.Collections;

public class BattleSystem : MonoBehaviour
{
    public TMP_Text battleLog;
    public ASCIIArtManager asciiArt; // Assign in Inspector
    public int playerMaxHP = 100; // You might want to get this from a player stats script later
    private int playerHP;
    private int enemyHP;
    private string enemyName; // Store enemy name for consistent messaging

    public bool PlayerWonLastBattle { get; private set; } = false;
    private bool battleIsOver = false; // Flag to prevent actions after battle ends

    // Call this to initialize the battle from MainGameController
    public void StartNewBattle()
    {
        battleIsOver = false;
        playerHP = playerMaxHP;
        enemyName = GetRandomEnemyName(); // Get and store the enemy name
        enemyHP = Random.Range(50, 100);
        PlayerWonLastBattle = false;

        if (asciiArt != null)
        {
            // Before: asciiArt.ShowEnemyArt(Random.Range(0, 3));
            asciiArt.ShowEnemyArtByIndex(Random.Range(0, 3)); // MODIFIED LINE
        }
        else
        {
            Debug.LogError("BattleSystem: asciiArt reference not set!");
        }

        // Initial battle prompt
        DisplayBattleStatus($"ENCOUNTER: {enemyName}!");
    }

    string GetRandomEnemyName()
    {
        // Returns one of the predefined enemy names randomly.
        return new string[] { "PLAGUE RATS", "CHAOS BEAST", "GLITCH ENTITY" }[Random.Range(0, 3)];
    }

    void Update()
    {
        if (battleIsOver || !this.enabled || !gameObject.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ResolvePlayerAction(BattleAction.Attack);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ResolvePlayerAction(BattleAction.Defend);
        // else if (Input.GetKeyDown(KeyCode.Alpha3)) ResolvePlayerAction(BattleAction.Item); // For future
    }

    enum BattleAction { Attack, Defend, Item }

    void ResolvePlayerAction(BattleAction action)
    {
        if (battleIsOver) return;

        string turnLog = "";

        // Player's Turn
        if (action == BattleAction.Attack)
        {
            int playerDamage = Random.Range(10, 20);
            enemyHP -= playerDamage;
            turnLog += $"YOU DEALT {playerDamage} DAMAGE TO {enemyName}!";
            if (enemyHP <= 0)
            {
                enemyHP = 0; // Prevent negative HP display
                turnLog += $"\n{enemyName} VANQUISHED!";
                DisplayBattleStatus(turnLog, false); // Show final player action result
                EndBattle(true);
                return;
            }
        }
        else if (action == BattleAction.Defend)
        {
            int recoveredHP = Random.Range(5, 11);
            playerHP += recoveredHP;
            if (playerHP > playerMaxHP) playerHP = playerMaxHP;
            turnLog += $"YOU DEFEND AND RECOVER {recoveredHP} HP!";
        }
        // else if (action == BattleAction.Item) { /* Item logic here */ }

        // Enemy's Turn (if player didn't win)
        int enemyDamage = (action == BattleAction.Defend) ? Random.Range(3, 10) : Random.Range(5, 15);
        playerHP -= enemyDamage;
        turnLog += $"\n{enemyName} RETALIATES FOR {enemyDamage} DAMAGE!";

        if (playerHP <= 0)
        {
            playerHP = 0; // Prevent negative HP display
            turnLog += $"\nYOU HAVE FALLEN!";
            DisplayBattleStatus(turnLog, false); // Show final enemy action result
            EndBattle(false);
            return;
        }

        // If battle is not over, display turn results and re-prompt
        DisplayBattleStatus(turnLog);
    }

    /// <summary>
    /// Updates the battle log with the current status and action prompts.
    /// </summary>
    /// <param name="turnMessage">Message from the last turn's actions. Can be empty for initial prompt.</param>
    /// <param name="promptForNextAction">Whether to add the "[1]ATTACK [2]DEFEND..." prompt.</param>
    void DisplayBattleStatus(string turnMessage, bool promptForNextAction = true)
    {
        if (battleLog == null) return;

        string message = "";
        if (!string.IsNullOrEmpty(turnMessage))
        {
            message += turnMessage + "\n";
        }
        message += $"PLAYER HP: {playerHP}/{playerMaxHP} | {enemyName} HP: {enemyHP}";

        if (promptForNextAction && !battleIsOver)
        {
            message += "\n[1]ATTACK [2]DEFEND [3]ITEM (N/A)";
        }
        battleLog.text = message;
    }


    void EndBattle(bool won)
    {
        if (battleIsOver) return; // Prevent multiple calls
        battleIsOver = true;
        PlayerWonLastBattle = won;

        // Display final outcome message clearly
        string finalMessage = won ? $"VICTORY! {enemyName} DEFEATED." : $"DEFEAT... YOU WERE OVERCOME BY {enemyName}.";
        // This call to DisplayBattleStatus will show the HPs and the final outcome.
        // We set promptForNextAction to false as the battle is over.
        DisplayBattleStatus(finalMessage, false);

        StartCoroutine(DelayedDisableAndClearUI(won ? 2.0f : 3.0f)); // Longer delay for defeat message
    }

    IEnumerator DelayedDisableAndClearUI(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for player to read the final battle log message

        if (battleLog != null) battleLog.text = ""; // Clear battle log
        if (asciiArt != null) asciiArt.ClearArt();   // Clear ASCII art

        this.enabled = false; // Disable BattleSystem script. MainGameController will hide the GameObject.
    }
}