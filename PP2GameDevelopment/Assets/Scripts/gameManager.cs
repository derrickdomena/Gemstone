using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;
    public PlayerStats playerStats;
    public bool dontMove;

    [Header("----- UI Stuff -----")]
    public GameObject activeMenu = null;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject levelClearedMenu;
    public GameObject loseMenu;
    public TextMeshProUGUI enemiesRemainingText;
    public Image playerHPBar;
    public GameObject playerFlashDamageScreen;
    public GameObject poisonFlashDamageScreen;
    public TextMeshProUGUI ammoCur;
    public TextMeshProUGUI ammoReserve;
    public TextMeshProUGUI HealthMax;
    public TextMeshProUGUI Health;
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
    public Image stasisCooldownFill;
    public Image dashCooldownFill;
    public Animator anim;
    public GameObject cutscene;
    public GameObject nextLevelCounter;
    public GameObject wavesLeftCounter;
    public TextMeshProUGUI wavesLeftText;

    [SerializeField] public float poisonEffectDuration;
    [SerializeField] public float poisonTimer = 0.0f;
    [SerializeField] public bool isPoisoned = false;

    public GameObject miniMap;
    public GameObject fullMap;
    public Transform chatBubble;

    [Header("----- UI Stuff Difficulty -----")]
    public float difficulty;

    [Header("----- Level Stuff -----")]
    public List<GameObject> rooms = new List<GameObject>();
    public GameObject[,] level;
    public int maxRooms = 10;
    public List<GameObject> capRooms = new List<GameObject>();
    public List<GameObject> BossRooms = new List<GameObject>();
    public int roomCount;
    public bool generated = false;

    [Header("----- Spawner Stuff -----")]
    public float waveTimer;
    public int enemiesPerWave;
    public int maxWaves;
    public int maxEnemies;

    public int enemiesRemaining;
    public int enemiesKilled;
    public int wave = 1;
    bool isPaused;
    float timescaleOrig;
    public bool isMiniMap;

    KeyCode mapKey = KeyCode.M;

    [Header("----SceneStuff----")]
    [SerializeField] int timer;
    //Awake is called before Start
    void Awake()
    {
        dontMove = false;
        enemiesKilled = 0;
        instance = this;
        level = new GameObject[50, 50];
        generated = false;
        //DontDestroyOnLoad(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timescaleOrig = Time.timeScale;
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");      
    }

    // Start is called before the first frame update
    private void Start()
    {
        playerScript.SpawnPlayer();
        //updateGameGoal(wave * enemiesPerWave);
        if (player == null) 
        {
            Instantiate(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerStats.NumberOfKills = enemiesKilled;
        //pressing ESC pauses the game
        if(Input.GetButtonDown("Cancel") && activeMenu == null) 
        {
            statePaused();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
        }
        if(Input.GetButtonDown("Skip"))
        {
            StartCoroutine(NextSceneTimer());
        }
        if (Input.GetButtonDown("MoveToSpawn"))
        {
            player.transform.position = playerSpawnPos.transform.position;
        }
        ShowMap();    

        if (isPoisoned)
        {
            poisonTimer += Time.deltaTime;
            float poisonDurOrig = poisonEffectDuration;
            if (poisonTimer >= 2 && poisonEffectDuration != 0)
            {
                poisonEffectDuration--;
                playerScript.TakePoisonDamage(1);
                poisonTimer = 0;
            }
            else if (poisonEffectDuration == 0)
            {
                poisonEffectDuration = poisonDurOrig;
                poisonTimer = 0;
                isPoisoned = false;
            }
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
        StartCoroutine(WaitForFrame());    
    }

    public void enemyCheckIn()
    {
       
        updateGameGoal(1);
    }

    public void enemyCheckOut()
    {
        enemiesKilled++;
        updateGameGoal(-1);
    }

    //updates enemies remaining and if no enemies remain sets active menu to win
    //also checks the amount of waves left before displaying win screen
    public void updateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        if(NextScene())
        {
            StartCoroutine(NextSceneTimer());
        }
    }

    //Sets active menu to loseMenu
    public void youLose()
    {
        if(bossHP.activeInHierarchy == true)
        {
            bossHP.SetActive(false);
        }
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
    public IEnumerator PlayerFlashDamage()
    {
        playerFlashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerFlashDamageScreen.SetActive(false);
    }

    //when a player is damaged by Poison effect flash the screen for .01 seconds
    public IEnumerator PoisonFlashDamage()
    {
        poisonFlashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        poisonFlashDamageScreen.SetActive(false);
    }

    //flashes the nextWave game object on screen for 4 seconds
    public IEnumerator NextWave()
    { 
        nextWave.SetActive(true);
        StartCoroutine(Countdown());
        yield return new WaitForSeconds(4);
        nextWave.SetActive(false);
    }
    public IEnumerator LevelCleared()
    {
        yield return new WaitForSeconds(5f);
        dontMove = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        activeMenu = levelClearedMenu;
        activeMenu.SetActive(true);
    }
    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(waveTimer);
    }

    //updates the gem amount
    public void updateGemCount(int amount)
    {
        playerScript.gems += amount;
        gemCount = playerScript.gems;
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

    IEnumerator NextSceneTimer()
    {
        nextLevelCounter.SetActive(true);
        yield return new WaitForSeconds(timer);
        anim.SetTrigger("FadeOut");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            nextLevelCounter.SetActive(false);
            player.transform.position = Vector3.zero;
            SceneManager.LoadScene(2);
        }
        else
        {
            player.transform.position = Vector3.zero;
            nextLevelCounter.SetActive(false);
            SceneManager.LoadScene(3);
        }
    }
    public bool NextScene()
    {
        if(enemiesRemaining <= 0 && wave >= maxWaves)
        {
            return true;
        }

        return false;
    }

    public IEnumerator YouWin()
    {
        yield return new WaitForSeconds(5);
        activeMenu = winMenu;
        activeMenu.SetActive(true);
        statePaused();
    }
    public void FirstDeath()
    {
        gameManager.instance.playerScript.Immune(true);
        anim.SetTrigger("FadeIn");
        StartCoroutine(FirstDeathTimer());       
    }

    IEnumerator FirstDeathTimer()
    {
        cutscene.SetActive(true);     
        yield return new WaitForSeconds(4);        
        youLose();
    }

    // Toggles between the mini-map and the fullsize map
    void ShowMap()
    {
        if (Input.GetKeyDown(mapKey) && !isMiniMap)
        {
            miniMap.SetActive(false);
            fullMap.SetActive(true);
            isMiniMap = true;
        }
        else if (Input.GetKeyDown(mapKey) && isMiniMap)
        {           
            miniMap.SetActive(true);
            fullMap.SetActive(false);
            isMiniMap = false;
        }
    }
    IEnumerator WaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        activeMenu = null;
    }
}
