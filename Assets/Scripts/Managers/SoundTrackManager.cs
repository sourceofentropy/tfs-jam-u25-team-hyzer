using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static GameManager;

[RequireComponent(typeof(AudioSource))]
public class SoundTrackManager : MonoBehaviour
{
    static SoundTrackManager _instance;
    public static SoundTrackManager Instance => _instance;

    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private AudioClip gameWonMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioSource musicSource;

    private AudioSource audioSource;

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
                
        if (musicSource == null)
        {
            UnityEngine.Debug.LogError("No AudioSource component found on SoundTrack Manager");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGameEvent += OnGamePaused;
            GameManager.Instance.PauseGameEvent += OnGameResumed;
            GameManager.Instance.GameStartedEvent += OnGameStarted; //not firing? not able to fire?
            GameManager.Instance.LevelSceneLoadedEvent += OnGameStarted; //workaround in lieu of gamestarted event not firing
            GameManager.Instance.GameOverEvent += OnGameOver;
            GameManager.Instance.GameWonEvent += OnGameWon;
            //GameManager.Instance.LevelStartEvent += OnLevelStarted;
            //GameManager.Instance.LevelCompleteEvent += OnLevelComplete; - uncomment when there is a post level score or additional levels to complete before the game won condition is triggered
        }
        else
        {
            UnityEngine.Debug.LogError("GameManager instance is null");
        }
    }
    private void OnGamePaused()
    {
        UnityEngine.Debug.Log("Pause game music");
        musicSource.Pause();
    }

    private void OnGameResumed()
    {
        musicSource.UnPause();
    }

    private void OnGameOver()
    {
        //play sound
        musicSource.clip = gameOverMusic;
        musicSource.Play();
    }

    private void OnGameWon()
    {
        musicSource.clip = gameWonMusic;
        musicSource.Play();
    }

    private void OnGameStarted()
    {
        UnityEngine.Debug.Log("OnGameStarted: start level music, game started");
        musicSource.clip = levelMusic;
        musicSource.Play();
    }

    //use when we have multiple levels?
    private void OnLevelStarted()
    {
        //TODO: start level music
        UnityEngine.Debug.Log("OnLevelStarted start level music, game started");
        musicSource.clip = levelMusic;
        musicSource.Play();
    }

    private void OnLevelComplete()
    {
        //TODO: stop level music
        musicSource.clip = gameWonMusic;
        musicSource.Play();
    }
    
    public void PlayTitleMusic()
    {
        musicSource.clip = titleMusic;
        musicSource.Play();
    }

    public void StopTitleMusic()
    {
        musicSource.Stop();
    }
}
