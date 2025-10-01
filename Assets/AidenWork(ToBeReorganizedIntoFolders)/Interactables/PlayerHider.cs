using UnityEngine;

public class PlayerHider : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private int[] originalOrders;
    private string[] originalLayers;

    private bool isHidden;
    private HideableObject currentHideSpot;

    public bool IsHidden => isHidden;

    void Awake()
    {
        // Cache all renderers (player may have multiple sprites)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalOrders = new int[spriteRenderers.Length];
        originalLayers = new string[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalOrders[i] = spriteRenderers[i].sortingOrder;
            originalLayers[i] = spriteRenderers[i].sortingLayerName;
        }

        Debug.Log($"[PlayerHider] Initialized with {spriteRenderers.Length} renderers");
    }

    public void Hide(HideableObject hideableObject, int hideableSortingOrder, string hideableLayer)
    {
        Debug.Log($"[PlayerHider] Hiding behind {hideableLayer}:{hideableSortingOrder}");

        isHidden = true;
        currentHideSpot = hideableObject;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            // Move to the same layer as the hideable
            spriteRenderers[i].sortingLayerName = hideableLayer;

            // Go just behind the hideable object
            spriteRenderers[i].sortingOrder = hideableSortingOrder - 1;
        }
    }

    public void Unhide()
    {
        Debug.Log("[PlayerHider] Unhide called, restoring original sorting");

        isHidden = false;
        currentHideSpot = null;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = originalOrders[i];
            spriteRenderers[i].sortingLayerName = originalLayers[i];
        }
    }

    public void ForceUnhideIfLeftArea(HideableObject hideableObject)
    {
        if (isHidden && currentHideSpot == hideableObject)
        {
            Debug.Log("[PlayerHider] Force unhiding - left the hide area");
            Unhide();
        }
    }
}
