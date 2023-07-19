using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] cameraControls mainCamera;

    [Header("Volume Setting")]
    [SerializeField] TMP_Text volumeTextValue;
    [SerializeField] Slider volumeSlider;
    [SerializeField] float defaultVolume = 0.5f;

    [Header("Gameplay Setting")]
    [SerializeField] TMP_Text sensitivityTextValue;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] int defaultSensitivity = 4;

    [Header("Invert Y Setting")]
    [SerializeField] Toggle invertY;

    //[Header("Confirmation")]
    //[SerializeField] private GameObject confirmationPrompt; 

    // Gets main camera
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraControls>();
    }

    // Sets the volume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    // Applied the selected values for volume and saves them to a playerpreferences called masterVolume
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        //StartCoroutine(ConfirmationPopup());
    }

    // Function works by not saving the modified changes (if apply wasn't clicked) and setting the values to the currently applied settings
    public void VolumeCancel()
    {
        defaultVolume = PlayerPrefs.GetFloat("masterVolume");
        volumeTextValue.text = defaultVolume.ToString("0.0");
        volumeSlider.value = defaultVolume;
        AudioListener.volume = defaultVolume;
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
        //StartCoroutine(ConfirmationPopup());
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
    }

    // Image pop up gives feedback to player that settings have been applied
    //public IEnumerator ConfirmationPopup()
    //{
    //    confirmationPrompt.SetActive(true);
    //    yield return new WaitForSeconds(2);
    //    confirmationPrompt.SetActive(false);
    //}

    // Default settings for Auido, Gameplay, and invertY
    public void DefaultSettings(string settingMenu)
    {
        if (settingMenu == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
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
    }
}
