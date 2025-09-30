using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Attack, Wait, Feared }
    private EnemyState currentState;

    [Header("References")]
    public Transform[] patrolPoints;
    public Transform rightPatrolBarrier;
    public Transform leftPatrolBarrier;
    private int currentPatrolIndex;
    public Rigidbody2D rb;
    public Transform player;
    public TestingPlayerController playerController;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float attackSpeed = 4f;
    public float waitTime = 2f;
    private float waitCounter;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float detectionAngle = 45f; // half cone angle in degrees

    [Header("Fear Settings")]
    public CircleCollider2D fearCollider; // assign in Inspector


    [Header("Debugging")]
    public EnemyState debugState;

    private void Start()
    {
        currentState = EnemyState.Patrol;
        currentPatrolIndex = 0;
        waitCounter = waitTime;
    }

    private void Update()
    {
        debugState = currentState;

        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Wait: Wait(); break;
            case EnemyState.Feared: Feared(); break;
        }

       // rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

        //handle direction change
        if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (rb.linearVelocity.x > 0)
        {
            transform.localScale = Vector3.one;
        }
    }



    // ----------- STATES -----------

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            ChangeState(EnemyState.Wait);
        }

        if (PlayerInCone(false))
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void Attack()
    {
        // Stop attacking immediately if player leaves detection
        if (!PlayerInCone(true))
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Wait);
            return;
        }

        // Chase player directly
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * attackSpeed;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= 1f) // attack range
        {
            Debug.Log("Enemy attacks player!");
            // Hook into DamagePlayer here
        }
    }

    private void Wait()
    {
        rb.linearVelocity = Vector2.zero;
        waitCounter -= Time.deltaTime;

        if (waitCounter <= 0f)
        {
            waitCounter = waitTime;
            ChangeState(EnemyState.Patrol);
        }

        if (PlayerInCone(false))
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void Feared()
    {
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Enemy is feared and cannot move/attack.");
    }

    // ----------- HELPERS -----------

    /// <summary>
    /// If usePlayerForward = false, use patrol forward. 
    /// If true, face player so enemy keeps chasing.
    /// </summary>
    private bool PlayerInCone(bool usePlayerForward)
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        Vector2 forward = (usePlayerForward || patrolPoints.Length == 0)
                          ? toPlayer
                          : (patrolPoints[currentPatrolIndex].position - transform.position).normalized;

        // -----------------------
        // Ignore player if outside patrol boundaries AND moving further away
        // -----------------------
        if (player.position.x < leftPatrolBarrier.position.x && playerController.IsMovingLeft())
            return false;

        if (player.position.x > rightPatrolBarrier.position.x && playerController.IsMovingRight())
            return false;
        // -----------------------

        float distance = Vector2.Distance(transform.position, player.position);
        float angle = Vector2.Angle(forward, toPlayer);

        return distance <= detectionRadius && angle <= detectionAngle;
    }


    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        rb.linearVelocity = Vector2.zero; // stop on state change
        Debug.Log($"<color=red>Enemy State Changed To: {newState}</color>");
    }

    // ----------- FEAR COLLIDER HANDLING -----------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (fearCollider != null && other == fearCollider)
            return; // Prevent self-collision checks

        TestingPlayerController playerCtrl = other.GetComponent<TestingPlayerController>();
        if (playerCtrl != null && playerCtrl.currentState == TestingPlayerController.PlayerState.Rage)
        {
            SetFeared(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TestingPlayerController playerCtrl = other.GetComponent<TestingPlayerController>();
        if (playerCtrl != null && playerCtrl.currentState == TestingPlayerController.PlayerState.Rage)
        {
            SetFeared(false);
        }
    }

    public void SetFeared(bool feared)
    {
        if (feared)
            ChangeState(EnemyState.Feared);
        else
            ChangeState(EnemyState.Wait);
    }

    // ----------- DEBUG VISUALS -----------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 forward = Vector3.right;
        if (patrolPoints != null && patrolPoints.Length > 0)
            forward = (patrolPoints[currentPatrolIndex].position - transform.position).normalized;

        Quaternion leftRot = Quaternion.Euler(0, 0, detectionAngle);
        Quaternion rightRot = Quaternion.Euler(0, 0, -detectionAngle);

        Vector3 leftDir = leftRot * forward * detectionRadius;
        Vector3 rightDir = rightRot * forward * detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + leftDir);
        Gizmos.DrawLine(transform.position, transform.position + rightDir);
    }
}
