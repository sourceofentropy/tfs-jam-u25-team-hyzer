using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed;
    public Rigidbody2D rb;

    public Vector2 moveDir;

    public GameObject impactEffect;

    public int damageAmount = 1;

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveDir * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealthController>().DamageEnemy(damageAmount);
        }

        if(other.tag == "Boss")
        {
            Debug.Log("Damage boss");
            //BossHealthController.instance.currentHealth -= 1;
            BossHealthController.instance.TakeDamage(damageAmount);
        }

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
        //collision.gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        //Destroy(gameObject);
    }
}
