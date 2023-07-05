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

    //Keybinds
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
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
            ReloadWeapon();

            if (Input.GetButton("Shoot") && !isShooting)
            {
                StartCoroutine(Shoot());
            }
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

    //Manages the ability shooting
    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;

        if (Weapon.instance.ammo > 0)
        {
            Weapon.instance.ammo -= 1;
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

    // Handles Weapon Reload and magazine size
    public void ReloadWeapon()
    {
        // Only Reaload's Weapon if ammo is less than initial ammoCount and not less than zero.
        // Reloads when the reloadKey is press
        if (Weapon.instance.ammo == 0 && Weapon.instance.ammo <= Weapon.instance.ammoCount && Input.GetKey(reloadKey))
        {
            // Reduces magazine size until it hits zero.
            if (Weapon.instance.magazines >= 1)
            {
                Weapon.instance.magazines -= 1;
                Weapon.instance.ammo = Weapon.instance.ammoCount;
            }
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

