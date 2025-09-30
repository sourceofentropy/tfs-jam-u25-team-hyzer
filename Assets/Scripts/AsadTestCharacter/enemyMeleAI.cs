using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Attack, Wait, Feared }

    [Header("Debug")]
    [SerializeField] private EnemyState currentState; // visible in Inspector
    [SerializeField] private bool debugIsGrounded;    // shows ground check
    [SerializeField] private int debugPatrolIndex;
    [SerializeField] private float debugWaitTimer;
    [SerializeField] private float debugAttackTimer;

    [Header("References")]
    public Transform[] patrolPoints;
    public Transform player;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float patrolSpeed = 2f;
    public float detectionRadius = 5f;
    public float waitTime = 2f;

    [Header("Attack Settings")]
    public int damageAmount = 10;
    public float attackCooldown = 1.5f;
    private float attackTimer;

    [Header("Player Rage Check")]
    public TestingPlayerController playerController;

    [Header("Ground Check")]
    public Transform groundCheck;       // Empty at enemy's feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private int currentPatrolIndex = 0;
    private float waitTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = EnemyState.Patrol;
        waitTimer = waitTime;
        attackTimer = 0f;
    }

    void Update()
    {
        // --- Ground check ---
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- Update debug info ---
       
        debugIsGrounded = isGrounded;
        debugPatrolIndex = currentPatrolIndex;
        debugWaitTimer = waitTimer;
        debugAttackTimer = attackTimer;

        attackTimer -= Time.deltaTime;

        // --- Rage / Feared override ---
        if (playerController != null && playerController.currentState == TestingPlayerController.PlayerState.Rage)
        {
            ChangeState(EnemyState.Feared);
        }
        else if (currentState == EnemyState.Feared && playerController.currentState != TestingPlayerController.PlayerState.Rage)
        {
            if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
            {
                ChangeState(EnemyState.Attack);
                Debug.Log("we are attacking");
            }
            else
                ChangeState(EnemyState.Wait);
        }   else
        {
            if (currentState != EnemyState.Patrol)
            {
                ChangeState(EnemyState.Patrol);
            }
            
        }


            // --- State machine ---
            switch (currentState)
            {
                case EnemyState.Patrol: Patrol(); break;
                case EnemyState.Attack: Attack(); break;
                case EnemyState.Wait: Wait(); break;
                case EnemyState.Feared: Feared(); break;
            }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        rb.linearVelocity = Vector2.zero;

        if (newState == EnemyState.Wait)
            waitTimer = waitTime;

        if (newState == EnemyState.Attack)
            attackTimer = attackCooldown;
    }

    private void Patrol()
    {
        if (!isGrounded || patrolPoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

    }

    private void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        if (Vector2.Distance(transform.position, player.position) > detectionRadius)
        {
            ChangeState(EnemyState.Wait);
            return;
        }

        if (attackTimer <= 0f)
        {
            DamagePlayer dp = player.GetComponent<DamagePlayer>();
            if (dp != null)
            {
                dp.TakeDamage(damageAmount);
                Debug.Log("Enemy hit player for " + damageAmount);
            }
            attackTimer = attackCooldown;
        }
    }

    private void Wait()
    {
        rb.linearVelocity = Vector2.zero;
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
            ChangeState(EnemyState.Patrol);

        if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            Debug.Log("this is wait change");
            ChangeState(EnemyState.Attack);
        }
            
    }

    private void Feared()
    {
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Enemy is feared and cannot act!");
    }

    // --- Gizmos ---
    void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, 0.2f);
            }
        }

        // Ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
