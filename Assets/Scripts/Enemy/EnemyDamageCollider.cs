using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour
{
    private EnemyAttack enemy;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false; // off by default
    }

    public void SetOwner(EnemyAttack owner)
    {
        enemy = owner;
    }

    public void EnableCollider()
    {
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy != null && other.CompareTag(enemy.playerTag))
        {
            enemy.DealDamage(other.gameObject);
        }
    }
}
