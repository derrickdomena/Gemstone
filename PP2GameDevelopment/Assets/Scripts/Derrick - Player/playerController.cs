using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] CharacterController controller;
    [SerializeField] CapsuleCollider capsuleCollider;

    [Header("----- Player Stats -----")]
    // Health
    [SerializeField] public int hp;
    // Movement
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;

    [Header("----- Gun Stats -----")]
    public List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    // Keybinds
    // Movement
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Reload
    public KeyCode reloadKey = KeyCode.R;

    // Health
    int hpOrig;

    // Movement
    Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed;
    int jumpCount;

    // Shooting
    bool isShooting;

    bool auto;

    bool reloadTutorial;

    public int selectedGun;

    // Start is called before the first frame update
    private void Start()
    {
        reloadTutorial = true;
        hpOrig = hp;
        SpawnPlayer();
        auto = gunList[selectedGun].auto;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager.instance.activeMenu == null)
        {
            Movement();
            StateHandler();
            
            if (gunList.Count > 0)
            {
                ScrollGuns();

                // Switch between Automatic shooting and Semi Automatic shooting
                if (auto && Input.GetButton("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }
                else if (!auto && Input.GetButtonDown("Shoot") && !isShooting)
                {
                    StartCoroutine(Shoot());
                }
            }

            // Reload Ammo
            if (Input.GetKeyDown(reloadKey))
            {
                gunList[selectedGun].ammoCurr = gunList[selectedGun].ammoMax;
            }



            gameManager.instance.ammo.text = gunList[selectedGun].ammoCurr.ToString("F0");
            // If ammo reaches 0 and mags are full display reload text
            //if (reloadTutorial == true && gunList[selectedGun].ammoCurr <= 0 && Weapon.instance.magazines > 0 && gameManager.instance.activeMenu == null)
            //{
            //    reloadTutorial = false;
            //    StartCoroutine(gameManager.instance.outOfAmmo());

            //}
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
            // Main Camera Position
            Camera.main.transform.localPosition = new Vector3(0f, 0.375f, 0f);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            // Character Controller Height
            controller.height = controller.height * 2;
            // Capsule Collider Height
            capsuleCollider.height = capsuleCollider.height * 2;
            // Main Camera Position
            Camera.main.transform.localPosition = new Vector3(0f, 0.75f, 0f);
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
        if (groundedPlayer && Input.GetKey(sprintKey))
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

            gunList[selectedGun].ammoCurr--;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {
                IDamage damagable = hit.collider.GetComponent<IDamage>();

                if (damagable != null && !hit.collider.CompareTag("Player"))
                {
                    damagable.TakeDamage(shootDamage);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }
    public void GunPickup(GunStats gunstat)
    {
        gunList.Add(gunstat);

        shootDamage = gunstat.shootDamage;
        shootDistance = gunstat.shootDist;
        shootRate = gunstat.shootRate;

        gunModel.GetComponent<MeshFilter>().mesh = gunstat.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().material = gunstat.model.GetComponent<MeshRenderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;

    }

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
    void ChangeGunStats()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
<<<<<<< Updated upstream
=======

    // When ammo pack is picked up, increases magazine
    public void MoreAmmo(int amount)
    {
        //Weapon.instance.magazines += 1;
    }
>>>>>>> Stashed changes
}

