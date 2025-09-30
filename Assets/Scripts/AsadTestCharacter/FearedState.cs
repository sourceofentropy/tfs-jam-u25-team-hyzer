using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class FearedState : MonoBehaviour
{
    public enum FearBehavior { Freeze, RunAway, HealthBoost }

    [Header("Which behaviors to enable")]
    public bool enableFreeze = true;
    public bool enableRunAway = false;
    public bool enableHealthBoost = false;

    [Header("RunAway settings")]
    public float runAwaySpeed = 5f;
    public float runAwayAcceleration = 20f;
    [Tooltip("If true, RunAway will try to move using Rigidbody2D velocity; otherwise it will attempt simple transform movement.")]
    public bool useRigidbodyForRun = true;

    [Header("HealthBoost settings")]
    [Tooltip("Amount of health to add once when fear starts (requires a Health component on same GameObject).")]
    public float healthBoostAmount = 10f;

    [Header("General")]
    [Tooltip("If true, applying fear repeatedly will reset the duration timer.")]
    public bool resetDurationIfReapplied = true;

    [Header("Debug / References")]
    public Transform playerTransform; // optionally set; if not set, must be provided on ApplyFear
    [Tooltip("Assign your enemy's AI script (optional). If provided, it will be disabled during Freeze.")]
    public MonoBehaviour enemyAIScript; // assign your EnemyAI (or any script to disable)
    public Rigidbody2D rb;

    [Header("Events")]
    public UnityEvent onFearStart;
    public UnityEvent onFearEnd;

    // runtime
    private bool isFeared = false;
    private float fearEndTime = 0f;
    private Coroutine fearCoroutine = null;
    private Vector2 fleeDirection = Vector2.zero;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isFeared) return;

        // If run away is enabled, handle movement each frame
        if (enableRunAway && playerTransform != null)
        {
            Vector2 dir = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
            fleeDirection = dir;

            if (useRigidbodyForRun && rb != null)
            {
                // Smoothly accelerate towards runAwaySpeed in flee direction
                Vector2 targetVel = dir * runAwaySpeed;
                rb.velocity = Vector2.MoveTowards(rb.velocity, targetVel, runAwayAcceleration * Time.deltaTime);
            }
            else
            {
                // Transform-based move (simple)
                transform.position += (Vector3)(dir * runAwaySpeed * Time.deltaTime);
            }
        }

        // optional auto clear (fallback) if something else messed up coroutine
        if (Time.time >= fearEndTime && fearEndTime > 0f)
        {
            ClearFear();
        }
    }

    /// <summary>
    /// Apply fear to this enemy.
    /// </summary>
    /// <param name="player">Reference to the player Transform (used for RunAway). Can be null if not needed or already set in inspector.</param>
    /// <param name="duration">How long the fear lasts in seconds.</param>
    public void ApplyFear(Transform player, float duration)
    {
        if (player != null) playerTransform = player;

        // If run away is requested but we have no player, we can't run away.
        if (enableRunAway && playerTransform == null)
        {
            Debug.LogWarning($"FearedState on {name}: RunAway enabled but playerTransform is null. RunAway will be skipped.");
            enableRunAway = false;
        }

        if (isFeared)
        {
            // either reset timer or ignore, depending on inspector toggle
            if (resetDurationIfReapplied)
            {
                fearEndTime = Time.time + duration;
                // restart coroutine to ensure timing consistent
                if (fearCoroutine != null) StopCoroutine(fearCoroutine);
                fearCoroutine = StartCoroutine(FearTimer(duration));
            }
            return;
        }

        // begin fear
        isFeared = true;
        fearEndTime = Time.time + duration;

        // Freeze: disable AI script / movement
        if (enableFreeze && enemyAIScript != null)
        {
            enemyAIScript.enabled = false;
        }

        // Health boost: apply immediately once (if Health component exists)
        if (enableHealthBoost)
        {
            var health = GetComponent<Health>(); // assumes you have a Health script with AddHealth method/field
            if (health != null)
            {
                // Try common method names safely
                var method = health.GetType().GetMethod("AddHealth");
                if (method != null)
                {
                    method.Invoke(health, new object[] { healthBoostAmount });
                }
                else
                {
                    // fallback: try property or field "currentHealth"
                    var field = health.GetType().GetField("currentHealth");
                    if (field != null)
                    {
                        float curr = (float)field.GetValue(health);
                        field.SetValue(health, curr + healthBoostAmount);
                    }
                    else
                    {
                        Debug.LogWarning($"FearedState on {name}: HealthBoost requested but can't find AddHealth() or currentHealth on Health component.");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"FearedState on {name}: HealthBoost requested but Health component not found.");
            }
        }

        // start timer coroutine
        fearCoroutine = StartCoroutine(FearTimer(duration));

        // Fire event
        onFearStart?.Invoke();
    }

    /// <summary>
    /// Ends fear immediately (or gets called automatically when duration expires).
    /// </summary>
    public void ClearFear()
    {
        if (!isFeared) return;

        isFeared = false;
        fearEndTime = 0f;

        // restore AI
        if (enableFreeze && enemyAIScript != null)
        {
            enemyAIScript.enabled = true;
        }

        // clear runAway velocity smoothing if used
        if (rb != null && useRigidbodyForRun)
        {
            // gently stop the run velocity (optional - you can change behavior)
            rb.velocity = Vector2.zero;
        }

        // stop coroutine
        if (fearCoroutine != null)
        {
            StopCoroutine(fearCoroutine);
            fearCoroutine = null;
        }

        onFearEnd?.Invoke();
    }

    private IEnumerator FearTimer(float duration)
    {
        float end = Time.time + duration;
        while (Time.time < end)
        {
            yield return null;
        }
        ClearFear();
    }

    // Optional debug drawing of flee direction
    void OnDrawGizmosSelected()
    {
        if (isFeared && enableRunAway && playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector3)transform.position + (Vector3)fleeDirection * 1.5f);
        }
    }
}
