using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class GameManager : MonoBehaviour
{

    static GameManager _instance;
    public static GameManager Instance => _instance;
        
    [HideInInspector] public PlayerController PlayerInstance => _playerInstance;
    public PlayerController _playerInstance = null;

    [HideInInspector] public ExecutionScore harvestScore;
    public TextMeshProUGUI harvestCounterHUD;

    public bool gamePaused = false;

    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private float lowPassFilterCuttoffFreq = 150f;
    private float defaultLowPassFilterCuttoffFreq;
    private readonly string cuttoffFieldName = "MusicLowPassCutoffFrequency";
    [SerializeField] private float lowPassFilterResonance = 2.5f;
    private float defaultLowPassFilterResonance;
    private readonly string resonanceFieldName = "MusicLowpassResonance";

    //Set Up Events - modify as needed
    public delegate void GameStart();
    public event GameStart GameStartedEvent;
    public delegate void GameOver();
    public event GameOver GameOverEvent;
    public delegate void GameWon();
    public event GameWon GameWonEvent;
    public delegate void GameQuit();
    public event GameQuit GameQuitEvent;
    public delegate void GameReturnToTitle();
    public event GameReturnToTitle GameReturnToTitleEvent;
    public delegate void PauseGame();
    public event PauseGame PauseGameEvent;
    public delegate void ResumeGame();
    public event ResumeGame ResumeGameEvent;   
    public delegate void LevelSceneLoaded();
    public event LevelSceneLoaded LevelSceneLoadedEvent;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.GameStartedEvent += OnGameStart;
        GameManager.Instance.GameQuitEvent += OnQuitGame;
        GameManager.Instance.GameOverEvent += OnGameOver;
        GameManager.Instance.GameWonEvent += OnGameWon;
        GameManager.Instance.PauseGameEvent += OnPauseGame; //TODO: should pause manager handle cursor logic? or game manager? or input manager? input manager is mostly for action based inputs...tbd
        GameManager.Instance.ResumeGameEvent += OnResumeGame; //TODO: should pause manager handle cursor logic? or game manager? or input manager? input manager is mostly for action based inputs...tbd
        GameManager.Instance.GameReturnToTitleEvent += OnReturnToTitle;
        GameManager.Instance.LevelSceneLoadedEvent += OnLevelSceneLoaded;

        harvestScore = gameObject.AddComponent<ExecutionScore>();

        //may need to move this when we have a restart feature
        /* will bork test scenes, uncomment when we properly load the player
        if (_playerInstance != null)
        {
            Destroy(_playerInstance.gameObject);
            _playerInstance = null;
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string name) //load initial scene, will need to update this when game supports saving and loading and needs to restore specific scenes
    {
        //UnityEngine.Debug.Log("build index of current scene " + SceneManager.GetActiveScene().name + " is " + SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(name);

    }

    public void OnUnitySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        if (GameManager.Instance.LevelSceneLoadedEvent != null)
        {
            GameManager.Instance.LevelSceneLoadedEvent();
        }
    }

    public void OnGameStart()
    {
        
    }

    public void OnQuitGame()
    {
        UnityEngine.Debug.Log("GM Quit Game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    public void OnGameOver()
    {
        UnityEngine.Debug.Log("game over - display game over screen");
        Time.timeScale = 0f;
        //TODO: re-enable when we have a menu, if we have a menu
        //EnableGameMenu();
    }

    public void OnGameWon()
    {
        UnityEngine.Debug.Log("game won - display game won screen");
        Time.timeScale = 0f;
        //TODO: re-enable when we have a menu, if we have a menu
        //EnableGameMenu();
    }

    public void OnPauseGame()
    {
        //TODO: re-enable when we have a menu, if we have a menu
        //EnablePauseMenu();
        Time.timeScale = 0f;
        MuffleAudio(true);
        gamePaused = true;
        UnlockCursor();
    }

    public void OnResumeGame()
    {
        //ToggleCursor();

        LockCursor();
        Time.timeScale = 1f;
        MuffleAudio(false);
        gamePaused = false;
        //TODO: re-enable when we have a menu, if we have a menu
        //DisablePauseMenu();
    }

    public void OnReturnToTitle()
    {
        //TODO: re-enable when we have a menu, if we have a menu
        //UnityEngine.Debug.Log("GameManager load title scene from level scene" + titleSceneName);
        //LoadScene(titleSceneName);         
    }

    public void OnLevelSceneLoaded()
    {

    }

    public void StartGame(int lives)
    {
        if (GameManager.Instance.GameStartedEvent != null)
        {
            //if (debugMode == DebugModes.Debug)
            UnityEngine.Debug.Log("Start game");

            GameManager.Instance.GameStartedEvent();
        }
    }

    public void GameOverSequence()
    {
        if (GameManager.Instance.GameOverEvent != null)
        {
            GameManager.Instance.GameOverEvent();
        }
    }

    public void GameQuitSequence()
    {
        if (GameManager.Instance.GameQuitEvent != null)
        {
            GameManager.Instance.GameQuitEvent();
        }
    }

    public void GameReturnToTitleSequence()
    {
        if (GameManager.Instance.GameReturnToTitleEvent != null)
        {
            GameManager.Instance.GameReturnToTitleEvent();
        }
    }

    //Initiate GameWon
    public void GameWonSequence()
    {
        if (GameManager.Instance.GameWonEvent != null)
        {
            GameManager.Instance.GameWonEvent();
        }
    }

    //Pause Game
    public void GamePaused()
    {
        if (GameManager.Instance.PauseGameEvent != null)
        {
            GameManager.Instance.PauseGameEvent();
        }
    }

    //Resume Game
    public void GameResumed()
    {
        if (GameManager.Instance.ResumeGameEvent != null)
        {
            GameManager.Instance.ResumeGameEvent();
        }
    }

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }

    }

    public void LockCursor()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void DisablePauseMenu()
    {
        //TODO: re-enable when we have a menu, if we have a menu
        //pauseMenu.gameObject.SetActive(false);
    }

    private void EnablePauseMenu()
    {
        //TODO: re-enable when we have a menu, if we have a menu
        //pauseMenu.gameObject.SetActive(true);
    }

    private void MuffleAudio(bool enable)
    {
        if (!enable)
        {
            masterMixer.SetFloat(cuttoffFieldName, defaultLowPassFilterCuttoffFreq);
            masterMixer.SetFloat(resonanceFieldName, defaultLowPassFilterResonance);
            return;
        }

        masterMixer.SetFloat(cuttoffFieldName, lowPassFilterCuttoffFreq);
        masterMixer.SetFloat(resonanceFieldName, lowPassFilterResonance);
    }
}
