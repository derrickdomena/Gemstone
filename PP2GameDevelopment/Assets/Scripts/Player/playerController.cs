using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour, IDamage, ShopCustomer
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] public CharacterController controller;
    //[SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] public GameObject midMass;
    public GameObject FirstPersonCamera;
    public GameObject CutSceneCamera;
    public GameObject CutScenePlayerCamera;
    public GameObject Cutscene;


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
    [SerializeField] public float dashCooldownMin;
    [SerializeField] public float fireballCooldown;
    [SerializeField] public float fireballCooldownMin;
    [SerializeField] public float stasisCooldown;

    // Other Stats
    [SerializeField] public float critChance;
    [SerializeField] public float critChanceOrig;
    [SerializeField] public int dashCount;
    [SerializeField] public int dashCountMax;

    // Poison Effect stats
    [SerializeField] public float poisonEffectDuration;
    [SerializeField] public float poisonTimer = 0.0f;
    [SerializeField] public bool isPoisoned = false;
    public float poisonDurOrig = 0.0f;


    [Header("----- Gun Stats -----")]
    [SerializeField] public List<WeaponStats> weaponList = new List<WeaponStats>();
    [SerializeField] float shootRate;
    [SerializeField] public int shootDamage;
    [SerializeField] public int shootDamageOrig;
    [SerializeField] public float critDam;
    [SerializeField] int shootDistance;
    bool outOfAmmo = true;
    [Header("----- Gun Components -----")]
    [SerializeField] public GameObject gunModel;
    public PlayerStats playerStats;

    // Aiming Positions
    [SerializeField] GameObject gunModelAimPos;
    [SerializeField] GameObject rifleModelAimPos;
    [SerializeField] GameObject smgModelAimPos;
    [SerializeField] GameObject sarModelAimPos;
    [SerializeField] GameObject SwordPos;
    [SerializeField] GameObject GunPos;

    // MuzzlePositions
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject smgMuzzleFlashPOS;
    [SerializeField] GameObject rifleMuzzleFlashPOS;
    [SerializeField] GameObject sarMuzzleFlashPOS;
    public int selectedWeapon;
    Vector3 gunModelOrig;

    WeaponRecoil weaponRecoil;

    // Audio
    AudioManager audioManager;

    [Header("----- Melee Stats -----")]
    //[SerializeField] public List<MeleeStats> meleeList = new List<MeleeStats>();

    public float attackDistance;
    public float attackSpeed;
    public int attackDamage;

    public string weaponType = "default";

    [Header("----- Melee Components -----")]
    [SerializeField] public GameObject meleeModel;
    [SerializeField] GameObject meleeAnimation;
    //public GameObject hitEffect;
    public int selectedMelee;
    bool isAttacking;

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

    bool isPlaying = false;
    private RaycastHit target;

    float defaultPlayerRadius = 0.5f;
    
    private void Awake()
    {       
        DontDestroyOnLoad(gameObject);       
        gunModelOrig = gunModel.transform.localPosition;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        weaponRecoil = GameObject.Find("WeaponHolder").GetComponent<WeaponRecoil>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        reloadTutorial = true;
        hpOrig = hp;
        poisonDurOrig = poisonEffectDuration;
        shootDamageOrig = shootDamage;
        SpawnPlayer();
        dashCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Since player changes to another scene the gameobject needs to be called again to other audiomanager in the scene. 
        if (audioManager == null)
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

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

            if (weaponList.Count > 0)
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
                    //ScrollGuns();
                    ScrollWeapon();
                }

                // Switch between Automatic shooting and Semi Automatic shooting
                if (weaponList[selectedWeapon].auto && Input.GetButton("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }
                else if (!weaponList[selectedWeapon].auto && Input.GetButtonDown("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }

                //If ammo reaches 0 and reserve ammo are full display reload text
                if (reloadTutorial == true && weaponList[selectedWeapon].ammoCurr <= 0 && weaponList[selectedWeapon].ammoReserve > 0 && gameManager.instance.activeMenu == null)
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

            if (weaponType == "Melee" && Input.GetMouseButtonDown(0) && !isAttacking)
            {
                StartCoroutine(MeleeAttack());
            }
        }

        if (isPoisoned)
        {
            poisonTimer += Time.deltaTime;
            if (poisonTimer >= 2 && poisonEffectDuration != 0)
            {
                poisonEffectDuration--;
                TakePoisonDamage();
                poisonTimer = 0;
            }
            else if (poisonEffectDuration == 0)
            {
                isPoisoned = false;
            }
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
            //capsuleCollider.height = capsuleCollider.height * 0.5f;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            // Character Controller Height
            controller.height = controller.height * 2;
            // Capsule Collider Height
            //capsuleCollider.height = capsuleCollider.height * 2;
        }

        if (Input.GetKeyDown(sprintKey))
        {
            controller.radius = controller.radius * 2;
        }

        if (Input.GetKeyUp(sprintKey))
        {
            controller.radius = controller.radius * 0.5f;
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
            //audioManager.musicSource.Stop();            
            gameManager.instance.Death();
            
        }
    }

    //Apply Poison Damage to player
    public void TakePoisonDamage()
    {
        if (immune == true) { return; }
        int poison = hpOrig / 10;
        hp -= poison;
        StartCoroutine(gameManager.instance.PoisonFlashDamage());
        UpdatePlayerUI();

        if (hp <= 0)
        {
            audioManager.musicSource.Stop();
            gameManager.instance.Death();
        }
    }

    // Updates the Player UI health
    public void UpdatePlayerUI()
    {       
        gameManager.instance.playerHPBar.fillAmount = (float)hp / hpOrig;
        gameManager.instance.Health.text = hp.ToString();
        gameManager.instance.HealthMax.text = hpOrig.ToString();
        gameManager.instance.ammoReserve.text = weaponList[gameManager.instance.playerScript.selectedWeapon].ammoReserve.ToString();

        if (weaponList.Count > 0)
        {
            gameManager.instance.ammoCur.text = weaponList[selectedWeapon].ammoCurr.ToString("f0");
            gameManager.instance.ammoReserve.text = weaponList[selectedWeapon].ammoReserve.ToString("f0");
        }
    }

    // Sets a position for the player starting point
    public void SpawnPlayer()
    {
        controller.enabled = false;
        controller.radius = defaultPlayerRadius;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;

        hp = hpOrig;
    }

    // Manages Shooting
    IEnumerator Shoot()
    {  
        if (weaponList[selectedWeapon].ammoCurr > 0)
        {
            isShooting = true;
            weaponRecoil.Recoil();
            StartCoroutine(muzzleFlashTimer());
            weaponList[selectedWeapon].ammoCurr--;

            // Shooting Audio

            string name = weaponList[selectedWeapon].name;

            switch (name)
            {
                case "Rifle":
                    //audioSource.PlayOneShot(rifleAudioClip, .7f);
                    audioManager.PlaySFXGun(audioManager.rifleSound);
                    
                    break;
                case "SMG":
                    //audioSource.PlayOneShot(autoAudioClip, .7f);
                    audioManager.PlaySFXGun(audioManager.autoSound);
                    audioManager.sfxSource.Stop();
                    break;
                case "SAR":
                    //audioSource.PlayOneShot(semiAudioClip, .7f);
                    audioManager.PlaySFXGun(audioManager.semiSound);
                    break;
                default:
                    //Debug.Log("Audio Error - playerController: SetMuzzlePOS() - NO AUDIO FOR WEAPON");
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
                    Instantiate(weaponList[selectedWeapon].hitEffect, hit.point, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;

        }
        else
        {
            if(outOfAmmo == true)
            StartCoroutine(outOfAmmoSoundEffect());
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
    public void WeaponPickup(WeaponStats weaponStat)
    {   
        
        weaponList.Add(weaponStat);

        shootDamage = weaponStat.shootDamage;
        shootDistance = weaponStat.shootDist;
        shootRate = weaponStat.shootRate;
        attackDamage = weaponStat.attackDamage;
        attackSpeed = weaponStat.attackSpeed;
        attackDistance = weaponStat.attackDistance;

        weaponType = weaponStat.weaponType;

        selectedWeapon = weaponList.Count - 1;

        

        if (weaponList[selectedWeapon].weaponType == "Melee")
        {
            gunModel.SetActive(false);
            meleeModel.SetActive(true);
            meleeModel.GetComponent<MeshFilter>().mesh = weaponList[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
            meleeModel.GetComponent<MeshRenderer>().material = weaponList[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
            meleeModel.transform.position = SwordPos.transform.position;
            meleeModel.transform.rotation = SwordPos.transform.rotation;
        }
        else
        {
            meleeModel.SetActive(false);
            gunModel.SetActive(true);
            gunModel.GetComponent<MeshFilter>().mesh = weaponList[selectedWeapon].model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().material = weaponList[selectedWeapon].model.GetComponent<MeshRenderer>().sharedMaterial;
        }
        SetMuzzlePOS();
        UpdatePlayerUI();
    }

    // Handles scrolling through weapons
    void ScrollWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon < weaponList.Count - 1)
        {
            selectedWeapon++;
            ChangeWeaponStats();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)
        {
            selectedWeapon--;
            ChangeWeaponStats();
        }

        // Weapon Switching with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (weaponList[selectedWeapon].weaponType == "Gun")
            {
                selectedWeapon = 0;
                ChangeWeaponStats();
            }
            else
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (weaponList[selectedWeapon].weaponType == "Melee")
            {
                selectedWeapon = 1;
                ChangeWeaponStats();
            }
            else
            {
                return;
            }
        }
    }

    //void ScrollMelee()
    //{
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedMelee < weaponList.Count - 1)
    //    {
    //        selectedMelee++;
    //    }
    //    else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedMelee > 0)
    //    {
    //        selectedMelee--;
    //    }
    //}

    // Method for changing weapon stats
    public void ChangeWeaponStats()
    {
        shootDamage = weaponList[selectedWeapon].shootDamage;
        shootDistance = weaponList[selectedWeapon].shootDist;
        shootRate = weaponList[selectedWeapon].shootRate;
        attackDamage = weaponList[selectedWeapon].attackDamage;
        attackSpeed = weaponList[selectedWeapon].attackSpeed;
        attackDistance = weaponList[selectedWeapon].attackDistance;

        weaponType = weaponList[selectedWeapon].weaponType;

        ChangeWeapon();
        SetMuzzlePOS();
        UpdatePlayerUI();
    }
    public void ChangeWeapon()
    {
        if (!meleeModel.activeSelf && isAttacking == false)
        {
            meleeModel.SetActive(true);
            gunModel.SetActive(false);
        }
        else if (meleeModel.activeSelf && isAttacking == false)
        {
            meleeModel.SetActive(false);
            gunModel.SetActive(true);
        }
    }
    // Reload Weapon method
    void ReloadWeapon()
    {
        int ammoLeft = weaponList[selectedWeapon].ammoReserve;
        int ammoEmpty = weaponList[selectedWeapon].ammoMax - weaponList[selectedWeapon].ammoCurr;

        if (weaponList[selectedWeapon].ammoReserve > 0)
        {
            if (ammoEmpty > weaponList[selectedWeapon].ammoReserve)
            {
                weaponList[selectedWeapon].ammoCurr = ammoLeft;
                weaponList[selectedWeapon].ammoReserve -= ammoLeft;
            }
            else
            {
                weaponList[selectedWeapon].ammoReserve -= (weaponList[selectedWeapon].ammoMax - weaponList[selectedWeapon].ammoCurr);
                weaponList[selectedWeapon].ammoCurr = weaponList[selectedWeapon].ammoMax;
            }
        }
    }

    // Sets the position of the muzzleFlash dependent on gun name
    void SetMuzzlePOS() 
    {
        string name = weaponList[selectedWeapon].name;

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
    //public void MeleePickup(WeaponStats meleestat)
    //{
    //    weaponList.Add(meleestat);

    //    attackDamage = meleestat.attackDamage;
    //    attackSpeed = meleestat.attackSpeed;
    //    attackDistance = meleestat.attackDistance;

    //    selectedMelee = weaponList.Count - 1;

    //    meleeModel.GetComponent<MeshFilter>().mesh = weaponList[selectedMelee].model.GetComponent<MeshFilter>().sharedMesh;
    //    meleeModel.GetComponent<MeshRenderer>().material = weaponList[selectedMelee].model.GetComponent<MeshRenderer>().sharedMaterial;
    //}

    // Melee attacks
    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        meleeModel.SetActive(false);
        meleeAnimation.SetActive(true);
        audioManager.PlaySFXMelee(audioManager.swingSound);
       
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out RaycastHit hit, attackDistance))
        {      
            IDamage damagable = hit.collider.GetComponent<IDamage>();

            if (damagable != null && !hit.collider.CompareTag("Player"))
            {
                damagable.TakeDamage(attackDamage);
                totalDamage += attackDamage;
            }
        }
        yield return new WaitForSeconds(attackSpeed);
        meleeAnimation.SetActive(false);
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
    IEnumerator outOfAmmoSoundEffect()
    {
        outOfAmmo = false;
        audioManager.PlaySFXGun(audioManager.outOfAmmoSound);
        yield return new WaitForSeconds(.5f);
        outOfAmmo = true;
    }
    // Animation Events Audio
    private void Step()
    {
        audioManager.PlaySFXPlayer(audioManager.walkingSound);
    }

    private void Land()
    {
        audioManager.PlaySFXPlayer(audioManager.landingSound);
    }

}

