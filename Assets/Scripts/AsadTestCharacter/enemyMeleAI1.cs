using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI2 : MonoBehaviour
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
    public float chaseSpeed = 4f; // separate chase speed from attackSpeed

    [Header("Attack Settings")]
    public int attackDamage = 10;      // custom damage per hit
    public float attackSpeed = 1f;     // attacks per second
    private float attackCooldownTimer = 0f;

    [Header("Detection Component")]
    public ConeDetection coneDetection;

    [Header("Debugging")]
    public EnemyState debugState;

    [Header("Fear Settings")]
    public FearedState fearedState;
    public DamagePlayer damagePlayer;

    [Header("Wait Settings")]
    public float waitTime = 2f;
    private float waitCounter;

    // reference to the player's state script
    private TestingPlayerController playerController;

    private void Start()
    {
        currentState = EnemyState.Patrol;
        currentPatrolIndex = 0;
        waitCounter = waitTime;

        if (player != null)
            playerController = player.GetComponent<TestingPlayerController>();
    }

    private void Update()
    {
        debugState = currentState;

        // 🔥 Rage check → Feared
        if (playerController != null)
        {
            if (playerController.currentState == TestingPlayerController.PlayerState.Rage)
            {
                SetFeared(true);
            }
            else if (currentState == EnemyState.Feared)
            {
                // if player leaves Rage, return to Wait
                SetFeared(false);
            }
        }

        // Handle attack cooldown timer
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Wait: Wait(); break;
            case EnemyState.Feared: Feared(); break;
        }

        // handle direction change for sprite facing
        if (rb.linearVelocity.x < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);
        else if (rb.linearVelocity.x > 0)
            transform.localScale = Vector3.one;

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
                ChangeState(EnemyState.Attack);
        }
    }

    private void Attack()
    {
        if (coneDetection != null)
        {
            coneDetection.SetPatrolIndex(currentPatrolIndex);

            // if player leaves cone, go back to wait
            if (!coneDetection.PlayerInCone(true))
            {
                rb.linearVelocity = Vector2.zero;
                ChangeState(EnemyState.Wait);
                return;
            }
        }

        // ✅ Chase player while in cone
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        // ✅ Attack only if player is in cone & cooldown ready
        if (attackCooldownTimer <= 0f && coneDetection.PlayerInCone(true))
        {
            if (player.CompareTag("Player"))
            {
                int finalDamage = attackDamage;

                // optional: stack with DamagePlayer
                if (damagePlayer != null)
                {
                    finalDamage += damagePlayer.damageAmount;

                    if (damagePlayer.FS != null)
                        finalDamage += damagePlayer.FS.GetCurrentDamage();

                    damagePlayer.DealDamage();
                }

                Debug.Log($"Enemy attacks player for <color=red>{finalDamage}</color> damage!");

                // reset cooldown based on attackSpeed (attacks/sec)
                attackCooldownTimer = 1f / attackSpeed;
            }
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
                ChangeState(EnemyState.Attack);
        }
    }

    private void Feared()
    {
        Debug.Log("Enemy is feared");

        if (fearedState.isActive)
            fearedState.ApplyFearModifiers();
        
    }

    // ----------- HELPERS -----------

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        if (currentState == EnemyState.Feared && fearedState != null)
            fearedState.enabled = false;

        currentState = newState;
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"<color=red>Enemy State Changed To: {newState}</color>");
    }

    public void SetFeared(bool feared)
    {
        if (feared)
            ChangeState(EnemyState.Feared);
        else
        {
            ChangeState(EnemyState.Wait);
            fearedState.ResetModdifiers();
        }
           
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}
