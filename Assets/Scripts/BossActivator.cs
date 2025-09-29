using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivator : MonoBehaviour
{
    public GameObject bossToActivate;
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == playerTag)
        {
            bossToActivate.SetActive(true);
            gameObject.SetActive(false);
        }

        
        
    }
}
