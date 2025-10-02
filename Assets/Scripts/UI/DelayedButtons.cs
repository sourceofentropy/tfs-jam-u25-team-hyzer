using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DelayedButtons : MonoBehaviour
{
    [Header("Buttons")]
    public Button menuButton;
    public Button quitButton;

    [Header("Settings")]
    public float delay = 5f; // Time before showing buttons

    private float timer;

    void Start()
    {
        // Hide buttons completely at start
        if (menuButton != null) menuButton.gameObject.SetActive(false);
        if (quitButton != null) quitButton.gameObject.SetActive(false);

        timer = delay;

        // Assign button functions
        if (menuButton != null)
            menuButton.onClick.AddListener(OpenMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        // Countdown until buttons become visible
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                if (menuButton != null) menuButton.gameObject.SetActive(true);
                if (quitButton != null) quitButton.gameObject.SetActive(true);
            }
        }
    }

    void OpenMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Stop play mode in Editor
#else
        Application.Quit(); // Quit in build
#endif
    }
}
