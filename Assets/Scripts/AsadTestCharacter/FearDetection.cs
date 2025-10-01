using UnityEngine;

public class FearDetection : MonoBehaviour
{
    [Header("References")]
    public EnemyAI enemyAI;
    public FearedState fearedState;       // link the FearedState component
    public CircleCollider2D fearCollider; // assign in Inspector
    public float fearDuration = 4f;

    private float fearTimer;

    private void Update()
    {
        // countdown timer if feared
        if (enemyAI != null && enemyAI.GetCurrentState() == EnemyAI.EnemyState.Feared)
        {
            fearTimer -= Time.deltaTime;
            if (fearTimer <= 0f)
            {
                enemyAI.SetFeared(false);
                if (fearedState != null)
                {
                    fearedState.enabled = false; // turn off fear behaviour
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyFear(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemyAI.GetCurrentState() == EnemyAI.EnemyState.Feared) return;
        TryApplyFear(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI.SetFeared(false);
            if (fearedState != null)
            {
                fearedState.enabled = false;
            }
        }
    }

    private void TryApplyFear(Collider2D other)
    {
        TestingPlayerController playerCtrl = other.GetComponent<TestingPlayerController>();
        if (playerCtrl != null && playerCtrl.currentState == TestingPlayerController.PlayerState.Rage)
        {
            enemyAI.SetFeared(true);
            fearTimer = fearDuration;

            if (fearedState != null)
            {
                fearedState.enabled = true; // activate feared behaviour
            }
        }
    }
}
