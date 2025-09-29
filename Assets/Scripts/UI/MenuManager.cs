using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadArea1()
    {
        SceneManager.LoadScene("area1");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
#endif
    }
}
