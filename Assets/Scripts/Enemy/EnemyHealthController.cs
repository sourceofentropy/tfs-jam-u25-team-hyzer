using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public int totalHealth = 3;
    public GameObject deathEffect;
    

    public void DamageEnemy(int damageAmount)
    {
        totalHealth -= damageAmount;

        if(totalHealth < 0 )
        {
            Death();
        }
    }

    public void ExecuteEnemy()
    {
        totalHealth = 0;
        Death();

    }
    public int GetCurrentHealth()
    {
        return totalHealth;
    }

    private void Death()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
