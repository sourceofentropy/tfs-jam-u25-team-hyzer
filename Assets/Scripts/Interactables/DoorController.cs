//JONATHAN'S LOGIC
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;


//public class DoorController : MonoBehaviour
//{
//    public Animator anim;
//    public float distanceToOpen;

//    private PlayerController player;
//    private string playerTag = "Player";
//    private bool playerExiting = false;
//    private bool playerEntering = true;

//    public Transform exitPoint; //old exit point from metroidvania tutorial
//    public Transform playerSpawnPoint; //on entering this room player will spawn here
//    public Transform playerStartPoint; //player will be moved to this location
//    public float movePlayerSpeed;

//    public string destinationLevel;

//    public int doorID = 0;
//    public int nextRoomDoorID;

//    public enum DoorType
//    {
//        Horizontal,
//        Vertical
//    }
//    public DoorType doorType = DoorType.Horizontal;

//    public bool activated = false;

//    // Start is called before the first frame update
//    void Start()
//    {

//        player = PlayerHealthController.instance.GetComponent<PlayerController>();
//        Debug.Log("does this play on scene start?");

//        //if((player.nextDoorPosition == doorPosition))
//        UIController.instance.StartFadeFromBlack();

//        RespawnController.instance.SetSpawn(playerSpawnPoint.position);
//        player.transform.position = playerSpawnPoint.position;

//        if (player.nextRoomDoorID == doorID)
//        {
//            activated = true;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//    }

//    public void HandleHorizontalDoor()
//    {

//        if (!playerEntering && player.nextRoomDoorID == doorID && activated)
//        {
//            //this tutorial wants to call this on every frame? let's come back to this later and add an additional trigger for the door sensor
//            if (Vector3.Distance(transform.position, player.transform.position) < distanceToOpen)
//            {
//                anim.SetBool("doorOpen", true);
//            }
//            else
//            {
//                anim.SetBool("doorOpen", false);
//            }
//        }

//        if (playerEntering && player.nextRoomDoorID == doorID && activated)
//        {
//            player.transform.position = Vector3.MoveTowards(player.transform.position, playerStartPoint.position, movePlayerSpeed * Time.deltaTime);
//            if (Vector2.Distance(player.transform.position, playerStartPoint.position) < 0.1)
//            {
//                playerEntering = false;
//                player.canMove = true;
//                player.anim.enabled = true;
//            }
//        }


//        if (playerExiting)
//        {
//            player.nextRoomDoorID = nextRoomDoorID;
//            activated = false;
//            player.transform.position = Vector3.MoveTowards(player.transform.position, exitPoint.position, movePlayerSpeed * Time.deltaTime);

//        }

//    }

//    public void HandleVerticalDoor()
//    {

//    }
//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if(playerEntering)
//        {
//            return;
//        }

//        Debug.Log("open the door");
//        if(other.tag == playerTag)
//        {

//            if (!playerExiting)
//            {
//                Debug.Log("stop the player");
//                player.canMove = false;
//                StartCoroutine(UseDoorCo());
//            }
//        }
//    }

//    IEnumerator UseDoorCo()
//    {
//        playerExiting = true;
//        player.anim.enabled = false;

//        UIController.instance.StartFadeToBlack();

//        yield return new WaitForSeconds(1.5f);

//        //RespawnController.instance.SetSpawn(exitPoint.position);
//        //player.canMove = true;
//        //player.anim.enabled = true;

//        //UIController.instance.StartFadeFromBlack();

//        SceneManager.LoadScene(destinationLevel);
//    }


//}
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

    public Transform exitPoint; // Where player walks to when exiting through this door
    public Transform playerSpawnPoint; // Where player spawns when entering through this door
    public Transform playerStartPoint; // Where player stops walking after spawning
    public float movePlayerSpeed;
    public string destinationLevel;

    [Header("Door Identification")]
    public int doorID = 0; // THIS door's unique ID in the current scene
    public int nextRoomDoorID; // The door ID in the NEXT room that connects back to this door

    public enum DoorType
    {
        Horizontal,
        Vertical
    }
    public DoorType doorType = DoorType.Horizontal;

    void Start()
    {
        player = PlayerHealthController.instance.GetComponent<PlayerController>();

        // Only handle this door if the player came through it
        if (player.nextRoomDoorID == doorID)
        {
            Debug.Log($"Player entered through door {doorID}");
            UIController.instance.StartFadeFromBlack();
            RespawnController.instance.SetSpawn(playerSpawnPoint.position);
            player.transform.position = playerSpawnPoint.position;

            // Flip player sprite based on door direction
            FlipPlayerBasedOnDoor();

            // Lock player movement during entry animation
            player.canMove = false;
            player.anim.enabled = false;
            playerEntering = true;
        }
        else
        {
            // Player didn't come through this door, so it's inactive
            playerEntering = false;
        }
    }

    void FlipPlayerBasedOnDoor()
    {
        if (doorType == DoorType.Horizontal)
        {
            // Determine direction based on spawn point relative to start point
            float direction = playerStartPoint.position.x - playerSpawnPoint.position.x;

            if (direction > 0)
            {
                // Player needs to walk right (door is on the left)
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (direction < 0)
            {
                // Player needs to walk left (door is on the right)
                player.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        else if (doorType == DoorType.Vertical)
        {
            // For vertical doors, you might want to always face a specific direction
            // Or determine based on your game's needs
            float direction = playerStartPoint.position.y - playerSpawnPoint.position.y;

            // Example: always face right when entering from vertical doors
            player.transform.localScale = new Vector3(1f, 1f, 1f);

            // Or you could base it on horizontal offset if your vertical doors aren't centered
        }
    }

    void Update()
    {
        if (doorType == DoorType.Horizontal)
        {
            HandleHorizontalDoor();
        }
        else if (doorType == DoorType.Vertical)
        {
            HandleVerticalDoor();
        }
    }

    public void HandleHorizontalDoor()
    {
        // Handle door opening animation when player is nearby
        if (!playerEntering)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < distanceToOpen)
            {
                anim.SetBool("doorOpen", true);
            }
            else
            {
                anim.SetBool("doorOpen", false);
            }
        }

        // Handle player entering animation (walking from spawn to start point)
        if (playerEntering && player.nextRoomDoorID == doorID)
        {
            anim.SetBool("doorOpen", true); // Keep door open during entry

            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                playerStartPoint.position,
                movePlayerSpeed * Time.deltaTime
            );

            if (Vector2.Distance(player.transform.position, playerStartPoint.position) < 0.1f)
            {
                playerEntering = false;
                player.canMove = true;
                player.anim.enabled = true;
            }
        }

        // Handle player exiting animation (walking to exit point)
        if (playerExiting)
        {
            anim.SetBool("doorOpen", true); // Keep door open during exit

            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                exitPoint.position,
                movePlayerSpeed * Time.deltaTime
            );
        }
    }

    public void HandleVerticalDoor()
    {
        // Similar logic for vertical doors (ladders, hatches, etc.)
        // Implement as needed
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prevent re-triggering during entry animation
        if (playerEntering)
        {
            return;
        }

        if (other.tag == playerTag && !playerExiting)
        {
            Debug.Log($"Player triggering door {doorID} to {destinationLevel}");
            player.canMove = false;
            StartCoroutine(UseDoorCo());
        }
    }

    IEnumerator UseDoorCo()
    {
        playerExiting = true;
        player.anim.enabled = false;

        // Set which door to spawn at in the next room
        player.nextRoomDoorID = nextRoomDoorID;

        UIController.instance.StartFadeToBlack();
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(destinationLevel);
    }
}