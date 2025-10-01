using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//currently running this on each slider - should manage this higher up, current setup is redundant
public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    //path to mixer Assets/Audio - keep paths in one settings file
    [SerializeField] private const float minVolume = -80f;
    [SerializeField] private const float maxVolume = 0f;

    //store this somewhere else and register these programatically when you can
    [SerializeField] private Slider masterSlider = null;
    [SerializeField] private Slider soundtrackSlider = null;
    [SerializeField] private Slider soundFXSlider = null;

    [SerializeField] private Toggle masterToggle = null;
    [SerializeField] private Toggle soundtrackToggle = null;
    [SerializeField] private Toggle soundFXtoggle = null;

    private float increment = 20;

    private readonly string masterChannelName = "Master";
    private readonly string soundtrackChannelName = "Soundtrack";
    private readonly string soundFXChannelName = "SoundFX";

    //default previous volume to min volume
    private float previousMasterVolume = minVolume;
    private float previousSoundtrackVolume = minVolume;
    private float previousSoundFXVolume = minVolume;

    public void OnEnable()
    {
        UnityEngine.Debug.Log("Fetch Mixer Reference");

        //masterMixer = Resources.Load<AudioMixer>("Assets\\Audio"); //will path matter for cross platform builds?
        /*
        if (masterMixer == null)
        {
            UnityEngine.Debug.LogError("Failed to load AudioMixer! Ensure it is in the Resources folder and the path is correct.");
        }
        else
        {
            UnityEngine.Debug.Log("AudioMixer loaded successfully: " + masterMixer.name);

            // Initialize volume sliders based on the mixer settings here
        }*/

        UnityEngine.Debug.Log("during enable master mixer volume is: " + masterMixer.GetFloat(AudioMixerGroups.Master, out float masterVolume) + " : " + masterVolume);
        //masterSlider = transform.Find("MasterAudio").GetComponent<Slider>();
        //soundtrackSlider = transform.Find("Music").GetComponent<Slider>();
        //soundFXSlider = transform.Find("SoundFX").GetComponent<Slider>();

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        soundtrackSlider.onValueChanged.AddListener(SetMusicVolume);
        soundFXSlider.onValueChanged.AddListener(SetSoundFXVolume);

        masterToggle.onValueChanged.AddListener(OnMasterToggleChanged);
        soundtrackToggle.onValueChanged.AddListener(OnSoundtrackToggleChanged);
        soundFXtoggle.onValueChanged.AddListener(OnSoundFXToggleChanged);

        //what about on the title menu?
        /*
        if(GameManager.Instance)
        {
            masterMixer = GameManager.Instance.masterMixer        }
        else
        {
            GameObject obj = FindObjectOfType<AudioMixerController>().gameObject;
            masterMixer = obj.GetComponent<AudioMixerController>();
        }
        */
        UnityEngine.Debug.Log("init volume sliders for mixer");
        InitVolumeControls();
        
    }

    public void SetMasterVolume(float sliderValue)
    {       
        masterMixer.SetFloat(AudioMixerGroups.Master, Mathf.Log10(sliderValue) * increment);
        SetToggle(masterToggle, sliderValue, masterSlider.minValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        masterMixer.SetFloat(AudioMixerGroups.Soundtrack, Mathf.Log10(sliderValue) * increment);
        SetToggle(soundtrackToggle, sliderValue, soundtrackSlider.minValue);
    }

    public void SetSoundFXVolume(float sliderValue)
    {
        masterMixer.SetFloat(AudioMixerGroups.SoundFX, Mathf.Log10(sliderValue) * increment);
        SetToggle(soundFXtoggle, sliderValue, soundFXSlider.minValue);
    }

    public void InitVolumeControls()
    {
        UnityEngine.Debug.Log("during init master mixer volume is: " + masterMixer.GetFloat(AudioMixerGroups.Master, out float masterVolume));
        if(masterMixer.GetFloat(AudioMixerGroups.Master, out float masterVolume2))
        {
            float normalizedtMasterVolume = NormalizeVolume(masterVolume);
            masterSlider.value = normalizedtMasterVolume;
            SetToggle(masterToggle, normalizedtMasterVolume, masterSlider.minValue);
        }

        if (masterMixer.GetFloat(AudioMixerGroups.Soundtrack, out float soundtrackVolume))
        {
            float normalizedSoundtrackVolume = NormalizeVolume(soundtrackVolume);
            soundtrackSlider.value = normalizedSoundtrackVolume;
            SetToggle(soundtrackToggle, normalizedSoundtrackVolume, soundtrackSlider.minValue);
        }

        if (masterMixer.GetFloat(AudioMixerGroups.SoundFX, out float soundFXVolume))
        {
            float normalizedFXVolume = NormalizeVolume(soundFXVolume);
            soundFXSlider.value = normalizedFXVolume;
            SetToggle(soundFXtoggle, normalizedFXVolume, soundFXSlider.minValue);
        }
    }

    private float NormalizeVolume(float dB)
    {
        if (dB <= minVolume) return 0f; // Clamps the minimum
        return (dB - minVolume) / (maxVolume - minVolume);
    }
    
    private float DenormalizeVolume(float normalizedValue)
    {
        return Mathf.Lerp(minVolume, maxVolume, normalizedValue);
    }
    
    private void SetToggle(Toggle toggle, float sliderValue, float minValue)
    {
        if (sliderValue <= minValue)
        {
            toggle.isOn = false;
        }
        else
        {
            toggle.isOn = true;
        }           
    }

    private void OnMasterToggleChanged(bool isOn)
    {
        masterToggle.isOn = isOn;
        
        if (!isOn)
        {
            previousMasterVolume = masterSlider.value;
            masterSlider.value = minVolume;
            return;
        }
       
        masterSlider.value = previousMasterVolume;
    }

    private void OnSoundtrackToggleChanged(bool isOn)
    {
        soundtrackToggle.isOn = isOn;

        if (!isOn)
        {
            previousSoundtrackVolume = soundtrackSlider.value;
            soundtrackSlider.value = minVolume;
            return;
        }

        soundtrackSlider.value = previousSoundtrackVolume; //should restore to previous volume but will have to store that somewhere
    }

    private void OnSoundFXToggleChanged(bool isOn)
    {
        soundFXtoggle.isOn = isOn;

        if (!isOn)
        {
            previousSoundFXVolume = soundFXSlider.value;
            soundFXSlider.value = minVolume;
            return;
        }

        soundFXSlider.value = previousSoundFXVolume; //should restore to previous volume but will have to store that somewhere
    }

    private void MuteChannel(string channelName)
    {
        masterMixer.SetFloat(channelName, minVolume);
    }

    private void UnMuteChannel(string channelName, float previousVolume)
    {
        masterMixer.SetFloat(channelName, previousVolume);
    }
}
