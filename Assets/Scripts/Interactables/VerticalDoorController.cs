using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class VerticalDoorController : MonoBehaviour
{
    public Animator anim;

    private PlayerController player;
    private string playerTag = "Player";
    private bool playerExiting = false;
    private bool playerEntering = true;

    public Transform exitPoint; //old exit point from metroidvania tutorial
    public Transform playerSpawnPoint; //on entering this room player will spawn here
    public Transform playerStartPoint; //player will be moved to this location

    public string destinationLevel;
    public int doorID = 0;
    public int nextRoomDoorID;

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
        //if player interacts with door
        // - set playerEntering = true
        //call UseDoorCo and trigger any door animations

        if(!playerEntering)
        {
        }

        if (playerEntering && player.nextRoomDoorID == doorID)
        {
            //player.transform.position = Vector3.MoveTowards(player.transform.position, playerStartPoint.position, movePlayerSpeed * Time.deltaTime);
            //if (Vector2.Distance(player.transform.position, playerStartPoint.position) < 0.1)
            //{

            playerEntering = false;
            player.canMove = true;
            player.anim.enabled = true;

            //}
        }


        if (playerExiting)
        {
            player.nextRoomDoorID = nextRoomDoorID;            
            //player.transform.position = Vector3.MoveTowards(player.transform.position, exitPoint.position, movePlayerSpeed * Time.deltaTime);
        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //play door open anim if there is one
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //play door close anim if there is one
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //is this where interaction goes?
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
