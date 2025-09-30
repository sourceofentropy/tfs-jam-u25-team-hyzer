using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Data (Progression)")]
    public DialogueData[] dialogues; // assign multiple dialogue assets in order

    [Header("Interaction")]
    public GameObject interactPrompt; // "Press E" UI

    private int currentDialogueIndex = 0;
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactPrompt?.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactPrompt?.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance.CurrentlyActive)
            {
                // Continue dialogue
                DialogueManager.Instance.NextLine();
            }
            else if (dialogues != null && dialogues.Length > 0)
            {
                // Start the current dialogue
                DialogueData dialogueToPlay = dialogues[currentDialogueIndex];
                if (dialogueToPlay != null)
                {
                    DialogueManager.Instance.StartDialogue(dialogueToPlay);

                    // Advance index for next time (but clamp at last dialogue)
                    if (currentDialogueIndex < dialogues.Length - 1)
                        currentDialogueIndex++;
                }
            }
        }
    }
}
