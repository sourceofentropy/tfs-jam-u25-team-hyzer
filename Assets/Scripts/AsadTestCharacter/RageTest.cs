using UnityEngine;
using static TestingPlayerController;


public class RageTest : MonoBehaviour
{
    public TestingPlayerController PC;

    public enum EnemyState {Feared, Patrol, Wait, Attack}

    public EnemyState currentState = EnemyState.Wait;

    public EnemyState previousState = EnemyState.Wait;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PC.GetPlayerState() == TestingPlayerController.PlayerState.Rage)
            {
                if (currentState != EnemyState.Feared)
                {
                    currentState = EnemyState.Feared;
                    Debug.Log("Runaway my man");
                }
               
            }
            else if (currentState == EnemyState.Feared) 
            {
                currentState = previousState;
                //TODO: Set a fear cooldown Timer, add a countdown
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PC.GetPlayerState() == TestingPlayerController.PlayerState.Rage)
            {
                if (currentState == EnemyState.Feared)
                {
                    currentState = previousState;
                    Debug.Log("Runaway my man");
                }

            }
        }
    }

    



}
