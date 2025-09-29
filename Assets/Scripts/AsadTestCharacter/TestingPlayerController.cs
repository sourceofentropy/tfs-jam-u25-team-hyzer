using UnityEngine;

public class TestingPlayerController : MonoBehaviour
{
    public enum PlayerState { Regular, Disguise, Rage }
    public enum DisguiseMode { Normal, Ultra }

    [Header("Debug")]
    public PlayerState currentState = PlayerState.Regular;
    public DisguiseMode disguiseMode = DisguiseMode.Normal;

    [Header("Movement Settings")]
    public float regularSpeed = 5f;
    public float disguiseSpeed = 2.5f;
    public float rageSpeed = 8f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float rageJumpForce = 11f;

    [Header("Rage Settings")]
    public float rageDuration = 5f;
    private float rageTimer;
    public int maxRage = 3;
    public int rageBar = 0;

    [Header("References")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Enemy Detection")]
    public float enemyCheckRadius = 1.5f;
    public string enemyTag = "Enemy";

    private bool isGrounded;

    void Update()
    {
        HandleInput();
        HandleStates();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            float jump = (currentState == PlayerState.Rage) ? rageJumpForce : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
        }

        // Toggle disguise with Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentState == PlayerState.Regular) EnterDisguise();
            else if (currentState == PlayerState.Disguise) ExitDisguise();
        }

        // Rage fill in Regular mode by pressing E near enemy
        if (currentState == PlayerState.Regular && Input.GetKeyDown(KeyCode.E))
        {
            //if (IsNearEnemy())
          //  {
                rageBar = Mathf.Min(rageBar + 1, maxRage);
                Debug.Log("Rage Bar: " + rageBar + "/" + maxRage);
          //  }
        }

        // Toggle Ultra Disguise inside disguise
        if (currentState == PlayerState.Disguise && Input.GetKeyDown(KeyCode.E))
        {
            if (disguiseMode == DisguiseMode.Normal)
            {
                EnterUltraDisguise();
            }
            else
            {
                ExitUltraDisguise();
            }
        }

        // Activate rage when bar is full
        if (currentState == PlayerState.Regular && Input.GetKeyDown(KeyCode.T))
        {
            if (rageBar >= maxRage) EnterRage();
        }
    }

    void HandleMovement()
    {
        float move = Input.GetAxis("Horizontal");
        float speed = 0f;

        switch (currentState)
        {
            case PlayerState.Regular: speed = regularSpeed; break;
            case PlayerState.Disguise: speed = disguiseSpeed; break;
            case PlayerState.Rage: speed = rageSpeed; break;
        }

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Flip sprite
        if (move != 0) spriteRenderer.flipX = move < 0;
    }

    void HandleStates()
    {
        if (currentState == PlayerState.Rage)
        {
            rageTimer -= Time.deltaTime;
            if (rageTimer <= 0) ExitRage();
        }
    }

    bool IsNearEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, enemyCheckRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(enemyTag)) return true;
        }
        return false;
    }

    // --- Disguise Logic ---
    void EnterDisguise()
    {
        currentState = PlayerState.Disguise;
        disguiseMode = DisguiseMode.Normal;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }

    void ExitDisguise()
    {
        currentState = PlayerState.Regular;
        spriteRenderer.color = Color.white;
    }

    void EnterUltraDisguise()
    {
        disguiseMode = DisguiseMode.Ultra;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.25f);
    }

    void ExitUltraDisguise()
    {
        disguiseMode = DisguiseMode.Normal;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }

    // --- Rage Logic ---
    void EnterRage()
    {
        currentState = PlayerState.Rage;
        rageTimer = rageDuration;
        spriteRenderer.color = Color.red;
        rageBar = 0;
    }

    void ExitRage()
    {
        currentState = PlayerState.Regular;
        spriteRenderer.color = Color.white;
    }

    public PlayerState GetPlayerState()
    {
        return currentState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyCheckRadius);
    }
}
