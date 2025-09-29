using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float moveSpeed;
    public float jumpForce;
    private float xInput;

    public Transform groundCheck;
    private bool isGrounded;
    private bool canDoubleJump; 

    public LayerMask groundLayer;
    private float groundCheckSize = 0.2f;

    public Animator anim;

    public BulletController shotToFire;
    public Transform shotOrigin;

    public float dashSpeed; //TODO: dash should be an ability added via builder pattern, so should jump and double jump and shoot TBH, maybe even base movement
    public float dashTime;
    private float dashCounter;
    public float waitAfterDashing;
    private float dashRechargeCounter;

    public SpriteRenderer sr;
    public SpriteRenderer afterImageSR;
    public float afterImageLifetime;
    public float timeBetweenAfterImages;
    private float afterImageCounter;
    public Color afterImageColor;

    public GameObject standing;
    public GameObject ball;
    public float waitToBall;
    private float ballCounter;
    public float ballThreshold = 0.9f;
    public Animator ballAnim;

    public Transform bombPoint;
    public GameObject bomb;

    private PlayerAbilityTracker abilities;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        abilities = GetComponent<PlayerAbilityTracker>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {

            if (dashRechargeCounter > 0)
            {
                dashRechargeCounter -= Time.deltaTime;
            }
            else
            {
                if (Input.GetButtonDown("Fire2") && standing.activeSelf && abilities.canDash)
                {
                    dashCounter = dashTime;
                    ShowAfterImage();
                }
            }


            if (dashCounter > 0)
            {
                dashCounter = dashCounter - Time.deltaTime;
                rb.linearVelocity = new Vector2(dashSpeed * transform.localScale.x, rb.linearVelocity.y);
                afterImageCounter -= Time.deltaTime;

                if (afterImageCounter <= 0)
                {
                    ShowAfterImage();
                }

                dashRechargeCounter = waitAfterDashing;
            }
            else
            {

                xInput = Input.GetAxisRaw("Horizontal");
                rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

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

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckSize, groundLayer);


            if (Input.GetButtonDown("Jump") && (isGrounded || (canDoubleJump && abilities.canDoubleJump)))
            {
                if (isGrounded)
                {
                    canDoubleJump = true;
                }
                else
                {
                    canDoubleJump = false;
                    anim.SetTrigger("doubleJump");
                }

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            //shooting
            if (Input.GetButtonDown("Fire1"))
            {
                if (standing.activeSelf)
                {
                    Instantiate(shotToFire, shotOrigin.position, shotOrigin.rotation).moveDir = new Vector2(transform.localScale.x, 0);
                    anim.SetTrigger("shotFired");
                }
                else if (ball.activeSelf && abilities.canDropBomb)
                {
                    Instantiate(bomb, bombPoint.position, bombPoint.rotation);
                }
            }

            //ball mode
            if (!ball.activeSelf)
            {
                if (Input.GetAxisRaw("Vertical") < -ballThreshold && abilities.canBecomeBall)
                {
                    ballCounter -= Time.deltaTime;
                    if (ballCounter <= 0)
                    {
                        ball.SetActive(true);
                        standing.SetActive(false);
                    }
                }
                else
                {
                    ballCounter = waitToBall;
                }
            }
            else
            {
                if (Input.GetAxisRaw("Vertical") > ballThreshold)
                {
                    ballCounter -= Time.deltaTime;
                    if (ballCounter <= 0)
                    {
                        ball.SetActive(false);
                        standing.SetActive(true);
                    }
                }
                else
                {
                    ballCounter = waitToBall;
                }
            }
        } else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (standing.activeSelf)
        {
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        }

        if(ball.activeSelf)
        {
            ballAnim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    public void ShowAfterImage()
    {
        SpriteRenderer image = Instantiate(afterImageSR, transform.position, transform.rotation);
        image.sprite = sr.sprite;
        image.transform.localScale = transform.localScale;
        image.color = afterImageColor;

        Destroy(image.gameObject, afterImageLifetime);
        afterImageCounter = timeBetweenAfterImages;
    }
}
