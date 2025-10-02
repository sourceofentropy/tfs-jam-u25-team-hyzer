using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public int totalHealth = 3;
    public GameObject deathEffect;

    [SerializeField] private AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip executeSound;
    

    public void DamageEnemy(int damageAmount)
    {
        totalHealth -= damageAmount;

        if(totalHealth < 0 )
        {
            StartCoroutine(PlayDeadAndWait(deathSound));
            //audioSource.PlayOneShot(deathSound);
            
            
        }
    }

    public void ExecuteEnemy()
    {
        totalHealth = 0;
        StartCoroutine(PlayDeadAndWait(executeSound));
        //audioSource.PlayOneShot(executeSound);        

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

    public void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (executeSound != null && audioSource != null)
            {
                Debug.Log("Playing clip...");
                audioSource.PlayOneShot(executeSound);
            }
        }*/
    }    
    IEnumerator PlayDeadAndWait(AudioClip sample)
    {
        audioSource.PlayOneShot(sample);
        yield return new WaitForSeconds(sample.length);

        Debug.Log("Sound finished playing!");
        // Trigger your "complete" logic here
        Death();
    }

}
