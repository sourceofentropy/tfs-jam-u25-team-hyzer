using System.Collections;
using UnityEngine;

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

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float attackSpeed = 4f;
    public float waitTime = 2f;
    private float waitCounter;

    [Header("Detection Component")]
    public ConeDetection coneDetection;

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

        // handle direction change for sprite facing
        if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (rb.linearVelocity.x > 0)
        {
            transform.localScale = Vector3.one;
        }

        // -------- EXTRA: Force attack if player is inside cone --------
        if (coneDetection != null && currentState != EnemyState.Feared)
        {
            if (coneDetection.PlayerInCone(true) && player.CompareTag("Player"))
            {
                ChangeState(EnemyState.Attack);
            }
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

        if (coneDetection != null)
        {
            coneDetection.SetPatrolIndex(currentPatrolIndex);
            if (coneDetection.PlayerInCone(false))
            {
                ChangeState(EnemyState.Attack);
            }
        }
    }

    private void Attack()
    {
        if (coneDetection != null)
        {
            coneDetection.SetPatrolIndex(currentPatrolIndex);

            if (!coneDetection.PlayerInCone(true))
            {
                rb.linearVelocity = Vector2.zero;
                ChangeState(EnemyState.Wait);
                return;
            }
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

        if (coneDetection != null)
        {
            coneDetection.SetPatrolIndex(currentPatrolIndex);
            if (coneDetection.PlayerInCone(true))
            {
                ChangeState(EnemyState.Attack);
            }
        }
    }

    private void Feared()
    {
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Enemy is feared and cannot move/attack.");
    }

    // ----------- HELPERS -----------

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        rb.linearVelocity = Vector2.zero; // stop on state change
        Debug.Log($"<color=red>Enemy State Changed To: {newState}</color>");
    }

    public void SetFeared(bool feared)
    {
        if (feared)
            ChangeState(EnemyState.Feared);
        else
            ChangeState(EnemyState.Wait);
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}
