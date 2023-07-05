using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

    [Header("----- UI Stuff -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public TextMeshProUGUI enemiesRemainingText;
    public Image playerHPBar;
    public GameObject playerFlashDamageScreen;

    [Header("----- Enemy Stuff -----")]
    [SerializeField] public int enemiesPerWave;
    [SerializeField] public int maxWaves;
    [SerializeField] public GameObject[] enemyTypes;
    [SerializeField] public int maxEnemies;

    GameObject[] enemySpawnLocs;
    int enemiesInScene;
    int enemiesRemaining;
    bool isPaused;
    float timescaleOrig;
    int wave = 1;

    //Awake is called before Start
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timescaleOrig = Time.timeScale;
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        enemySpawnLocs = GameObject.FindGameObjectsWithTag("Enemy Spawn");

        //temp
        spawnEnemies(enemiesPerWave);
        //temp
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
    //updates enemies remaining and if no enemies remain sets active menu to win
    public void updateGameGoal(int amount)
    {
        enemiesRemaining += amount;
        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        if (enemiesRemaining <= 0)
        {
            activeMenu = winMenu;
            activeMenu.SetActive(true);
            statePaused();
        }
    }

    public IEnumerator playerFlashDamage()
    {
        playerFlashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerFlashDamageScreen.SetActive(false);
    }

    void spawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomSpawnLoc = Random.Range(0, enemySpawnLocs.Length);
            int randomEnemyType = Random.Range(0, enemyTypes.Length);

            Debug.Log("Spawning enemy type: " + randomEnemyType + " at spawner: " + randomSpawnLoc);

            Instantiate(
                enemyTypes[randomEnemyType], 
                enemySpawnLocs[randomSpawnLoc].transform.position, 
                enemySpawnLocs[randomSpawnLoc].transform.rotation);
        }
    }
}
