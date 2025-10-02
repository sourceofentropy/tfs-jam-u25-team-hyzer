using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsTrigger : MonoBehaviour
{
    [SerializeField] private string creditsSceneName = "Credits";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable player
            other.gameObject.SetActive(false);

            // Find UI Canvas by tag and disable it
            GameObject uiCanvas = GameObject.FindGameObjectWithTag("UI Canvas");
            if (uiCanvas != null)
            {
                uiCanvas.SetActive(false);
            }

            // Load credits scene
            SceneManager.LoadScene(creditsSceneName);
        }
    }
}
