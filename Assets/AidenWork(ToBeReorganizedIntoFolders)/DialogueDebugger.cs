using UnityEngine;

public class DialogueDebugger : MonoBehaviour
{
    void Update()
    {
        // Press P to see all dialogue progress
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.DebugLogAllProgress();
            }
            else
            {
                Debug.LogError("[DEBUG] DialogueManager.Instance is NULL!");
            }
        }

        // Press R to reset ALL dialogue (for testing)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ResetAllDialogue();
                Debug.Log("[DEBUG] All dialogue progress reset!");
            }
        }
    }
}