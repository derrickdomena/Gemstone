using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPreferences : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] bool isAvailable;
    [SerializeField] cameraControls mainCamera;
    [SerializeField] OptionsScript optionsMenuScript;

    [Header("Volume Setting")]
    [SerializeField] TMP_Text volumeTextValue;
    [SerializeField] Slider volumeSlider;

    [Header("Gameplay Setting")]
    [SerializeField] TMP_Text sensitivityTextValue;
    [SerializeField] Slider sensitivitySlider;

    [Header("Invert Y Setting")]
    [SerializeField] Toggle invertY;

    // Loads the previous saved settings if availble is true
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<cameraControls>();

        if (isAvailable)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");
                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
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
