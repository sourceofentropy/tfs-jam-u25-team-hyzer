using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyingController : MonoBehaviour
{
    public float rangeToChase;
    private bool isChasing;

    public float moveSpeed, turnSpeed;

    private Transform player;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerHealthController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isChasing)
        {
            if(Vector3.Distance(transform.position, player.position) < rangeToChase)
            {
                isChasing = true;
                anim.SetBool("isChasing", isChasing);
            }
        } else
        {
            if(player.gameObject.activeSelf)
            {
                Vector3 direction = transform.position - player.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                //transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

                transform.position += -transform.right * moveSpeed * Time.deltaTime;
                /* can't use this without changing the rotation slerp above...
                 * 
                float xDistanceToPlayer = transform.position.x - player.position.x;
                if (xDistanceToPlayer > 0) {
                    transform.position += -transform.right * moveSpeed * Time.deltaTime;
                } else if (xDistanceToPlayer < 0)
                {
                    transform.position += transform.right * moveSpeed * Time.deltaTime;
                }*/
                
            }
        }
    }
}
