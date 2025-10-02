using UnityEngine;
using System.Collections.Generic;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data (Progression)")]
    public DialogueData[] dialogues;

    [Header("NPC Identity")]
    [Tooltip("Unique ID for this NPC. Must be unique across ALL scenes!")]
    public string npcID = "NPC_Villager_01";

    void Start()
    {
        if (string.IsNullOrEmpty(npcID))
        {
            Debug.LogError($"NPCDialogue on {gameObject.name}: npcID is empty! Dialogue progress won't persist.", this);
        }

        Debug.Log($"[NPCDialogue] {npcID} initialized in scene with {dialogues.Length} dialogues");
    }

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
            // Get the current dialogue index from DialogueManager
            int currentDialogueIndex = DialogueManager.Instance.GetNPCDialogueIndex(npcID);

            // Clamp to valid range
            if (currentDialogueIndex >= dialogues.Length)
            {
                currentDialogueIndex = dialogues.Length - 1;
                Debug.Log($"[NPCDialogue] {npcID} clamped to final dialogue");
            }

            DialogueData dialogueToPlay = dialogues[currentDialogueIndex];

            if (dialogueToPlay != null)
            {
                Debug.Log($"[NPCDialogue] {npcID} starting dialogue {currentDialogueIndex}");
                // NEW: Pass the total number of dialogues
                DialogueManager.Instance.StartDialogue(dialogueToPlay, this, npcID, dialogues.Length);
            }
        }
    }
}