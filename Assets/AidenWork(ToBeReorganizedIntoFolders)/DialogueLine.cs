using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 5)] public string text;
    public Sprite portrait; // optional
    public AudioClip voiceClip; // optional
}
