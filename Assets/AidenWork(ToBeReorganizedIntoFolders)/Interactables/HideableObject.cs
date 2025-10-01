using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class HideableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer objectSprite;
    [SerializeField] private float hiddenAlpha = 0.4f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private GameObject smokeEffectPrefab;

    private PlayerHider currentPlayer;
    private float originalAlpha;
    private bool isPlayerHidden = false;

    void Awake()
    {
        if (objectSprite == null)
            objectSprite = GetComponentInChildren<SpriteRenderer>();

        if (objectSprite != null)
            originalAlpha = objectSprite.color.a;
    }

    public List<InteractionOption> GetOptions()
    {
        return new List<InteractionOption>
        {
            new InteractionOption { key = KeyCode.F, description = isPlayerHidden ? "Unhide" : "Hide" }
        };
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to get PlayerHider from the colliding object OR its parent
            currentPlayer = other.GetComponent<PlayerHider>();
            if (currentPlayer == null)
                currentPlayer = other.GetComponentInParent<PlayerHider>();
            if (currentPlayer == null)
                currentPlayer = other.GetComponentInChildren<PlayerHider>();

            Debug.Log($"[HideableObject] Player entered. PlayerHider found: {currentPlayer != null}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[HideableObject] Player exited");

            // Auto-unhide if player leaves while hidden
            if (currentPlayer != null && isPlayerHidden)
            {
                currentPlayer.ForceUnhideIfLeftArea(this);
                isPlayerHidden = false;

                if (objectSprite != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(FadeAlpha(originalAlpha));
                }
            }

            currentPlayer = null;
        }
    }

    public void Interact(KeyCode key, GameObject player)
    {
        Debug.Log($"[HideableObject] Interact called. Key: {key}, CurrentPlayer: {currentPlayer != null}, IsHidden: {isPlayerHidden}");

        if (key != KeyCode.F)
        {
            Debug.Log($"[HideableObject] Wrong key pressed. Expected F, got {key}");
            return;
        }

        // Get PlayerHider from the player parameter if currentPlayer is null
        if (currentPlayer == null)
        {
            currentPlayer = player.GetComponent<PlayerHider>();
            if (currentPlayer == null)
                currentPlayer = player.GetComponentInParent<PlayerHider>();
            if (currentPlayer == null)
                currentPlayer = player.GetComponentInChildren<PlayerHider>();

            Debug.Log($"[HideableObject] CurrentPlayer was null, fetching from parameter: {currentPlayer != null}");
        }

        if (currentPlayer == null)
        {
            Debug.LogError("[HideableObject] Cannot interact - no PlayerHider component found!");
            return;
        }

        if (isPlayerHidden)
            Unhide();
        else
            Hide();
    }

    private void Hide()
    {
        Debug.Log("[HideableObject] Hiding player");
        isPlayerHidden = true;

        // Pass the hideable object's sorting order so player can render behind it
        int sortingOrder = objectSprite != null ? objectSprite.sortingOrder : 0;
        currentPlayer.Hide(this, sortingOrder);

        if (smokeEffectPrefab != null)
            Instantiate(smokeEffectPrefab, currentPlayer.transform.position, Quaternion.identity);

        if (objectSprite != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeAlpha(hiddenAlpha));
        }
    }

    private void Unhide()
    {
        Debug.Log("[HideableObject] Unhiding player");
        isPlayerHidden = false;
        currentPlayer.Unhide();

        if (smokeEffectPrefab != null)
            Instantiate(smokeEffectPrefab, currentPlayer.transform.position, Quaternion.identity);

        if (objectSprite != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeAlpha(originalAlpha));
        }
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        Color c = objectSprite.color;
        while (!Mathf.Approximately(c.a, targetAlpha))
        {
            c.a = Mathf.MoveTowards(c.a, targetAlpha, fadeSpeed * Time.deltaTime);
            objectSprite.color = c;
            yield return null;
        }
        Debug.Log($"[HideableObject] Fade complete. Final alpha: {objectSprite.color.a}");
    }
}