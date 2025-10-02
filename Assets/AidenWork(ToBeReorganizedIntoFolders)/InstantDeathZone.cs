using UnityEngine;

public class InstantDeathZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("InstantDeathZone: Something entered the trigger! Object name: " + other.gameObject.name + ", Tag: " + other.tag);

        // Check if the object that entered is the player
        if (other.CompareTag(playerTag))
        {
            Debug.Log("InstantDeathZone: PLAYER DETECTED! Starting instant death sequence...");

            // Play sound effect if available
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Kill the player instantly
            if (PlayerHealthController.instance != null)
            {
                Debug.Log("InstantDeathZone: PlayerHealthController found. Current health: " + PlayerHealthController.instance.currentHealth + "/" + PlayerHealthController.instance.maxHealth);
                Debug.Log("InstantDeathZone: Calling DamagePlayer with " + PlayerHealthController.instance.maxHealth + " damage!");

                PlayerHealthController.instance.DamagePlayer(PlayerHealthController.instance.maxHealth);

                Debug.Log("InstantDeathZone: After damage call. New health: " + PlayerHealthController.instance.currentHealth);
            }
            else
            {
                Debug.LogError("InstantDeathZone: PlayerHealthController.instance is NULL!");
            }
        }
        else
        {
            Debug.Log("InstantDeathZone: Object is NOT the player. Expected tag: '" + playerTag + "', Got tag: '" + other.tag + "'");
        }
    }
}