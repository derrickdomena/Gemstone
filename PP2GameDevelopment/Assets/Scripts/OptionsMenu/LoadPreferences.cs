using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class LoadPreferences : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] bool isAvailable;
    [SerializeField] cameraControls mainCamera;
    [SerializeField] OptionsScript optionsMenuScript;

    [Header("Volume Setting")]
    [SerializeField] TMP_Text musicTextValue;
    [SerializeField] Slider musicSlider;
    [SerializeField] float playerPrefsMusic;

    [SerializeField] TMP_Text sfxTextValue;
    [SerializeField] Slider sfxSlider;
    [SerializeField] float playerPrefsSFX;

    [Header("Gameplay Setting")]
    [SerializeField] TMP_Text sensitivityTextValue;
    [SerializeField] Slider sensitivitySlider;

    [Header("Invert Y Setting")]
    [SerializeField] Toggle invertY;

    // Loads the previous saved settings if availble is true
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraControls>();
        playerPrefsMusic = PlayerPrefs.GetFloat("masterMusic");
        playerPrefsSFX = PlayerPrefs.GetFloat("masterSFX");

        if (isAvailable)
        {
            if (PlayerPrefs.HasKey("masterMusic") && PlayerPrefs.HasKey("masterSFX"))
            {
                float localMusic = PlayerPrefs.GetFloat("masterMusic");              
                musicTextValue.text = localMusic.ToString("0.0");             
                musicSlider.value = localMusic;
                optionsMenuScript.audioMixer.SetFloat("Music", Mathf.Log10(localMusic) * 20);

                float localSFX = PlayerPrefs.GetFloat("masterSFX");
                sfxTextValue.text = localSFX.ToString("0.0");
                sfxSlider.value = localSFX;
                optionsMenuScript.audioMixer.SetFloat("SFX", Mathf.Log10(localSFX) * 20);
            }
            else
            {
                optionsMenuScript.DefaultSettings("Audio");
            }

            if (PlayerPrefs.HasKey("masterSensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
                sensitivityTextValue.text = localSensitivity.ToString("0");
                sensitivitySlider.value = localSensitivity;
                mainCamera.sensitivity = (int)localSensitivity * 100;
            }
            else
            {
                optionsMenuScript.DefaultSettings("Gameplay");
            }

            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                if (PlayerPrefs.GetInt("masterInvertY") == 0)
                {
                    invertY.isOn = false;   
                    mainCamera.invertY = false;
                }
                else
                {
                    invertY.isOn = true;
                    mainCamera.invertY = true;
                }
            }
            else
            {
                optionsMenuScript.DefaultSettings("Gameplay");
            }
        }
    }
}
