using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;

    [Header("Settings UI Elements")]
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeTextValue;
    public Slider musicVolumeSlider;
    public TMP_Text musicVolumeTextValue;
    public Slider sfxVolumeSlider;
    public TMP_Text soundFXVolumeTextValue;

    [Header("MainMenu Buttons")]
    public Button playButton;
    public Button howToPlayButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("HowToPlay Buttons")]
    public Button howToPlayBackButton;

    [Header("Settings Buttons")]
    public Button applySettingsButton;
    public Button settingsBackButton;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    // Cache original values for reverting if user cancels
    private float originalMasterVolume;
    private float originalMusicVolume;
    private float originalSfxVolume;

    private Stack<GameObject> panelHistory = new Stack<GameObject>();

    void Start()
    {
        // Assign button actions
        playButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(() => OpenPanel(howToPlayPanel));
        settingsButton.onClick.AddListener(() => OpenPanel(settingsPanel));
        quitButton.onClick.AddListener(QuitGame);
        howToPlayBackButton.onClick.AddListener(BackToPreviousPanel);
        settingsBackButton.onClick.AddListener(BackToPreviousPanel);
        applySettingsButton.onClick.AddListener(ApplySettings);

        // Initialize audio settings
        InitializeVolumeSettings();

        // Add onValueChanged listeners for updating text values
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolumeText);
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolumeText);
        sfxVolumeSlider.onValueChanged.AddListener(UpdateSfxVolumeText);

        // Initial text updates
        UpdateMasterVolumeText(masterVolumeSlider.value);
        UpdateMusicVolumeText(musicVolumeSlider.value);
        UpdateSfxVolumeText(sfxVolumeSlider.value);
    }

    void InitializeVolumeSettings()
    {
        // Load saved settings
        float savedMaster = PlayerPrefs.GetFloat("masterVolume", 1.0f);
        float savedMusic = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        float savedSFX = PlayerPrefs.GetFloat("soundFXVolume", 1.0f);

        // Cache original values
        originalMasterVolume = savedMaster;
        originalMusicVolume = savedMusic;
        originalSfxVolume = savedSFX;

        // Set slider values
        masterVolumeSlider.value = savedMaster;
        musicVolumeSlider.value = savedMusic;
        sfxVolumeSlider.value = savedSFX;

        // Immediately apply to AudioMixer so audio matches sliders
        if (AudioMixerManager.instance != null)
        {
            AudioMixerManager.instance.UpdateMasterVolume(savedMaster);
            AudioMixerManager.instance.UpdateMusicVolume(savedMusic);
            AudioMixerManager.instance.UpdateSoundFXVolume(savedSFX);
        }

        // Add preview listeners for real-time adjustments
        masterVolumeSlider.onValueChanged.AddListener(PreviewMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(PreviewMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(PreviewSfxVolume);

        // Add text update listeners
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolumeText);
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolumeText);
        sfxVolumeSlider.onValueChanged.AddListener(UpdateSfxVolumeText);

        // Update text immediately
        UpdateMasterVolumeText(savedMaster);
        UpdateMusicVolumeText(savedMusic);
        UpdateSfxVolumeText(savedSFX);
    }

    // Preview functions - updates mixer but doesn't save to PlayerPrefs
    void PreviewMasterVolume(float volume)
    {
        AudioMixerManager.instance.UpdateMasterVolume(volume);
    }

    void PreviewMusicVolume(float volume)
    {
        AudioMixerManager.instance.UpdateMusicVolume(volume);
    }

    void PreviewSfxVolume(float volume)
    {
        AudioMixerManager.instance.UpdateSoundFXVolume(volume);
    }

    // Text update functions
    void UpdateMasterVolumeText(float value)
    {
        masterVolumeTextValue.text = Mathf.RoundToInt(value * 100).ToString() + "%";
    }

    void UpdateMusicVolumeText(float value)
    {
        musicVolumeTextValue.text = Mathf.RoundToInt(value * 100).ToString() + "%";
    }

    void UpdateSfxVolumeText(float value)
    {
        soundFXVolumeTextValue.text = Mathf.RoundToInt(value * 100).ToString() + "%";
    }

    public void ApplySettings()
    {
        // Save the current slider values to PlayerPrefs
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("soundFXVolume", sfxVolumeSlider.value);
        PlayerPrefs.Save();

        // Update cached original values
        originalMasterVolume = masterVolumeSlider.value;
        originalMusicVolume = musicVolumeSlider.value;
        originalSfxVolume = sfxVolumeSlider.value;

        Debug.Log("Settings applied and saved");

        // Return to previous panel
        BackToPreviousPanel();
    }

    // Cancel settings and revert to original values
    public void CancelSettings()
    {
        // Revert slider UI
        masterVolumeSlider.value = originalMasterVolume;
        musicVolumeSlider.value = originalMusicVolume;
        sfxVolumeSlider.value = originalSfxVolume;

        // Revert mixer values
        AudioMixerManager.instance.UpdateMasterVolume(originalMasterVolume);
        AudioMixerManager.instance.UpdateMusicVolume(originalMusicVolume);
        AudioMixerManager.instance.UpdateSoundFXVolume(originalSfxVolume);

        // Go back to previous panel
        BackToPreviousPanel();
    }

    public void StartGame()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
        else
            Debug.LogError("Game scene name not set in MenuManager!");
    }

    public void OpenPanel(GameObject panelToOpen)
    {
        if (panelToOpen.activeSelf) return;

        // Special case for settings panel
        if (panelToOpen == settingsPanel)
        {
            // Cache current saved values when entering settings
            originalMasterVolume = PlayerPrefs.GetFloat("masterVolume", 1.0f);
            originalMusicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
            originalSfxVolume = PlayerPrefs.GetFloat("soundFXVolume", 1.0f);

            // Update UI to match saved values
            masterVolumeSlider.value = originalMasterVolume;
            musicVolumeSlider.value = originalMusicVolume;
            sfxVolumeSlider.value = originalSfxVolume;
        }

        if (panelHistory.Count == 0 || panelHistory.Peek() != panelToOpen)
        {
            panelHistory.Push(GetActivePanel());
        }

        GetActivePanel()?.SetActive(false);
        panelToOpen.SetActive(true);
    }

    public void BackToPreviousPanel()
    {
        if (panelHistory.Count > 0)
        {
            GameObject previousPanel = panelHistory.Pop();
            GetActivePanel()?.SetActive(false);
            previousPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game called");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private GameObject GetActivePanel()
    {
        if (mainMenuPanel.activeSelf) return mainMenuPanel;
        if (howToPlayPanel.activeSelf) return howToPlayPanel;
        if (settingsPanel.activeSelf) return settingsPanel;
        return null;
    }
}