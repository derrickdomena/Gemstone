using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] public CharacterController controller;
    [SerializeField] CapsuleCollider capsuleCollider;

    [Header("----- Player Stats -----")]
    // Health
    [SerializeField] public int hp;

    // Movement
    [SerializeField] public float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;

    [Header("----- Gun Stats -----")]
    [SerializeField] public List<GunStats> gunList = new List<GunStats>();
    [SerializeField] float shootRate;
    [SerializeField] public int shootDamage;
    [SerializeField] int shootDistance;

    [Header("----- Gun Components -----")]
    [SerializeField] GameObject gunModel; 
    [SerializeField] GameObject gunModelAimPos;
    [SerializeField] GameObject rifleModelAimPos;
    [SerializeField] GameObject smgModelAimPos;
    [SerializeField] GameObject sarModelAimPos;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject smgMuzzleFlashPOS;
    [SerializeField] GameObject rifleMuzzleFlashPOS;
    [SerializeField] GameObject sarMuzzleFlashPOS;

    public AudioSource audioSource;
    [SerializeField] AudioClip autoAudioClip;
    [SerializeField] AudioClip semiAudioClip;
    [SerializeField] AudioClip rifleAudioClip;

    Vector3 gunModelOrig;

    public int selectedGun;

    // Keybinds
    // Movement
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Reload
    public KeyCode reloadKey = KeyCode.R;

    // Health
    public int hpOrig;

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

    //Aiming bool
    private bool isAiming = false;

    // Start is called before the first frame update
    private void Start()
    {
        gunModelOrig = gunModel.transform.localPosition;

        reloadTutorial = true;
        hpOrig = hp;
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.activeMenu == null)
        {
            Movement();
            StateHandler();

            // Checks if player is aiming or not
            bool isAimButtonPressed = Input.GetButton("Aim");

            // if player is aiming then transform position of weapon, else put weapon pos back to original
            if (isAimButtonPressed && !isAiming)
            {
                Debug.Log("aimed");
                gunModel.transform.position = gunModelAimPos.transform.position;
                isAiming = true;
            }
            else if (!isAimButtonPressed && isAiming)
            {
                Debug.Log("un-aimed");
                gunModel.transform.localPosition = gunModelOrig;
                isAiming = false;
            }

            if (gunList.Count > 0)
            {
                ScrollGuns();

                // Switch between Automatic shooting and Semi Automatic shooting
                if (gunList[selectedGun].auto && Input.GetButton("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }
                else if (!gunList[selectedGun].auto && Input.GetButtonDown("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }

                //If ammo reaches 0 and mags are full display reload text
                if (reloadTutorial == true && gunList[selectedGun].ammoCurr <= 0 && gunList[selectedGun].ammoReserve > 0 && gameManager.instance.activeMenu == null)
                {
                    reloadTutorial = false;
                    StartCoroutine(gameManager.instance.outOfAmmo());
                }

                //reload
                if (Input.GetKeyDown(reloadKey))
                {
                    Debug.Log("reloaded");
                    reloadTutorial = false;
                    ReloadWeapon();

                }

                UpdatePlayerUI();
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
        hp -= amount;
        StartCoroutine(gameManager.instance.playerFlashDamage());
        UpdatePlayerUI();

        if (hp <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    // Updates the Player UI health
    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)hp / hpOrig;
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
                    Debug.Log("Error - playerController: SetMuzzlePOS()");
                    break;
            }



            UpdatePlayerUI();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {
                IDamage damagable = hit.collider.GetComponent<IDamage>();

                if (damagable != null && !hit.collider.CompareTag("Player"))
                {
                    damagable.TakeDamage(shootDamage);
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
        SetMuzzlePOS();

        gunModel.GetComponent<MeshFilter>().mesh = gunstat.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().material = gunstat.model.GetComponent<MeshRenderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;
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
        SetMuzzlePOS();
    }

    // Method for changing weapon stats
    public void ChangeGunStats()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        SetMuzzlePOS();
    
        gunModel.GetComponent<MeshFilter>().mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
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
            default:
                Debug.Log("Error - playerController: SetMuzzlePOS()");
                break;
        }
    }
}

