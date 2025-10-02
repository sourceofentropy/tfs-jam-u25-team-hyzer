using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private IInteractable currentInteractable;
    private bool wasInDialogue = false; // NEW: Track if we were in dialogue

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;

            // Only show prompt if not in dialogue
            if (DialogueManager.Instance == null || !DialogueManager.Instance.CurrentlyActive)
            {
                InteractionPrompt.Instance?.Show(interactable.GetOptions());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            // If we're leaving an NPC while in dialogue, stop it
            NPCDialogue npc = other.GetComponent<NPCDialogue>();
            if (npc != null && DialogueManager.Instance != null && DialogueManager.Instance.CurrentlyActive)
            {
                DialogueManager.Instance.StopDialogue(npc);
            }

            currentInteractable = null;
            InteractionPrompt.Instance?.Hide();
        }
    }

    void Update()
    {
        if (currentInteractable == null) return;

        // NEW: Check if dialogue just ended and we're still in range
        bool inDialogueNow = DialogueManager.Instance != null && DialogueManager.Instance.CurrentlyActive;

        if (wasInDialogue && !inDialogueNow)
        {
            // Dialogue just ended - re-show the prompt
            InteractionPrompt.Instance?.Show(currentInteractable.GetOptions());
        }

        wasInDialogue = inDialogueNow;

        // Handle input
        foreach (var option in currentInteractable.GetOptions())
        {
            if (Input.GetKeyDown(option.key))
            {
                currentInteractable.Interact(option.key, gameObject);
            }
        }
    }
}