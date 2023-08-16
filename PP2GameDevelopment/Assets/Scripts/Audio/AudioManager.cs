using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip background;
    public AudioClip backgroundBoss;
    public AudioClip vendor;

    [Header("SFX Clips")]
    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip jumpingSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        BackgroundMusic();
    }

    //public void PlayMusic()
    //{
    //    if (gameManager.instance.activeMenu != gameManager.instance.shop)
    //    {
    //        musicSource.Stop();
    //        BackgroundMusic();
    //    }
    //    else
    //    {
    //        musicSource.Stop();
    //        musicSource.clip = vendor;
    //        musicSource.Play();
    //    }
        
    //}

    
    public void BackgroundMusic()
    {    
        if (SceneManager.GetActiveScene().buildIndex >= 0 && SceneManager.GetActiveScene().buildIndex <= 3)
        {
            musicSource.clip = background;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            musicSource.clip = backgroundBoss;
        }
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {      
        sfxSource.PlayOneShot(clip);
    }
}
