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
}
