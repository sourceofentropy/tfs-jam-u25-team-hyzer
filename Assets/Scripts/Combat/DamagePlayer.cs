using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    
    public int damageAmount = 1;
    public string playerTag = "Player";

    public bool destroyOnDamage;
    public GameObject destroyEffect;
    public FearedState FS;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == playerTag)
        {
            DealDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == playerTag)
        {
            DealDamage();
        }
    }

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

    void DealDamage()
    {
        //if FearedState.Weak = true
        //{
        // 
        //}
        //DamageAmount = DamageAmount + FS.Damage()

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
