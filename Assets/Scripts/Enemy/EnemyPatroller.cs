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

        foreach(Transform patrolPoint in patrolPoints)
        {
            patrolPoint.SetParent(null);
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
    }

}
