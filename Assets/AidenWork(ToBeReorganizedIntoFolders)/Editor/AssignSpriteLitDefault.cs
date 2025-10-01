using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class ConvertSpritesToLit : MonoBehaviour
{
    [MenuItem("Tools/Convert All Sprites to Sprite-Lit-Default")]
    public static void ConvertAllSprites()
    {
        // Find the shader
        Shader litShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
        if (litShader == null)
        {
            Debug.LogError("Could not find Sprite-Lit-Default shader! Make sure URP 2D Renderer is installed.");
            return;
        }

        Material litMat = new Material(litShader);

        int sceneCount = 0;
        int prefabCount = 0;

        // --- Step 1: Convert all sprites in currently open scenes ---
        SpriteRenderer[] sceneSprites = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in sceneSprites)
        {
            sr.material = litMat;
            sceneCount++;
            EditorUtility.SetDirty(sr); // mark object dirty for saving
        }

        // Save scenes
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();

        // --- Step 2: Convert all sprites in prefabs ---
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            bool modified = false;

            if (prefab != null)
            {
                SpriteRenderer[] prefabSprites = prefab.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var sr in prefabSprites)
                {
                    sr.material = litMat;
                    modified = true;
                }

                if (modified)
                {
                    EditorUtility.SetDirty(prefab);
                    prefabCount++;
                }
            }
        }

        // Save all assets
        AssetDatabase.SaveAssets();

        Debug.Log($"Assigned Sprite-Lit-Default to {sceneCount} sprites in open scenes and {prefabCount} prefabs.");
    }
}
