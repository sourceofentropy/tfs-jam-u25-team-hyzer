using UnityEngine;

public class EnemyRangeCollider : MonoBehaviour
{
    private EnemyAttack enemy;
    private bool playerInRange = false;

    void Start()
    {
        enemy = GetComponentInParent<EnemyAttack>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemy.playerTag))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(enemy.playerTag))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange)
        {
            enemy.TryAttack();
        }
    }
}
