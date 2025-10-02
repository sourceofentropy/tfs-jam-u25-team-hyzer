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
    public float delay = 5f;       // Time before fade starts
    public float fadeDuration = 1f; // Duration of fade in seconds

    private float timer;
    private bool fading = false;

    private CanvasGroup menuCanvasGroup;
    private CanvasGroup quitCanvasGroup;

    void Start()
    {
        // Add CanvasGroup to buttons if they don't have one
        menuCanvasGroup = GetOrAddCanvasGroup(menuButton);
        quitCanvasGroup = GetOrAddCanvasGroup(quitButton);

        // Start fully transparent and non-interactable
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;

        quitCanvasGroup.alpha = 0f;
        quitCanvasGroup.interactable = false;
        quitCanvasGroup.blocksRaycasts = false;

        timer = delay;

        // Assign button functions
        if (menuButton != null)
            menuButton.onClick.AddListener(OpenMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (!fading)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                fading = true;
                timer = 0f;
            }
        }
        else
        {
            // Fade in buttons
            if (menuCanvasGroup != null)
                FadeIn(menuCanvasGroup);

            if (quitCanvasGroup != null)
                FadeIn(quitCanvasGroup);
        }
    }

    private void FadeIn(CanvasGroup cg)
    {
        if (cg.alpha < 1f)
        {
            // Smoothly move alpha towards 1
            cg.alpha = Mathf.MoveTowards(cg.alpha, 1f, Time.deltaTime / fadeDuration);

            // Enable interaction when fully visible
            if (cg.alpha >= 1f)
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
    }

    private CanvasGroup GetOrAddCanvasGroup(Button button)
    {
        if (button == null) return null;

        CanvasGroup cg = button.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = button.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    void OpenMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
