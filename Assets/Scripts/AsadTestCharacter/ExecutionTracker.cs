using UnityEngine;
using TMPro;

public class ExecutionScore : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI executedText; // Assign a TMP text object

    [Header("Execution Tracking")]
    public int executed = 0;
    public int executedWithRage = 0;
    public int executedInSilence = 0;

    [Header("References")]
    public TestingPlayerController player; // Drag PlayerController here

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (player != null && player.GetPlayerState() == TestingPlayerController.PlayerState.Rage)
            {
                executedWithRage++;
                Debug.Log("Executed WITH RAGE! Total Rage Executions: " + executedWithRage);
            }
            else
            {
                executed++;
                Debug.Log("Executed normally. Total Executed: " + executed);
                UpdateUI();
            }
        }
    }

    // -------------------------------
    // Public functions (if needed externally)
    // -------------------------------
    public void AddExecution()
    {
        executed++;
        UpdateUI();
    }

    public void AddExecutionWithRage()
    {
        executedWithRage++;
        Debug.Log("Executed WITH RAGE! Total Rage Executions: " + executedWithRage);
    }

    public void AddExecutionInSilence()
    {
        executedInSilence++;
        Debug.Log("Executed IN SILENCE. Total Silent Executions: " + executedInSilence);
    }

    // -------------------------------
    // UI Update
    // -------------------------------
    void UpdateUI()
    {
        if (executedText != null)
        {
            executedText.text = "Executed: " + executed;
        }
    }
}
