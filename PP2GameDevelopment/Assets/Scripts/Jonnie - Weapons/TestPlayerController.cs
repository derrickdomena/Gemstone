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

    [Header("----- Gun Stats -----")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    //Keybinds
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode reloadAmmo = KeyCode.R;

    Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed;
    int jumpCount;
    bool isShooting;
    int hpOrig;
    bool auto;
    public int selectedGun;

    // Start is called before the first frame update
    private void Start()
    {
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
        if(Input.GetKeyDown(reloadAmmo))
        {
            gunList[selectedGun].ammoCurr = gunList[selectedGun].ammoMax;
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
}


