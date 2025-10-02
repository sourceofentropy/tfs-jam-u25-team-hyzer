using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float attackCooldown = 2f;
    private bool isAttacking = false;
    private float lastAttackTime;
    public string playerTag = "Player";

    [Header("Effects")]
    public bool destroyOnDamage;
    public GameObject destroyEffect;
    public FearedState FS;
    
    [Header("Colliders")]
    public Collider2D rangeCol;
    public Collider2D DamageCol;

    [Header("Animation")]
    public CultBoiAnimsController cbac;

    void Start()
    {
        //cbac = GetComponent<CultBoiAnimsController>();
        //FS = GetComponent<FearedState>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the player
        if (other.CompareTag(playerTag))
        {
            // If the collider that triggered is the range collider
            if (other == rangeCol && !isAttacking)
            {
                Debug.Log("Player in range");
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    StartCoroutine(Attack());
                }
            }

            // If the collider that triggered is the damage collider
            if (other == DamageCol && isAttacking)
            {
                Debug.Log("Dealing damage to player");
                DealDamage();
            }
        }
    }


    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        Debug.Log("Enemy Attacking");
        lastAttackTime = Time.time;
        if (cbac != null)
        {
            cbac.PlayAttack();
        }
        
        yield return new WaitForSeconds(attackCooldown);
        
        isAttacking = false;
    }

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.tag == playerTag)
    //    {
    //        DealDamage();
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(other.tag == playerTag)
    //    {
    //        DealDamage();
    //    }
    //}

    public void TakeDamage(int amount)
    {
        int modifiedDamage = amount + (FS != null ? FS.GetCurrentDamage() : 0);

        PlayerHealthController.instance.DamagePlayer(modifiedDamage);

        if (destroyOnDamage)
        {
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    public void DealDamage()
    {
        
        int modifiedDamage = damageAmount + FS.GetCurrentDamage();
    
        PlayerHealthController.instance.DamagePlayer(modifiedDamage);

        if(destroyOnDamage)
        {
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
