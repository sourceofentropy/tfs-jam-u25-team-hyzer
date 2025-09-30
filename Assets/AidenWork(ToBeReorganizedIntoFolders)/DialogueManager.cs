using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public TypewriterEffect typewriter;
    public Image portraitImage;   // <-- reference to UI slot

    public bool CurrentlyActive => currentDialogue != null;
    private DialogueData currentDialogue;
    private int currentIndex;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    public void NextLine()
    {
        if (currentDialogue == null) return;

        //  Case 1: Typewriter still running ? skip instead of advancing
        if (typewriter.IsRunning)
        {
            typewriter.Skip();
            return;
        }

        //  Case 2: Fully revealed ? move to next line
        if (currentIndex < currentDialogue.lines.Length - 1)
        {
            currentIndex++;
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowLine()
    {
        var line = currentDialogue.lines[currentIndex];

        nameText.text = line.speakerName;
        typewriter.Play(line.text);

        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.enabled = true;
            }
            else
            {
                portraitImage.enabled = false; // hide if no portrait
            }
        }
    }


    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentDialogue = null;
    }
}
