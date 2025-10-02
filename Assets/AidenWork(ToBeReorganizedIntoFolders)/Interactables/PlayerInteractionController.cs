using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private IInteractable currentInteractable;
    private PlayerController player;

    void Start()
    {
        player = GameManager.Instance.PlayerInstance;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (player != null && player.currentState == PlayerController.PlayerState.Regular)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                InteractionPrompt.Instance?.Show(interactable.GetOptions());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (player != null && player.currentState == PlayerController.PlayerState.Regular)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && interactable == currentInteractable)
            {
                // ✅ NEW: stop dialogue if player walks away mid-conversation
                NPCDialogue npc = other.GetComponent<NPCDialogue>();
                if (npc != null && DialogueManager.Instance != null && DialogueManager.Instance.CurrentlyActive)
                {
                    DialogueManager.Instance.StopDialogue(npc);
                }

                currentInteractable = null;
                InteractionPrompt.Instance?.Hide();
            }
        }
    }

    void Update()
    {
        if (currentInteractable == null) return;

        foreach (var option in currentInteractable.GetOptions())
        {
            if (Input.GetKeyDown(option.key))
            {
                currentInteractable.Interact(option.key, gameObject);
            }
        }
    }
}
