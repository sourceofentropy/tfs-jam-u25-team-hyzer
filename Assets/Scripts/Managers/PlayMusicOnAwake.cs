using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayMusicOnAwake : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = true; // Keeps playing in a loop
            audioSource.playOnAwake = false; // We'll handle it manually
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No music clip assigned in PlayMusicOnAwake script.");
        }
    }
}
