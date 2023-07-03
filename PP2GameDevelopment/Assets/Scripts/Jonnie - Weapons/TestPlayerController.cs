using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    // Track Player Stats, hp, movement, gravity, and max potential jumps.
    [SerializeField] int hp;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;

    //[Header("----- Gun Components -----")]
    // Weapon Slots
    //[SerializeField] GameObject weapon;

    //[SerializeField] float shootRate;
    //[SerializeField] int shootDamage;
    //[SerializeField] int shootDistance;

    //Keybinds
    public KeyCode sprintKey = KeyCode.LeftShift;

    Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed;
    int jumpCount;
    bool isShooting;
    int hpOrig;
    bool auto;

    // Start is called before the first frame update
    private void Start()
    {
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

    // Handles Movement for Player
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

        if (Input.GetButtonDown("Jump") && jumpCount < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            jumpCount++;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    //Manages the ability shooting
    IEnumerator Shoot()
    {
        isShooting = true;
        
        RaycastHit hit;

        //Takes in stats from the Weapon script tied to the instance of the weapon being used (Weapon.instance.stat_here)
        //This allows for dyanmic weapon choices rather than hard coding this into the player.

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, Weapon.instance.shootDistance))
        {
            IDamage damagable = hit.collider.GetComponent<IDamage>();
            if (damagable != null)
            {
                damagable.TakeDamage(Weapon.instance.shootDamage);
            }
        }

        yield return new WaitForSeconds(Weapon.instance.shootRate);
        //yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    // Apply Damage done by Enemies
    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            //UI Health
        }
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;

        hp = hpOrig;
    }

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

    private void StateHandler()
    {
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


