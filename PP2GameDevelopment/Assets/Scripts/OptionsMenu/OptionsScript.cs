using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] cameraControls mainCamera;
    [SerializeField] public AudioMixer audioMixer;

    [Header("Volume Setting")]
    [SerializeField] TMP_Text musicTextValue;
    [SerializeField] Slider musicSlider;
    [SerializeField] float defaultMusicVolume = 0.5f;
    [SerializeField] TMP_Text sfxTextValue;
    [SerializeField] Slider sfxSlider;
    [SerializeField] float defaultSFXVolume = 0.5f;

    [Header("Gameplay Setting")]
    [SerializeField] TMP_Text sensitivityTextValue;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] int defaultSensitivity = 4;

    [Header("Difficulty Setting")]

    [SerializeField] TMP_Text difficultyTextValue;
    [SerializeField] Slider difficultySlider;
    [SerializeField] int defaultDifficulty = 1;

    [Header("Invert Y Setting")]
    [SerializeField] Toggle invertY;

    // Gets main camera
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraControls>();      
    }

    private void Start()
    {
        SetMusicVolume(); // Calls SetMusicVolume to update the slider and slider text value
        SetSFXVolume(); // Calls SetSFXVolume to update the slider and slider text value
    }

    public void SetDifficulty()
    {
        defaultDifficulty = (int)difficultySlider.value;
        difficultyTextValue.text = difficultySlider.value.ToString();
    }
    // Sets the music volume
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
        musicTextValue.text = volume.ToString("0.0");
    }

    // Sets the sfx volume
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        sfxTextValue.text = volume.ToString("0.0");
    }

    // Applied the selected values for volume and saves them to a playerpreferences called masterVolume
    public void VolumeApply()
    {      
        PlayerPrefs.SetFloat("masterMusic", musicSlider.value);
        PlayerPrefs.SetFloat("masterSFX", sfxSlider.value);      
    }

    // Function works by not saving the modified changes (if apply wasn't clicked) and setting the values to the currently applied settings
    public void VolumeCancel()
    {
        defaultMusicVolume = PlayerPrefs.GetFloat("masterMusic");
        musicTextValue.text = defaultMusicVolume.ToString("0.0");
        musicSlider.value = defaultMusicVolume;
        audioMixer.SetFloat("Music", Mathf.Log10(defaultMusicVolume) * 20);

        defaultSFXVolume = PlayerPrefs.GetFloat("masterSFX");
        sfxTextValue.text = defaultSFXVolume.ToString("0.0");
        sfxSlider.value = defaultSFXVolume;
        audioMixer.SetFloat("SFX", Mathf.Log10(defaultSFXVolume) * 20);
    }

    // Sets the sensitivity
    public void SetSensitivity(float sensitivity)
    {
        defaultSensitivity = Mathf.RoundToInt(sensitivity);
        sensitivityTextValue.text = sensitivity.ToString("0");
    }

    // Applies gameplay changes
    public void GameplayApply()
    {
        if (invertY.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
            mainCamera.invertY = true;
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
            mainCamera.invertY = false;
        }

        PlayerPrefs.SetFloat("masterSensitivity", defaultSensitivity);
        mainCamera.sensitivity = defaultSensitivity * 100;

        PlayerPrefs.SetFloat("difficulty", defaultDifficulty);        
    }

    // Makes sure when gameplay UI values change and don't get apply sets the value to the previously saved settings
    public void GameplayCancel()
    {
        if (PlayerPrefs.GetInt("masterInvertY") == 0)
        {
            invertY.isOn = false;
        }
        else
        {
            invertY.isOn = true;
        }

        defaultSensitivity = (int)PlayerPrefs.GetFloat("masterSensitivity");
        sensitivityTextValue.text = defaultSensitivity.ToString("0");
        sensitivitySlider.value = defaultSensitivity;

        defaultDifficulty = PlayerPrefs.GetInt("difficulty");
        difficultyTextValue.text = difficultySlider.value.ToString("0.0");
        difficultySlider.value = defaultDifficulty;
    }

    // Default settings for Auido, Gameplay, and invertY
    public void DefaultSettings(string settingMenu)
    {
        if (settingMenu == "Audio")
        {
            audioMixer.SetFloat("Music", Mathf.Log10(defaultMusicVolume) * 20);
            musicSlider.value = defaultMusicVolume;
            musicTextValue.text = defaultMusicVolume.ToString("0.0");
            audioMixer.SetFloat("SFX", Mathf.Log10(defaultSFXVolume) * 20);
            sfxSlider.value = defaultSFXVolume;
            sfxTextValue.text = defaultSFXVolume.ToString("0.0");
            VolumeApply();
        }

        if (settingMenu == "Gameplay")
        {
            sensitivitySlider.value = defaultSensitivity;
            sensitivityTextValue.text = defaultSensitivity.ToString("0");
            mainCamera.sensitivity = defaultSensitivity * 100;

            GameplayApply();
        }

        if (settingMenu == "masterInvertY")
        {
            invertY.isOn = false;
            mainCamera.invertY = false;
            GameplayApply();
        }

        if (settingMenu == "difficulty")
        {
            difficultySlider.value = defaultDifficulty;
            difficultyTextValue.text = defaultDifficulty.ToString("0.0");
            GameplayApply();
        }
    }
}
