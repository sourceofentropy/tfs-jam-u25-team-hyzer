using UnityEngine;
using System.Collections.Generic;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Sensor")]
    [SerializeField] private Vector2 sensorSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 sensorOffset = Vector2.zero; // Offset from player pivot
    [SerializeField] private LayerMask interactableMask;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private IInteractable currentInteractable;
    private Collider2D currentCollider;
    private PlayerController player;
    private PlayerController.PlayerState lastKnownState;

    void Start()
    {
        player = GameManager.Instance.PlayerInstance;
        if (player == null)
        {
            Debug.LogError("[PlayerInteractionController] Player instance is null!");
        }
        else
        {
            lastKnownState = player.currentState;
        }
    }

    void FixedUpdate()
    {
        // Refresh player reference if needed
        if (player == null)
            player = GameManager.Instance.PlayerInstance;

        // Early exit if player not Regular — clears any existing prompts
        if (player == null || player.currentState != PlayerController.PlayerState.Regular)
        {
            ClearCurrentInteractable();
            if (showDebugLogs && player != null)
                Debug.Log($"Player not in Regular state (current: {player.currentState}), cleared interactable");
            return;
        }

        // Calculate interaction box position
        Vector2 sensorPos = (Vector2)transform.position + sensorOffset;

        // Check for interactable in range
        Collider2D hit = Physics2D.OverlapBox(sensorPos, sensorSize, 0f, interactableMask);

        if (hit != null)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null && currentCollider != hit)
            {
                // New interactable detected
                ClearCurrentInteractable();

                currentInteractable = interactable;
                currentCollider = hit;

                // Show prompt if there are options
                List<InteractionOption> options = interactable.GetOptions();
                if (options != null && options.Count > 0)
                {
                    InteractionPrompt.Instance?.Show(options);
                    if (showDebugLogs)
                    {
                        Debug.Log($"Showing interaction prompt for {hit.gameObject.name} with {options.Count} options");
                    }
                }
            }
        }
        else
        {
            // Nothing in range
            ClearCurrentInteractable();
        }
    }

    void Update()
    {
        // Check for state changes every frame for immediate response
        if (player != null)
        {
            if (player.currentState != lastKnownState)
            {
                if (showDebugLogs)
                    Debug.Log($"Player state changed from {lastKnownState} to {player.currentState}");

                lastKnownState = player.currentState;

                // If changing away from Regular, clear immediately
                if (player.currentState != PlayerController.PlayerState.Regular)
                {
                    ClearCurrentInteractable();
                    return;
                }
            }
        }

        // Only allow interactions if we have a valid interactable and player is Regular
        if (currentInteractable == null || player == null || player.currentState != PlayerController.PlayerState.Regular)
            return;

        List<InteractionOption> options = currentInteractable.GetOptions();
        if (options == null || options.Count == 0) return;

        foreach (var option in options)
        {
            if (Input.GetKeyDown(option.key))
            {
                if (showDebugLogs)
                    Debug.Log($"Interaction key pressed: {option.key} on {currentCollider?.gameObject.name}");

                currentInteractable.Interact(option.key, gameObject);
                break; // Only allow one interaction per frame
            }
        }
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable == null && currentCollider == null) return;

        if (showDebugLogs)
            Debug.Log($"Clearing interactable: {currentCollider?.gameObject.name ?? "null"}");

        // Stop dialogue if leaving NPC mid-conversation
        if (currentCollider != null)
        {
            NPCDialogue npc = currentCollider.GetComponent<NPCDialogue>();
            if (npc != null && DialogueManager.Instance != null && DialogueManager.Instance.CurrentlyActive)
            {
                if (showDebugLogs)
                    Debug.Log("Stopping dialogue as player left interactable area");
                DialogueManager.Instance.StopDialogue(npc);
            }
        }

        currentInteractable = null;
        currentCollider = null;

        // Hide interaction prompt
        InteractionPrompt.Instance?.Hide();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = currentInteractable != null ? Color.green : Color.cyan;
        Vector2 sensorPos = (Vector2)transform.position + sensorOffset;
        Gizmos.DrawWireCube(sensorPos, sensorSize);

        if (currentCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentCollider.transform.position, 0.3f);
        }
    }
}