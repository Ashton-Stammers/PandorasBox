using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public string interactionText = "INTERACT? [PRESS E]";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            onInteract.Invoke();
        }
    }
}