using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.AI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

    [Header("----- UI Stuff -----")]
    public GameObject activeMenu = null;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public TextMeshProUGUI enemiesRemainingText;
    public Image playerHPBar;
    public GameObject playerFlashDamageScreen;
    public TextMeshProUGUI ammoCur;
    public TextMeshProUGUI ammoReserve;
    public GameObject reload;
    public GameObject nextWave;
    public Image gem;
    public TextMeshProUGUI gemText;
    public int gemCount;
    public GameObject shop;
    public GameObject ShopMask;
    public Image BossHPBar;
    public GameObject bossHP;
    public Image grenadeCooldownFill;
    public Image dashCooldownFill;

    [Header("----- Spawner Stuff -----")]
    public float waveTimer;
    public int enemiesPerWave;
    public int maxWaves;
    public int maxEnemies;

    public int enemiesRemaining;
    public int enemiesInScene;
    public int wave = 1;
    bool isPaused;
    float timescaleOrig;
    
    //Awake is called before Start
    void Awake()
    {
        //singleton
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        //persist between scenes
        DontDestroyOnLoad(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timescaleOrig = Time.timeScale;
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
    }

    // Start is called before the first frame update
    private void Start()
    {
        updateGameGoal(wave * enemiesPerWave);
    }

    // Update is called once per frame
    void Update()
    {
        //pressing ESC pauses the game
        if(Input.GetButtonDown("Cancel") && activeMenu == null) 
        {
            statePaused();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
        }
    }

    //Pause game instance and unlocks cursor to the area of the game
    public void statePaused()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = !isPaused;
    }

    //Resumes game instance from timeScale locks cursor
    public void stateUnpaused()
    {
        Time.timeScale = timescaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void enemyCheckIn()
    {
        enemiesInScene++;
    }

    public void enemyCheckOut()
    {
        enemiesInScene--;
        updateGameGoal(-1);
    }

    //updates enemies remaining and if no enemies remain sets active menu to win
    //also checks the amount of waves left before displaying win screen
    public void updateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        if (enemiesRemaining <= 0 && wave >= maxWaves)
        {
            //end of level
        }
    }

    //Sets active menu to loseMenu
    public void youLose()
    {
        statePaused();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }

    //first time ammo is 0
    public IEnumerator OutOfAmmo()
    {
        reload.SetActive(true);
        yield return new WaitForSeconds(1);
        reload.SetActive(false);
    }

    //when a player is damaged Flash the screen for .01 seconds
    public IEnumerator playerFlashDamage()
    {
        playerFlashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerFlashDamageScreen.SetActive(false);
    }

    //flashes the nextWave game object on screen for 4 seconds
    public IEnumerator NextWave()
    {
        nextWave.SetActive(true);
        StartCoroutine(Countdown());
        yield return new WaitForSeconds(4);
        nextWave.SetActive(false);
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(waveTimer);
    }

    //updates the gem amount
    public void updateGemCount(int amount)
    {
        gemCount += amount;
        gemText.text = gemCount.ToString("F0");
    }
    
    //sets active menu to shop
    public void Vendor()
    {
        statePaused();
        ShopMask.SetActive(true);
        activeMenu = shop;
        activeMenu.SetActive(true);
    }
}
