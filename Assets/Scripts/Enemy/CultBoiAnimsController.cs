using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CultBoiAnimsController : MonoBehaviour
{
    private Animator animator; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttack()
    {
        animator.Play("CultistAttack");
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Target is either moving (1) or idle (0)
        float targetSpeed = inputVector.magnitude > 0 ? 1f : 0f;

        // Smoothly interpolate current speed toward target
        float currentSpeed = animator.GetFloat("Speed");
        float speed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        animator.SetFloat("Speed", speed);

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
    }
}
