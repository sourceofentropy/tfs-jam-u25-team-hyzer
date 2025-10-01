using UnityEngine;

public class PlayerHider : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private string originalSortingLayer;
    private bool isHidden;
    private HideableObject currentHideSpot;

    public bool IsHidden => isHidden;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
            originalSortingLayer = spriteRenderer.sortingLayerName;
            Debug.Log($"[PlayerHider] Initialized. Original sorting: {originalSortingLayer} / {originalSortingOrder}");
        }
        else
        {
            Debug.LogError("[PlayerHider] No SpriteRenderer found in children!");
        }
    }

    public void Hide(HideableObject hideableObject, int hideableSortingOrder)
    {
        Debug.Log($"[PlayerHider] Hide called. Hideable sorting order: {hideableSortingOrder}");
        isHidden = true;
        currentHideSpot = hideableObject;

        // Render behind the hideable object (one layer below it)
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = hideableSortingOrder - 1;
            Debug.Log($"[PlayerHider] Sorting order changed to {hideableSortingOrder - 1}");
        }
    }

    public void Unhide()
    {
        Debug.Log($"[PlayerHider] Unhide called. Restoring sorting order to {originalSortingOrder}");
        isHidden = false;
        currentHideSpot = null;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
            spriteRenderer.sortingLayerName = originalSortingLayer;
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