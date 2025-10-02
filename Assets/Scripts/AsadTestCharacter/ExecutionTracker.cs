using UnityEngine;
using TMPro;

public class ExecutionScore : MonoBehaviour
{
    //[Header("UI Reference")]
    //public TextMeshProUGUI executedText; // Assign a TMP text object
    //get ref from game manager instead for now

    [Header("Execution Tracking")]
    public int executed = 0;
    public int executedWithRage = 0;
    public int executedInSilence = 0;

    //[Header("References")]
    //public TestingPlayerController player; // Drag PlayerController here
    //get from game manager instead
    private PlayerController player;

    public enum Type
    {
        silence,
        rage
    }

    void Start()
    {
        UpdateUI();
        player = GameManager.Instance.PlayerInstance; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            /* Uncomment when rage state is implemented
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
            */
            executed++;
            Debug.Log("Executed normally. Total Executed: " + executed);
            UpdateUI();
        }
    }

    // -------------------------------
    // Public functions (if needed externally)
    // -------------------------------
    public void AddExecution(Type type)
    {
        Debug.Log("player adds execution to the list");
        executed++;        
        if(type == Type.silence)
        {
            AddExecutionInSilence();
        } else if (type == Type.rage)
        {
            AddExecutionWithRage();
        }
        
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

    public int GetExecutionCount()
    {
        return executed;
    }

    // -------------------------------
    // UI Update
    // -------------------------------
    void UpdateUI()
    {
        /*
        if (executedText != null)
        {
            executedText.text = "Executed: " + executed;
        }
        */
        Debug.Log("player updates ui text");
        GameManager.Instance.harvestCounterHUD.text = executed.ToString();
    }
}
