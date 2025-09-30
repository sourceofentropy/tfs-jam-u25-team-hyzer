using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    [Header("Test Dialogue")]
    public DialogueData testDialogue;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (DialogueManager.Instance.CurrentlyActive)
            {
                DialogueManager.Instance.NextLine();
            }
            else if (testDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(testDialogue);
            }
        }
    }
}
