using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    string playerTag = "Player";
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == playerTag)
        {
            RespawnController.instance.SetSpawn(transform.position);
        }
    }
}
