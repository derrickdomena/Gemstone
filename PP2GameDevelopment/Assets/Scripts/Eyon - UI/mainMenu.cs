using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class mainMenu : MonoBehaviour
{
    [SerializeField] cameraControls mainCamera;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue;
    [SerializeField] private Slider volumeSlider;
    public float defaultVolume = 0.5f;

    [Header("Gameplay Setting")]
    [SerializeField] private TMP_Text sensitivityTextValue;
    [SerializeField] private Slider sensitivitySlider;
    public int defaultSensitivity = 4;

    [Header("Toggle Setting")]
    [SerializeField] private Toggle invertY; 

    //[Header("Confirmation")]
    //[SerializeField] private GameObject confirmationPrompt;

    //Load the game scene
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Quit the game
    public void Quit()
    {
        Application.Quit();
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

    public void SetSensitivity(float sensitivity)
    {
        defaultSensitivity = Mathf.RoundToInt(sensitivity);
        sensitivityTextValue.text = sensitivity.ToString("0");
    }

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
