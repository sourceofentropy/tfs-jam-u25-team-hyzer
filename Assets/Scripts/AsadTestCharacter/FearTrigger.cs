using UnityEngine;

public class FearTrigger : MonoBehaviour
{
    private EnemyAI enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TestingPlayerController player = other.GetComponent<TestingPlayerController>();
        if (player != null && player.currentState == TestingPlayerController.PlayerState.Rage)
        {
           // enemy.SetFeared(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TestingPlayerController player = other.GetComponent<TestingPlayerController>();
        if (player != null && player.currentState == TestingPlayerController.PlayerState.Rage)
        {
            //enemy.SetFeared(false);
        }
    }
}
