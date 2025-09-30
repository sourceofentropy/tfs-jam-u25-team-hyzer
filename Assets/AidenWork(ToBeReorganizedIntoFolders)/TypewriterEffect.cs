using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public float charactersPerSecond = 20f;

    private TMP_Text textComponent;
    private string fullText;
    private Coroutine typeRoutine;
    private bool isRunning;

    public bool IsRunning => isRunning;   // check if currently animating

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    public void Play(string text)
    {
        fullText = text;
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = StartCoroutine(TypeText());
    }

    public void Skip() // instantly reveal
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
        }
        textComponent.text = fullText;
        isRunning = false;
    }

    private IEnumerator TypeText()
    {
        isRunning = true;
        textComponent.text = "";
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(1f / charactersPerSecond);
        }
        typeRoutine = null;
        isRunning = false;
    }
}
