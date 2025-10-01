using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ConvertAllToLitMaterial : MonoBehaviour
{
    [MenuItem("Tools/Convert All Sprites & Tilemaps to Sprite-Lit-Default Material")]
    public static void ConvertAll()
    {
        // Load the existing material from the project
        string[] materialGUIDs = AssetDatabase.FindAssets("Sprite-Lit-Default t:Material");
        if (materialGUIDs.Length == 0)
        {
            Debug.LogError("Could not find a Material named 'Sprite-Lit-Default' in the project.");
            return;
        }

        string materialPath = AssetDatabase.GUIDToAssetPath(materialGUIDs[0]);
        Material litMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (litMat == null)
        {
            Debug.LogError("Failed to load the 'Sprite-Lit-Default' material.");
            return;
        }

        int spriteCountScene = 0;
        int tilemapCountScene = 0;
        int spriteCountPrefabs = 0;
        int tilemapCountPrefabs = 0;

        // --- Step 1: Convert all SpriteRenderers in currently open scenes ---
        SpriteRenderer[] sceneSprites = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in sceneSprites)
        {
            sr.material = litMat;
            EditorUtility.SetDirty(sr);
            spriteCountScene++;
        }

        // --- Step 2: Convert all TilemapRenderers in currently open scenes ---
        TilemapRenderer[] sceneTilemaps = Object.FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None);
        foreach (var tm in sceneTilemaps)
        {
            tm.material = litMat;
            EditorUtility.SetDirty(tm);
            tilemapCountScene++;
        }

        // Save scenes
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();

        // --- Step 3: Convert all SpriteRenderers and TilemapRenderers in prefabs ---
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            bool modified = false;

            if (prefab != null)
            {
                // SpriteRenderers in prefab
                SpriteRenderer[] prefabSprites = prefab.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var sr in prefabSprites)
                {
                    sr.material = litMat;
                    modified = true;
                    spriteCountPrefabs++;
                }

                // TilemapRenderers in prefab
                TilemapRenderer[] prefabTilemaps = prefab.GetComponentsInChildren<TilemapRenderer>(true);
                foreach (var tm in prefabTilemaps)
                {
                    tm.material = litMat;
                    modified = true;
                    tilemapCountPrefabs++;
                }

                if (modified)
                    EditorUtility.SetDirty(prefab);
            }
        }

        // Save all assets
        AssetDatabase.SaveAssets();

        Debug.Log($"Conversion complete:\n" +
                  $"{spriteCountScene} sprites in scenes\n" +
                  $"{tilemapCountScene} tilemaps in scenes\n" +
                  $"{spriteCountPrefabs} sprites in prefabs\n" +
                  $"{tilemapCountPrefabs} tilemaps in prefabs\n" +
                  $"All assigned to 'Sprite-Lit-Default' material.");
    }
}
