using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Audio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip backgroundBoss;
    public AudioClip vendor;

    private void Start()
    {
        BackgroundMusic();
    }

    public void SwitchMusic()
    {
        BackgroundMusic();
    }

    void BackgroundMusic()
    {
        //if(gameManager.instance.activeMenu == gameManager.instance.shop)
        //{
        //    musicSource.clip = vendor;
        //}
        //else
        //{
        //    if (SceneManager.GetActiveScene().buildIndex >= 0 && SceneManager.GetActiveScene().buildIndex <= 2)
        //    {
        //        musicSource.clip = background;

        //    }
        //    else if (SceneManager.GetActiveScene().buildIndex == 3)
        //    {
        //        musicSource.clip = backgroundBoss;

        //    }
        //}
        musicSource.clip = background;
        musicSource.Play();
    }
}
