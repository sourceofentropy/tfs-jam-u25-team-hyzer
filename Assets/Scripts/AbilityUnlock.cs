using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityUnlock : MonoBehaviour
{

    public bool unlockDoubleJump;
    public bool unlockDash;
    public bool unlockBecomeBall;
    public bool unlockDropBomb;
    public string playerTag = "Player";

    public GameObject pickupEffect;

    public string unlockMessage;
    public TMP_Text unlockText;
    public float textTTL = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(playerTag))
        {
            PlayerAbilityTracker player = other.GetComponentInParent<PlayerAbilityTracker>();
            if (unlockDoubleJump)
            {
                player.canDoubleJump = true;
            }

            if (unlockDash)
            {
                player.canDash = true;
            }

            if (unlockBecomeBall)
            {
                player.canBecomeBall = true;
            }

            if (unlockDropBomb)
            {
                player.canDropBomb = true;
            }

            Instantiate(pickupEffect, transform.position, transform.rotation);
            unlockText.transform.parent.SetParent(null); //prevent premature destruction - though there may be other ways
            unlockText.transform.parent.position = transform.position;
            unlockText.text = unlockMessage;
            unlockText.gameObject.SetActive(true);

            Destroy(unlockText.transform.parent.gameObject, textTTL);
            Destroy(gameObject);
        }
    }
}
