using UnityEngine;

public class FearedState : MonoBehaviour
{
    [Header("Fear Modifiers")]
    public bool doLessDamage;
    public bool increaseDamage;
    public bool nothingModifier; // does nothing, left for completeness

    [Header("Fear Behaviours")]
    public bool runAwayFromPlayer;
    public bool attackSpeedBonus;
    public bool stunned;
    public bool nothingBehaviour; // does nothing, left for completeness

    [Header("Stats Modifiers")]
    private int currentDamage = 0;
    public int damageModifier = 1;
    public float attackSpeed = 1f;
    public float runSpeed = 3f;

    private bool isStunned = false;
    private Transform player;

    public DamagePlayer DamagePlayer;

    enum FearModifier {Weak, Emboldened, Stoic, None}

    enum FearBehaviour {Run, Survivor, Freeze, None}

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ApplyFearModifiers();
    }

    void Update()
    {
        HandleFearBehaviours();
    }

    void ApplyFearModifiers()
    {
        if (doLessDamage)
        {
            currentDamage = damageModifier;
        }
        if (increaseDamage)
        {
            currentDamage = -damageModifier; 
        }
        if (nothingModifier)
        {
            // Does nothing
        }
    }

    void HandleFearBehaviours()
    {
        if (runAwayFromPlayer && player != null && !stunned)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            transform.position += dir * runSpeed * Time.deltaTime;
        }

        if (attackSpeedBonus && !stunned)
        {
            attackSpeed = 1.5f; // Example boost
        }

        if (stunned)
        {
            if (!isStunned)
            {
                isStunned = true;
                currentDamage = 0;

                // Stop movement
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = Vector2.zero;
            }
        }


        if (nothingBehaviour)
        {
            // No behaviour change
        }
    }

    public int GetCurrentDamage()
    {
        return currentDamage;
    }
}
