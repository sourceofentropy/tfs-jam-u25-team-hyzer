using UnityEngine;


public class RageTest : MonoBehaviour
{
    public TestingPlayerController PC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider other)
    {
        Debug.Log("Do you work?");

        if (other.gameObject.CompareTag("Player"))
        {
            if (PC.GetPlayerState() == TestingPlayerController.PlayerState.Rage)
            {
                Debug.Log("Runaway my man");
            }
        }
    }


}
