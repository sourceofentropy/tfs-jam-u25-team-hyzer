using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour //Sound FX manager
{
    static SoundManager _instance;
    public static SoundManager Instance => _instance;
    
    [SerializeField] private AudioClip playerGainedLife;
    [SerializeField] private AudioClip playerHit;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip gameWon;
    [SerializeField] private AudioClip gamePaused;
    [SerializeField] private AudioClip coinCollected;
    [SerializeField] private AudioClip hammerCollected;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip powerUp;
    [SerializeField] private AudioSource audioSource;

    private void Awake() //TODO: contents of awake could be an abstract manager function, if unity plays nice with that idea for the game manager as well
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject); 
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("#lab9 SoundManager Start");

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on Sound Manager");
        }

        if (GameManager.Instance != null)
        {
            //GameManager.Instance.PlayerGainedLifeEvent += OnPlayerGainedLife;
            //GameManager.Instance.PlayerHitEvent += OnPlayerHit;
            GameManager.Instance.PauseGameEvent += OnGamePaused;
            GameManager.Instance.GameOverEvent += OnGameOver;
            GameManager.Instance.GameWonEvent += OnGameWon;
            //GameManager.Instance.CoinCollectedEvent += OnCoinCollected;
            //GameManager.Instance.PowerUpCollectedEvent += OnPowerUpCollectedEvent;
        }
        else
        {
            Debug.LogError("GameManager instance is null");
        }
    }
    private void OnGamePaused()
    {
        audioSource.PlayOneShot(gamePaused);
    }

    private void OnPlayerHit(int damage)
    {
        //TODO: play different audio clip based on level of damage received
        audioSource.PlayOneShot(playerHit);
    }

    private void OnGameOver()
    {
        //play sound
        audioSource.PlayOneShot(gameOver);
    }
    private void OnPlayerGainedLife(int lives)
    {
        //play sound
        audioSource.PlayOneShot(playerGainedLife);
    }

    private void OnCoinCollected()
    {
        Debug.Log("someone got a coin, play coin collected sound");
        audioSource.PlayOneShot(coinCollected);
    }

    private void OnPowerUpCollectedEvent()
    {
        Debug.Log("Generic powerup sound - get more specific in the future");
        audioSource.PlayOneShot(powerUp);
    }

    private void OnGameWon()
    {
        audioSource.PlayOneShot(gameWon);
    }
}
