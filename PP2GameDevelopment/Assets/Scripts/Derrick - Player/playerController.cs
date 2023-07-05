using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    // Health
    [SerializeField] int hp;
    // Movement
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;

    // Keybinds
    // Movement
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Reload
    public KeyCode reloadKey = KeyCode.R;

    Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);
    int jumpCount;
    bool isShooting;
    int hpOrig;
    bool auto;

    bool reloadTutorial;

    // Movement
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    // Start is called before the first frame update
    private void Start()
    {
        reloadTutorial = true;
        hpOrig = hp;
        SpawnPlayer();
        auto = Weapon.instance.automatic;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.activeMenu == null)
        {
            Movement();
            StateHandler();

            // Check if player has ammo
            if (Weapon.instance.ammo > 0)
            {
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

        }

        if (Input.GetKeyDown(reloadKey))
        {
            Weapon.instance.ReloadWeapon();
        }

        // If ammo reaches 0 and mags are full display reload text
        if (reloadTutorial == true && Weapon.instance.ammo <= 0 && Weapon.instance.magazines > 0 )
        {
            StartCoroutine(gameManager.instance.outOfAmmo());
            reloadTutorial = false;
        }
    }

    // Movement
    void Movement()
    {
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
        if (Input.GetKeyDown(jumpKey) || Input.GetKey(jumpKey) && jumpCount < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            jumpCount++;
        }

        // Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Manages Shooting
    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;

        // Decrease ammo count when shooting
        if (Weapon.instance.ammo > 0)
        {
            Weapon.instance.ammoUpdate();
        }

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, Weapon.instance.shootDistance))
        {
            IDamage damagable = hit.collider.GetComponent<IDamage>();
            if (damagable != null)
            {
                damagable.TakeDamage(Weapon.instance.shootDamage);
            }
        }

        yield return new WaitForSeconds(Weapon.instance.shootRate);
        isShooting = false;

    }


    // When ammo pack is picked up, increases magazine
    public void MoreAmmo(int amount)
    {
        Weapon.instance.magazines += 1;
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

    // Receive health when picking up a health pack
    public void Heal(int amount)
    {
        //hp += amount; Future use, when health packs vary in healing ammount.
        hp = hpOrig;
        UpdatePlayerUI();

    }

    // Updates the Player UI health
    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)hp / hpOrig;
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;

        hp = hpOrig;
    }

    // Sets the movement speed for each movement state
    private void StateHandler()
    {
        // Movement - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            playerSpeed = walkSpeed / 2;
        }
        // Movement - Running
        if (groundedPlayer && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            playerSpeed = sprintSpeed;
        }
        // Movement - Walking
        else if (groundedPlayer)
        {
            state = MovementState.walking;
            playerSpeed = walkSpeed;
        }
        // Movement - Air
        else
        {
            state = MovementState.air;
        }
    }
}

