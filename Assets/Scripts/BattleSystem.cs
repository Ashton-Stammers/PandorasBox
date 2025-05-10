using UnityEngine;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public TMP_Text battleLog;
    public ASCIIArtManager asciiArt;
    private int playerHP = 100;
    private int enemyHP;

    void Start()
    {
        StartNewBattle();
    }

    void StartNewBattle()
    {
        enemyHP = Random.Range(50, 100); // Enemy HP will be between 50 and 99
        if (asciiArt != null) // Added a null check for safety, was implicit in your original structure
        {
            asciiArt.ShowEnemyArt(Random.Range(0, 3));
        }
        UpdateLog($"ENCOUNTER: {GetEnemyName()}\n[1]ATTACK [2]DEFEND [3]ITEM");
    }

    void UpdateLog(string message)
    {
        battleLog.text = message + $"\nPLAYER HP: {playerHP} | ENEMY HP: {enemyHP}";
    }

    string GetEnemyName()
    {
        return new string[] { "PLAGUE RATS", "CHAOS BEAST", "GLITCH ENTITY" }[Random.Range(0, 3)];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ResolveAttack();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ResolveDefend();
    }

    void ResolveAttack()
    {
        int damage = Random.Range(10, 20); // Damage will be between 10 and 19
        enemyHP -= damage;
        UpdateLog($"YOU DEALT {damage} DAMAGE!");
        if (enemyHP <= 0) EndBattle(true);
    }

    void ResolveDefend()
    {
        int recoveredHP = Random.Range(5, 11); // Player recovers between 5 and 10 HP
        playerHP += recoveredHP;
        if (playerHP > 100) // Cap player HP at the initial maximum
        {
            playerHP = 100;
        }
        UpdateLog($"YOU DEFEND AND RECOVER {recoveredHP} HP!");
        // Defending itself does not trigger an EndBattle condition in this context.
    }

    void EndBattle(bool won)
    {
        UpdateLog(won ? "VICTORY!" : "DEFEAT!");
        this.enabled = false; // Exit battle mode
    }
}