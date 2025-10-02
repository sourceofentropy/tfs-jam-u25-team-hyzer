using UnityEngine;

public class PlayerHider : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private int[] originalOrders;
    private string[] originalSortingLayers;
    private bool isHidden;
    private HideableObject currentHideSpot;

    [Header("Collision Settings")]
    [SerializeField] private string hiddenLayerName = "HiddenPlayer";
    private Transform[] allChildren;
        
    public Transform playerSpriteTransform;
    public SpriteRenderer playerSpriteRenderer;
    private GameObject parent; //player
    private int originalPlayerPhysicsLayer;
    private int originalSpritePhysicsLayer;
    private string originalSpriteSortingLayerName;
    private int originalSpriteSortingOrder;
    private string originalPlayerSortingLayerName;

    private int[] originalPhysicsLayers;

    public bool IsHidden => isHidden;

    void Awake()
    {
        parent = transform.root.gameObject;

        // Cache all renderers (player may have multiple sprites)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalOrders = new int[spriteRenderers.Length];
        originalSortingLayers = new string[spriteRenderers.Length];

        //may no longer be using in favour of just player parent and sprite renderer
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalOrders[i] = spriteRenderers[i].sortingOrder;
            originalSortingLayers[i] = spriteRenderers[i].sortingLayerName;
        }

        // Cache all transforms (parent + all children) and their original layers
        allChildren = GetComponentsInChildren<Transform>(true);
        originalPhysicsLayers = new int[allChildren.Length];

        //may no longer be using in favour of just player parent and sprite renderer
        for (int i = 0; i < allChildren.Length; i++)
        {
            originalPhysicsLayers[i] = allChildren[i].gameObject.layer;
        }


        Debug.Log($"[PlayerHider] Initialized with {spriteRenderers.Length} renderers and {allChildren.Length} objects");
    }

    public void Hide(HideableObject hideableObject, int hideableSortingOrder, string hideableLayer)
    {
        Debug.Log($"[PlayerHider] Hiding behind {hideableLayer}:{hideableSortingOrder}");
        isHidden = true;
        currentHideSpot = hideableObject;

        //may no longer be using for loop in favour of just player parent and sprite renderer
        // Update visual sorting
        /*
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            // Move to the same sorting layer as the hideable
            spriteRenderers[i].sortingLayerName = hideableLayer;
            // Go just behind the hideable object
            spriteRenderers[i].sortingOrder = hideableSortingOrder - 1;
        }*/
        originalSpriteSortingLayerName = playerSpriteRenderer.sortingLayerName;
        originalSpriteSortingOrder = playerSpriteRenderer.sortingOrder;
        playerSpriteRenderer.sortingLayerName = hideableLayer;
        playerSpriteRenderer.sortingOrder = hideableSortingOrder - 1;        

        // Change physics layer to disable enemy collision for ALL objects
        int hiddenLayer = LayerMask.NameToLayer(hiddenLayerName);

        /*
        for (int i = 0; i < allChildren.Length; i++)
        {
            allChildren[i].gameObject.layer = hiddenLayer;
        }
        Debug.Log($"[PlayerHider] Changed entire hierarchy to layer: {hiddenLayerName}");
        */

        parent.layer = hiddenLayer;
        gameObject.layer = hiddenLayer;

        Debug.Log($"[PlayerHider] Changed player and sprite renderer to layer: {hiddenLayerName}");
    }

    public void Unhide()
    {
        Debug.Log("[PlayerHider] Unhide called, restoring original sorting");
        isHidden = false;
        currentHideSpot = null;

        playerSpriteRenderer.sortingLayerName = originalSpriteSortingLayerName;
        playerSpriteRenderer.sortingOrder = originalSpriteSortingOrder;

        /*
        // Restore visual sorting
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = originalOrders[i];
            spriteRenderers[i].sortingLayerName = originalSortingLayers[i];
        }
        */

        parent.layer = originalPlayerPhysicsLayer;
        gameObject.layer = originalSpritePhysicsLayer;
        Debug.Log($"[PlayerHider] Restored player and its sprite renderer to original layers");
        /*
        // Restore original physics layer for ALL objects
        for (int i = 0; i < allChildren.Length; i++)
        {
            allChildren[i].gameObject.layer = originalPhysicsLayers[i];
        }
        Debug.Log($"[PlayerHider] Restored entire hierarchy to original layers");
        */
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