using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InteractionPrompt : MonoBehaviour
{
    public static InteractionPrompt Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text promptText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        root.SetActive(false);
    }

    public void Show(List<InteractionOption> options)
    {
        if (options == null || options.Count == 0)
        {
            Hide();
            return;
        }

        promptText.text = "";
        foreach (var opt in options)
        {
            promptText.text += $"[{opt.key}] {opt.description}\n";
        }

        // Remove trailing newline
        promptText.text = promptText.text.TrimEnd('\n');

        root.SetActive(true);
    }

    public void Hide() => root.SetActive(false);
}