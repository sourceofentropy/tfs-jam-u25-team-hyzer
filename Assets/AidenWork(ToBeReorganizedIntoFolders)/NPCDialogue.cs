using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data (Progression)")]
    public DialogueData[] dialogues;
    private int currentDialogueIndex = 0;

    public List<InteractionOption> GetOptions()
    {
        return new List<InteractionOption>
        {
            new InteractionOption { key = KeyCode.E, description = "Talk" }
        };
    }

    public void Interact(KeyCode key, GameObject player)
    {
        if (key != KeyCode.E) return;

        if (DialogueManager.Instance.CurrentlyActive)
        {
            DialogueManager.Instance.NextLine();
        }
        else if (dialogues != null && dialogues.Length > 0)
        {
            DialogueData dialogueToPlay = dialogues[currentDialogueIndex];
            if (dialogueToPlay != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueToPlay);

                if (currentDialogueIndex < dialogues.Length - 1)
                    currentDialogueIndex++;
            }
        }
    }
}