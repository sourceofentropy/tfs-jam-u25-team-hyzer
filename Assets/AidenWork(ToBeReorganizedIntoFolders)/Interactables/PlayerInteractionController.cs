using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private IInteractable currentInteractable;

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            InteractionPrompt.Instance?.Show(interactable.GetOptions());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
            InteractionPrompt.Instance?.Hide();
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