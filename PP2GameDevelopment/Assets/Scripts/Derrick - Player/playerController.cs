using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    // Character Controller
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    // Track Player Stats, hp, movement, gravity, and max potential jumps.
    [SerializeField] float hp;
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;

    [Header("----- Gun Components -----")]
    // Weapon Slots
    [SerializeField] GameObject weapon;
    [SerializeField] Transform weaponPOS;



    Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    int jumpCount;
    bool isShooting;

    // Start is called before the first frame update
    private void Start()
    {
        Instantiate(weapon, weaponPOS.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
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

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            jumpCount++;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Manages the ability shooting
    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;

        Instantiate(Weapon.instance.bullet, Weapon.instance.shootingPOS.position, transform.rotation);

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, Weapon.instance.shootDistance))
        {
            IDamage damagable = hit.collider.GetComponent<IDamage>();
            if (damagable != null)
            {
                damagable.TakeDamage(weapon.gameObject.GetInstanceID());
            }
        }

        yield return new WaitForSeconds(Weapon.instance.shootRate);
        isShooting = false;
    }
    // Apply Damage done by Enemies
    public void TakeDamage(int amount)
    {
        hp -= amount;
    }
}

