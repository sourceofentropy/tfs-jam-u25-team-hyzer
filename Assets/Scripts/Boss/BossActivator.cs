using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivator : MonoBehaviour
{
    public GameObject bossToActivate;
    public string playerTag = "Player";
    public int harvestedSoulsRequired = 0;
    public int bossBattle = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((bossBattle == 1)) 
        {
            if(!GameManager.Instance.isBoss1Dead)
            {
                SpawnBoss(other);
            }
        }
        if ((bossBattle == 2))
        {
            if (!GameManager.Instance.isBoss2Dead)
            {
                SpawnBoss(other);
            }
        }
    }

    private void SpawnBoss(Collider2D other)
    {
        if (other.tag == playerTag && GameManager.Instance.harvestScore.GetExecutionCount() >= harvestedSoulsRequired)
        {
            bossToActivate.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
