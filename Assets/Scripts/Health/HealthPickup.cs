using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount;

    public GameObject pickupEffect;

    public string playerTag = "Player";
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == playerTag)
        {
            PlayerHealthController.instance.HealPlayer(healthAmount);

            if(pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

}
