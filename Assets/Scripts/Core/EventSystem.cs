using UnityEngine;
using System.Collections; // Required for IEnumerator
using System.Collections.Generic; // [cite: 84]

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance; // Optional: Singleton

    public List<string> genericEvents = new List<string>(); // [cite: 84]
    public List<string> cursedEvents = new List<string>(); // [cite: 85]

    // You might want a more structured way to define events, e.g., using ScriptableObjects
    // For now, strings are used as keys as per AI output.

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Populate with some example events if lists are empty
        if (genericEvents.Count == 0)
        {
            genericEvents.Add("found_trinket");
            genericEvents.Add("empty_room");
            genericEvents.Add("wounded_knight"); // [cite: 87]
        }
        if (cursedEvents.Count == 0)
        {
            cursedEvents.Add("ambush");
            cursedEvents.Add("stat_drain_trap");
        }
    }

    public void TriggerRandomEvent() // [cite: 85]
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found for EventSystem.");
            return;
        }

        // Determine if a cursed event should be used [cite: 85]
        bool useCursedEvent = GameManager.Instance.activeCurses.Count > 0 && // [cite: 85]
                              Random.value > 0.7f; // [cite: 85]

        List<string> pool = useCursedEvent ? cursedEvents : genericEvents; // [cite: 86]

        if (pool.Count == 0)
        {
            Debug.LogWarning("No events available in the selected pool.");
            GameManager.Instance.UpdateASCIIDisplay("The air is still... nothing happens.");
            return;
        }

        string eventKey = pool[Random.Range(0, pool.Count)]; // [cite: 86]
        Debug.Log($"Triggering event: {eventKey}");
        StartCoroutine(RunEventSequence(eventKey)); // [cite: 86]
    }

    IEnumerator RunEventSequence(string eventKey) // [cite: 87]
    {
        // Sample event structure [cite: 87]
        // You'll need to expand this significantly based on your game's events.
        // This also implies your GameManager's UpdateASCIIDisplay can handle multi-line strings.

        if (GameManager.Instance == null) yield break;

        switch (eventKey)
        {
            case "wounded_knight": // [cite: 87]
                GameManager.Instance.UpdateASCIIDisplay( // [cite: 87]
                    "A wounded knight blocks your path!\n" +
                    "[1] Help him (-1 potion)\n" + // [cite: 88]
                    "[2] Push past (-5 HP)\n" + // [cite: 88]
                    "[3] Open the Box (50% curse)" // [cite: 88]
                );

                yield return new WaitUntil(() => Input.anyKeyDown); // [cite: 89] // Wait for player choice

                // Handle input for choice [cite: 90]
                if (Input.GetKeyDown(KeyCode.Alpha1)) // [cite: 90]
                {
                    GameManager.Instance.UpdateASCIIDisplay("You offer the knight a potion. He thanks you weakly.");
                    // TODO: Implement potion deduction & knight's reaction/reward
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    GameManager.Instance.UpdateASCIIDisplay("You shove the knight aside, taking some damage in the scuffle.");
                    // TODO: Implement HP deduction
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    GameManager.Instance.UpdateASCIIDisplay("You decide to open Pandora's Box instead...");
                    GameManager.Instance.OpenPandorasBox();
                }
                else
                {
                    GameManager.Instance.UpdateASCIIDisplay("The knight sighs as you ponder too long, and fades away.");
                }
                break; // [cite: 89]

            case "found_trinket":
                GameManager.Instance.UpdateASCIIDisplay("You found a small, shiny trinket on the ground!");
                // TODO: Add trinket to inventory or give some minor buff
                break;

            case "empty_room":
                GameManager.Instance.UpdateASCIIDisplay("This room is eerily quiet and empty.");
                break;

            case "ambush":
                GameManager.Instance.UpdateASCIIDisplay("It's an ambush! Enemies appear!");
                // TODO: Initiate combat
                break;

            case "stat_drain_trap":
                GameManager.Instance.UpdateASCIIDisplay("You feel a draining sensation... A curse saps your strength!");
                // TODO: Apply a temporary stat debuff
                break;

            default:
                GameManager.Instance.UpdateASCIIDisplay($"An unknown event '{eventKey}' occurred.");
                break;
        }

        yield return new WaitForSeconds(1.5f); // Give time to read outcome
        GameManager.Instance.UpdateASCIIDisplay(GameManager.Instance.GetFloorASCII()); // Return to floor display
    }
}