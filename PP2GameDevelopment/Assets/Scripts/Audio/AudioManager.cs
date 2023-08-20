using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource; // Music Only
    [SerializeField] public AudioSource sfxSource; // All SFX
    [SerializeField] public AudioSource sfxPlayer; // Player SFX Only
    [SerializeField] public AudioSource sfxEnemy; // Enemy SFX Only
    [SerializeField] public AudioSource sfxMenu; // Menu SFX Only
    [SerializeField] public AudioSource sfxAbilities; // Abilities SFX Only
    [SerializeField] public AudioSource sfxMelee; // Melee SFX Only
    [SerializeField] public AudioSource sfxGun; // Gun SFX Only
    [SerializeField] public AudioSource sfxInteractSounds; // Interact Sounds SFX Only

    [Header("Music Clips")] // Will
    public AudioClip background;
    public AudioClip backgroundBoss;
    public AudioClip vendor;

    [Header("SFX Clips")]
    [Header("Player Movement")]
    public AudioClip walkingSound;
    public AudioClip landingSound;

    [Header("Enemies")]
    [Header("General")]
    public AudioClip enemyWalkSound;
    public AudioClip enemySpawnSound;
    public AudioClip enemyMeleeSound;
    public AudioClip enemyProjectileSound;
    public AudioClip enemyDeathSound;

    [Header("Spider Minions")]
    public AudioClip spiderWalkSound;
    public AudioClip spiderAttackSound;
    public AudioClip spiderProjectileSound;
    public AudioClip spiderDeathSound;
    public AudioClip spiderHissSound;

    [Header("Boss Spider")]
    public AudioClip spiderBossSlam;
    public AudioClip spiderBossProjectile;
    public AudioClip spiderBossMelee;
    public AudioClip spiderBossWalk;
    public AudioClip spiderBossDeath;
    public AudioClip spiderBossHiss;

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
        if (SceneManager.GetActiveScene().buildIndex >= 0 && SceneManager.GetActiveScene().buildIndex <= 4)
        {
            musicSource.clip = background;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            musicSource.clip = backgroundBoss;
        }
        musicSource.Play();
    }

    // Player - Movement
    public void PlaySFXPlayer(AudioClip clip)
    {      
        sfxPlayer.PlayOneShot(clip);
    }

    // Enemies
    public void PlaySFXEnemy(AudioClip clip)
    {
        sfxEnemy.PlayOneShot(clip);
    }

    // Menu
    public void PlaySFXMenu(AudioClip clip)
    {
        sfxMenu.PlayOneShot(clip);
    }

    // Abilities
    public void PlaySFXAbilities(AudioClip clip)
    {
        sfxAbilities.PlayOneShot(clip);
    }

    // Gun
    public void PlaySFXGun(AudioClip clip)
    {
        sfxGun.PlayOneShot(clip);
    }

    // Melee
    public void PlaySFXMelee(AudioClip clip)
    {
        sfxMelee.PlayOneShot(clip);
    }

    // Interact Sounds
    public void PlaySFXInteractSounds(AudioClip clip)
    {
        sfxInteractSounds.PlayOneShot(clip);
    }
}
