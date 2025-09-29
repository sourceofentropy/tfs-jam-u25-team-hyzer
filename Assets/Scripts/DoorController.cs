using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DoorController : MonoBehaviour
{
    public Animator anim;
    public float distanceToOpen;

    private PlayerController player;
    private string playerTag = "Player";
    private bool playerExiting;

    public Transform exitPoint;
    public float movePlayerSpeed;

    public string destinationLevel;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerHealthController.instance.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //this tutorial wants to call this on every frame? let's come back to this later and add an additional trigger for the door sensor
        if (Vector3.Distance(transform.position, player.transform.position) < distanceToOpen)
        {
            anim.SetBool("doorOpen", true);
        } else
        {
            anim.SetBool("doorOpen", false);
        }

        if(playerExiting)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, exitPoint.position, movePlayerSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("open the door");
        if(other.tag == playerTag)
        {

            if (!playerExiting)
            {
                Debug.Log("stop the player");
                player.canMove = false;
                StartCoroutine(UseDoorCo());
            }
        }
    }

    IEnumerator UseDoorCo()
    {
        playerExiting = true;
        player.anim.enabled = false;

        UIController.instance.StartFadeToBlack();

        yield return new WaitForSeconds(1.5f);

        RespawnController.instance.SetSpawn(exitPoint.position);
        player.canMove = true;
        player.anim.enabled = true;

        UIController.instance.StartFadeFromBlack();

        SceneManager.LoadScene(destinationLevel);
    }
}
