using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPreferences : MonoBehaviour
{
    [SerializeField] cameraControls mainCamera;

    [Header("Components")]
    [SerializeField] private bool isAvailable;
    [SerializeField] private mainMenu menuSettings;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue;
    [SerializeField] private Slider volumeSlider;

    [Header("Gameplay Setting")]
    [SerializeField] private TMP_Text sensitivityTextValue;
    [SerializeField] private Slider sensitivitySlider;

    [Header("Invert Y Setting")]
    [SerializeField] private Toggle invertY;

    private void Awake()
    {
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
                menuSettings.DefaultSettings("Audio");
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
                menuSettings.DefaultSettings("Gameplay");
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
                menuSettings.DefaultSettings("Gameplay");
            }
        }
    }
}
