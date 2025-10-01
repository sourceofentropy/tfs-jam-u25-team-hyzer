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
    private bool playerExiting = false;
    private bool playerEntering = true;

    public Transform exitPoint; //old exit point from metroidvania tutorial
    public Transform playerSpawnPoint; //on entering this room player will spawn here
    public Transform playerStartPoint; //player will be moved to this location
    public float movePlayerSpeed;

    public string destinationLevel;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerHealthController.instance.GetComponent<PlayerController>();
        Debug.Log("does this play on scene start?");
        

        UIController.instance.StartFadeFromBlack();

        RespawnController.instance.SetSpawn(playerSpawnPoint.position);
        player.transform.position = playerSpawnPoint.position;

    }

    // Update is called once per frame
    void Update()
    {
        if(!playerEntering)
        {
            //this tutorial wants to call this on every frame? let's come back to this later and add an additional trigger for the door sensor
            if (Vector3.Distance(transform.position, player.transform.position) < distanceToOpen)
            {
                anim.SetBool("doorOpen", true);
            }
            else
            {
                anim.SetBool("doorOpen", false);
            }
        }

        if (playerEntering)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerStartPoint.position, movePlayerSpeed * Time.deltaTime);
            if (Vector2.Distance(player.transform.position, playerStartPoint.position) < 0.1) {
                playerEntering = false;
                player.canMove = true;
                player.anim.enabled = true;
            }
        }


        if (playerExiting)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, exitPoint.position, movePlayerSpeed * Time.deltaTime);
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(playerEntering)
        {
            return;
        }

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

        //RespawnController.instance.SetSpawn(exitPoint.position);
        //player.canMove = true;
        //player.anim.enabled = true;

        //UIController.instance.StartFadeFromBlack();

        SceneManager.LoadScene(destinationLevel);
    }

    
}
