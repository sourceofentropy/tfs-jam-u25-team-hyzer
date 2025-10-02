using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float attackCooldown = 2f;
    private bool isAttacking = false;
    private float lastAttackTime;

    [Header("References")]
    public string playerTag = "Player";
    public FearedState FS;
    public CultBoiAnimsController cbac;
    public EnemyDamageCollider damageCollider; // reference to the damage collider script

    void Start()
    {
        if (damageCollider != null)
            damageCollider.SetOwner(this);
    }

    public void TryAttack()
    {
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        Debug.Log("Enemy attacking");

        if (cbac != null)
            cbac.PlayAttack();

        // Enable damage collider for the attack window
        if (damageCollider != null)
            damageCollider.EnableCollider();

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        if (damageCollider != null)
            damageCollider.DisableCollider();
    }

    public void DealDamage(GameObject player)
    {
        int modifiedDamage = damageAmount + (FS != null ? FS.GetCurrentDamage() : 0);

        PlayerHealthController.instance.DamagePlayer(modifiedDamage);
        Debug.Log($"Damaged player for {modifiedDamage}");
    }
}
