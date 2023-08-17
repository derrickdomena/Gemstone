using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;

    [Header("Music Clips")] // Will
    public AudioClip background;
    public AudioClip backgroundBoss;
    public AudioClip vendor;

    [Header("SFX Clips")]
    [Header("Player Movement")]
    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip jumpingSound;

    [Header("Enemies")]
    public AudioClip enemyWalkSound; 
    public AudioClip enemySpawnSound;
    public AudioClip enemySwingSound;
    public AudioClip enemyProjectileSound;

    [Header("Menu")]
    public AudioClip hoverSound;
    public AudioClip applySound;
    public AudioClip cancelSound;
    public AudioClip pauseSound;
    public AudioClip unpauseSound;

    [Header("Abilities")]
    public AudioClip explosionSound;
    public AudioClip fireballSound;
    public AudioClip dashSound;
    public AudioClip stasisSound;

    [Header("Melee")]
    public AudioClip swingSound;
    public AudioClip hitSound;

    [Header("Gun")]
    public AudioClip autoSound;
    public AudioClip semiSound;
    public AudioClip rifleSound;

    [Header("Interact Sounds")] // Josh
    public AudioClip enterDungeonSound;
    public AudioClip bobInteractSound;
    public AudioClip pickUpSound;

    private void Start()
    {
        BackgroundMusic();
    }
    
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
