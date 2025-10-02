using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    public static AudioMixerManager instance;

    [SerializeField] private AudioMixer MainMixer;

    private const float minVolume = -80f;
    private const float maxVolume = 0f;
    private const float increment = 20f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadAndApplySettings();
    }

    private void LoadAndApplySettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 1.0f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat("soundFXVolume", 1.0f);

        UpdateMasterVolume(masterVolume);
        UpdateMusicVolume(musicVolume);
        UpdateSoundFXVolume(sfxVolume);
    }

    public void UpdateMasterVolume(float normalizedValue)
    {
        float dB = NormalizedToDecibel(normalizedValue);
        MainMixer.SetFloat("MasterVolume", dB); // <--- matches your exposed name
    }

    public void UpdateMusicVolume(float normalizedValue)
    {
        float dB = NormalizedToDecibel(normalizedValue);
        MainMixer.SetFloat("MusicVolume", dB); // <--- matches your exposed name
    }

    public void UpdateSoundFXVolume(float normalizedValue)
    {
        float dB = NormalizedToDecibel(normalizedValue);
        MainMixer.SetFloat("SoundFXVolume", dB); // <--- matches your exposed name
    }

    private float NormalizedToDecibel(float normalizedValue)
    {
        if (normalizedValue <= 0.0001f)
            return minVolume;

        return Mathf.Log10(normalizedValue) * increment;
    }

    private float DecibelToNormalized(float dB)
    {
        if (dB <= minVolume)
            return 0f;

        return Mathf.Pow(10, dB / increment);
    }

    public float GetMasterVolume()
    {
        if (MainMixer.GetFloat("MasterVolume", out float dB))
            return DecibelToNormalized(dB);
        return 1.0f;
    }

    public float GetMusicVolume()
    {
        if (MainMixer.GetFloat("MusicVolume", out float dB))
            return DecibelToNormalized(dB);
        return 1.0f;
    }

    public float GetSoundFXVolume()
    {
        if (MainMixer.GetFloat("SoundFXVolume", out float dB))
            return DecibelToNormalized(dB);
        return 1.0f;
    }
}
