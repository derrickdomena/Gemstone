using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class playerController : MonoBehaviour, IDamage, ShopCustomer
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] public CharacterController controller;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] public GameObject midMass;
    static int death = 0;
    private string deathCounter;


    [Header("----- Player Stats -----")]
    // Health
    [SerializeField] public int hp;
    [SerializeField] public int totalDamage;
    // Movement
    [SerializeField] public float walkSpeed;
    [SerializeField] public float sprintSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;
    [SerializeField] public float dashCooldown;
    [SerializeField] public float grenadeCooldown;

    // Other Stats
    [SerializeField] public float critChance;
    [SerializeField] public float critChanceOrig;
    [SerializeField] public int dashCount;

    // Poison Effect stats



    [Header("----- Gun Stats -----")]
    [SerializeField] public List<GunStats> gunList = new List<GunStats>();
    [SerializeField] float shootRate;
    [SerializeField] public int shootDamage;
    [SerializeField] public int shootDamageOrig;
    [SerializeField] public float critDam;
    [SerializeField] int shootDistance;

    [Header("----- Gun Components -----")]
    [SerializeField] GameObject gunModel;
    public PlayerStats playerStats;

    // Aiming Positions
    [SerializeField] GameObject gunModelAimPos;
    [SerializeField] GameObject rifleModelAimPos;
    [SerializeField] GameObject smgModelAimPos;
    [SerializeField] GameObject sarModelAimPos;

    // MuzzlePositions
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject smgMuzzleFlashPOS;
    [SerializeField] GameObject rifleMuzzleFlashPOS;
    [SerializeField] GameObject sarMuzzleFlashPOS;

    public AudioSource audioSource;
    [SerializeField] AudioClip autoAudioClip;
    [SerializeField] AudioClip semiAudioClip;
    [SerializeField] AudioClip rifleAudioClip;

    Audio audioManager;

    Vector3 gunModelOrig;

    public int selectedGun;

    [Header("----- Melee Stats -----")]
    [SerializeField] public List<MeleeStats> meleeList = new List<MeleeStats>();

    public float attackDistance;
    public float attackDelay;
    public float attackSpeed;
    public int attackDamage;

    [Header("----- Melee Components -----")]
    [SerializeField] GameObject meleeModel;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    public int selectedMelee;

    bool isAttacking;

    public AudioClip explosionSound;

    [Header("----- Keybinds -----")]
    // Keybinds
    // Movement
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Reload
    public KeyCode reloadKey = KeyCode.R;

    // Health
    public int hpOrig;
    public int hpMax;
    public int gems;
    public bool immune;
    // Movement
    [HideInInspector]
    public Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed;
    int jumpCount;

    // Shooting
    bool isShooting;
    bool reloadTutorial;

    // Aiming bool
    private bool isAiming = false;


    private RaycastHit target;

    public AudioSource walkingSound, runningSound, jumpSound;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PlayerPrefs.SetInt(deathCounter, death);
        gunModelOrig = gunModel.transform.localPosition;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<Audio>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        reloadTutorial = true;
        hpOrig = hp;
        shootDamageOrig = shootDamage;
        SpawnPlayer();
        dashCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        playerStats.TotalDamage = totalDamage;
        if (gameManager.instance.activeMenu == null)
        {
            if(!gameManager.instance.dontMove)
            {
                Movement();

            }
            StateHandler();

            gameManager.instance.Health.text = hp.ToString();
            gameManager.instance.HealthMax.text = hpOrig.ToString();

            // Checks if player is aiming or not
            bool isAimButtonPressed = Input.GetButton("Aim");

            if (gunList.Count > 0)
            {
                // if player is aiming then transform position of weapon, else put weapon pos back to original
                if (isAimButtonPressed && !isAiming)
                {
                    gunModel.transform.position = gunModelAimPos.transform.position;
                    isAiming = true;
                }
                else if (!isAimButtonPressed && isAiming)
                {
                    gunModel.transform.localPosition = gunModelOrig;
                    isAiming = false;
                }

                if (!isAiming)
                {
                    ScrollGuns();
                }

                // Switch between Automatic shooting and Semi Automatic shooting
                if (gunList[selectedGun].auto && Input.GetButton("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }
                else if (!gunList[selectedGun].auto && Input.GetButtonDown("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }

                //If ammo reaches 0 and reserve ammo are full display reload text
                if (reloadTutorial == true && gunList[selectedGun].ammoCurr <= 0 && gunList[selectedGun].ammoReserve > 0 && gameManager.instance.activeMenu == null)
                {
                    reloadTutorial = false;
                    StartCoroutine(gameManager.instance.OutOfAmmo());
                }

                //reload
                if (Input.GetKeyDown(reloadKey))
                {
                    //Debug.Log("reloaded");
                    reloadTutorial = false;
                    ReloadWeapon();
                }

                UpdatePlayerUI();
            }

            if (meleeList.Count > 0 && gunList.Count <= 0)
            {
                ScrollMelee();
                if (Input.GetMouseButtonDown(0) && !isAttacking)
                {
                    StartCoroutine(MeleeAttack());
                }           
            }
        }
        else
        {
            walkingSound.enabled = false;
            runningSound.enabled = false;
            jumpSound.enabled = false;
        }
    }

    // Movement
    void Movement()
    {
        controller.enabled = true;
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpCount = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) +
               (transform.forward * Input.GetAxis("Vertical"));

        controller.Move(playerSpeed * Time.deltaTime * move);

        // Audio
        if(hp > 0)
        {
            if (move != Vector3.zero)
            {
                walkingSound.enabled = true;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    walkingSound.enabled = false;
                    runningSound.enabled = true;
                }
                else
                {
                    walkingSound.enabled = true;
                    runningSound.enabled = false;
                }
            }
            else
            {
                walkingSound.enabled = false;
                runningSound.enabled = false;
            }
            // Seperate jump check, due to the conditions on the if statement below dont play audio 
            if (Input.GetKey(KeyCode.Space))
            {
                jumpSound.enabled = true;
                walkingSound.enabled = false;
                runningSound.enabled = false;
            }
            else
            {
                jumpSound.enabled = false;
            }
        }
        

        // Jump
        // Allows for single consecutive jumps when grounded without needing to press jumpKey again
        // or double jump while in the air if jumpsCount is less than jumpsMax
        if (Input.GetKeyDown(jumpKey) && jumpCount < jumpsMax || groundedPlayer && Input.GetKey(jumpKey))
        {
            playerVelocity.y = jumpHeight;          
            jumpCount++;          
        }

        // Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            // Character Controller Height
            controller.height = controller.height * 0.5f;
            // Capsule Collider Height
            capsuleCollider.height = capsuleCollider.height * 0.5f;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            // Character Controller Height
            controller.height = controller.height * 2;
            // Capsule Collider Height
            capsuleCollider.height = capsuleCollider.height * 2;
        }
        
        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Sets the movement speed for each movement state
    private void StateHandler()
    {
        // Movement - Crouching
        if (groundedPlayer && Input.GetKey(crouchKey))
        {
            playerSpeed = walkSpeed / 2;
        }
        // Movement - Running
        if (groundedPlayer && Input.GetKey(sprintKey) && !Input.GetKey(crouchKey))
        {
            playerSpeed = sprintSpeed;
        }
        // Movement - Walking
        else if (groundedPlayer)
        {
            playerSpeed = walkSpeed;
        }
    }

    // Apply Damage done by Enemies
    public void TakeDamage(int amount)
    {
        if (immune == true) { return; }
        hp -= amount;
        StartCoroutine(gameManager.instance.PlayerFlashDamage());
        UpdatePlayerUI();

        if (hp <= 0)
        {
            audioManager.musicSource.Stop();          
            if (PlayerPrefs.GetInt(deathCounter)  == 0)
            {
                death++;
                gameManager.instance.FirstDeath();
            }
            else
            {
                gameManager.instance.youLose();
            }
        }
    }

    //Apply Poison Damage to player
    public void TakePoisonDamage(int amount)
    {
        if (immune == true) { return; }
        hp -= amount;
        StartCoroutine(gameManager.instance.PoisonFlashDamage());
        UpdatePlayerUI();

        if (hp <= 0)
        {
            audioManager.musicSource.Stop();
            if (PlayerPrefs.GetInt(deathCounter) == 0)
            {
                death++;
                gameManager.instance.FirstDeath();
            }
            else
            {
                gameManager.instance.youLose();
            }
        }
    }

    // Updates the Player UI health
    public void UpdatePlayerUI()
    {       
        gameManager.instance.playerHPBar.fillAmount = (float)hp / hpOrig;
        gameManager.instance.Health.text = hp.ToString();
        gameManager.instance.HealthMax.text = hpOrig.ToString();

        if (gunList.Count > 0)
        {
            gameManager.instance.ammoCur.text = gunList[selectedGun].ammoCurr.ToString("f0");
            gameManager.instance.ammoReserve.text = gunList[selectedGun].ammoReserve.ToString("f0");
        }
    }

    // Sets a position for the player starting point
    public void SpawnPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;

        hp = hpOrig;
    }

    // Manages Shooting
    IEnumerator Shoot()
    {
        if (gunList[selectedGun].ammoCurr > 0)
        {
            isShooting = true;
            StartCoroutine(muzzleFlashTimer());
            gunList[selectedGun].ammoCurr--;

            // Shooting Audio

            string name = gunList[selectedGun].name;

            switch (name)
            {
                case "Rifle":
                    audioSource.PlayOneShot(rifleAudioClip, .7f);
                    break;
                case "SMG":
                    audioSource.PlayOneShot(autoAudioClip, .7f);
                    break;
                case "SAR":
                    audioSource.PlayOneShot(semiAudioClip, .7f);
                    break;
                default:
                    Debug.Log("Audio Error - playerController: SetMuzzlePOS() - NO AUDIO FOR WEAPON");
                    break;
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {
                target = hit;
                IDamage damagable = hit.collider.GetComponent<IDamage>();

                if (damagable != null && !hit.collider.CompareTag("Player"))
                {
                    int critCheck = Random.Range(0, 100);
                    if (critCheck <= critChance)
                    {
                        damagable.TakeDamage((int)(shootDamage + critDam));
                    }
                    else
                    {
                        damagable.TakeDamage(shootDamage);
                    }
                }
                else
                {
                    Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;

        }
    }
    // Set muzzleFlash to active on timer and turn off
    IEnumerator muzzleFlashTimer()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.SetActive(false);
    }

    // Handles picking up weapons
    public void GunPickup(GunStats gunstat)
    {
        gunList.Add(gunstat);

        shootDamage = gunstat.shootDamage;
        shootDistance = gunstat.shootDist;
        shootRate = gunstat.shootRate;

        selectedGun = gunList.Count - 1;

        gunModel.GetComponent<MeshFilter>().mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;

        SetMuzzlePOS();
        UpdatePlayerUI();
    }

    // Handles scrolling through weapons
    void ScrollGuns()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            ChangeGunStats();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            ChangeGunStats();
        }
    }

    void ScrollMelee()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedMelee < meleeList.Count - 1)
        {
            selectedMelee++;           
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedMelee > 0)
        {
            selectedMelee--;
        }
    }

    // Method for changing weapon stats
    public void ChangeGunStats()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;

        SetMuzzlePOS();

        UpdatePlayerUI();
    }

    // Reload Weapon method
    void ReloadWeapon()
    {
        int ammoLeft = gunList[selectedGun].ammoReserve;
        int ammoEmpty = gunList[selectedGun].ammoMax - gunList[selectedGun].ammoCurr;

        if (gunList[selectedGun].ammoReserve > 0)
        {
            if (ammoEmpty > gunList[selectedGun].ammoReserve)
            {
                gunList[selectedGun].ammoCurr = ammoLeft;
                gunList[selectedGun].ammoReserve -= ammoLeft;
            }
            else
            {
                gunList[selectedGun].ammoReserve -= (gunList[selectedGun].ammoMax - gunList[selectedGun].ammoCurr);
                gunList[selectedGun].ammoCurr = gunList[selectedGun].ammoMax;
            }
        }
    }

    // Sets the position of the muzzleFlash dependent on gun name
    void SetMuzzlePOS() 
    {
        string name = gunList[selectedGun].name;

        switch (name)
        {
            case "Rifle":
                muzzleFlash.transform.position = rifleMuzzleFlashPOS.transform.position;
                gunModelAimPos.transform.position = rifleModelAimPos.transform.position;

                break;
            case "SMG":
                muzzleFlash.transform.position = smgMuzzleFlashPOS.transform.position;
                gunModelAimPos.transform.position = smgModelAimPos.transform.position;
                break;
            case "SAR":
                muzzleFlash.transform.position = sarMuzzleFlashPOS.transform.position;
                gunModelAimPos.transform.position = sarModelAimPos.transform.position;
                break;
        }
    }

    // Handles picking up melee
    public void MeleePickup(MeleeStats meleestat)
    {
        meleeList.Add(meleestat);

        attackDamage = meleestat.attackDamage;
        attackSpeed = meleestat.attackSpeed;
        attackDelay = meleestat.attackDelay;
        attackDistance = meleestat.attackDistance;

        selectedMelee = meleeList.Count - 1;

        meleeModel.GetComponent<MeshFilter>().mesh = meleeList[selectedMelee].model.GetComponent<MeshFilter>().sharedMesh;
        meleeModel.GetComponent<MeshRenderer>().material = meleeList[selectedMelee].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Melee attacks
    IEnumerator MeleeAttack()
    {
        isAttacking = true;

        audioSource.PlayOneShot(swordSwing);
       
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out RaycastHit hit, attackDistance))
        {      
            IDamage damagable = hit.collider.GetComponent<IDamage>();

            if (damagable != null && !hit.collider.CompareTag("Player"))
            {
                damagable.TakeDamage(attackDamage);
                totalDamage += attackDamage;
            }
        }

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    public int getGemAmount()
    {
        return gameManager.instance.gemCount;
    }

    public bool TrySpendGemAmount(int gemAmount)
    {
        if (getGemAmount() - gemAmount >= 0)
        { 
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Immune(bool var)
    {
        immune = var;
    }

}

