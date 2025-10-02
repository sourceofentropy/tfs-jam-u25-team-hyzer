using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroller : MonoBehaviour
{
    public Transform[] patrolPoints;
    private int currentPoint;

    public float moveSpeed, waitAtPoints;
    public float distanceThreshold = 0.2f;
    private float waitCounter;

    public float jumpForce;

    public bool isReadyForHarvest = false;
    public SpriteRenderer harvestHalo;
    public float debugHaloHeight = 0.2f;
    public float debugHaloSize = 0.2f;
    public Color debugHaloColour = Color.red;
    public Color haloExecuteColour = Color.red;
    public Color haloDefaultColour;

    public Rigidbody2D rb;
    public Animator anim;

    public PlayerController PC;

    public enum EnemyState { Feared, Patrol, Wait, Attack }
    public EnemyState currentState = EnemyState.Wait;
    public EnemyState previousState = EnemyState.Wait;

    // Start is called before the first frame update
    void Start()
    {
        waitCounter = waitAtPoints;
        haloDefaultColour = harvestHalo.color;

        foreach (Transform patrolPoint in patrolPoints)
        {
            patrolPoint.SetParent(null);
        }

        if(isReadyForHarvest)
        {
            ActivateHarvestHalo();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPoints.Length == 0)
        {
            throw new System.Exception("Enemy instance has no Patrol Points assigned");
        }

        if(Mathf.Abs(transform.position.x - patrolPoints[currentPoint].position.x) > distanceThreshold)
        {
            if (transform.position.x < patrolPoints[currentPoint].position.x)
            {
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
                transform.localScale = new Vector3(-1f, 1f, 1f);
            } else
            {
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
                transform.localScale = Vector3.one;
            }

            if(transform.position.y < patrolPoints[currentPoint].position.y - 0.5f && rb.linearVelocity.y < 0.1f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

        } else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            waitCounter -= Time.deltaTime;
            if(waitCounter < 0)
            {
                waitCounter = waitAtPoints;

                currentPoint++;

                if(currentPoint >= patrolPoints.Length)
                {
                    currentPoint = 0;
                }
            }
        }

        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.PlayerInstance.IsHidden())
        {
            ActivateExecuteHalo();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PC.GetPlayerState() == PlayerController.PlayerState.Rage)
            {
                if (currentState != EnemyState.Feared)
                {
                    currentState = EnemyState.Feared;
                    Debug.Log("Runaway my man");
                }

            } 
            else if (currentState == EnemyState.Feared)
            {
                currentState = previousState;
                //TODO: Set a fear cooldown Timer, add a countdown
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PC.GetPlayerState() == PlayerController.PlayerState.Rage)
            {
                if (currentState == EnemyState.Feared)
                {
                    currentState = previousState;
                    Debug.Log("Runaway my man");
                }

            }
        }

        if (GameManager.Instance.PlayerInstance.IsHidden())
        {
            DeactivateExecuteHalo();
        }
    }

    public void ActivateHarvestHalo()
    {
        harvestHalo.enabled = true;
    }

    public void ActivateExecuteHalo()
    {
        harvestHalo.color = haloExecuteColour;
    }

    public void DeactivateExecuteHalo()
    {
        harvestHalo.color = haloDefaultColour;
    }
    private void OnDrawGizmos()
    {
        if(isReadyForHarvest)
        {
            Gizmos.color = debugHaloColour;
            Vector3 pos = transform.position + Vector3.up * debugHaloHeight;
            Gizmos.DrawSphere(pos, debugHaloSize);
        }
    }
}
