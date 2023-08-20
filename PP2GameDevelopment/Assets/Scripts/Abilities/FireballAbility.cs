using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAbility : MonoBehaviour
{
    [Header("----- Fireball Components -----")]
    public GameObject fireballProjectile;
    public Transform fireballPosition;

    [Header("----- Fireball Stats -----")]
    public float fireballSpeed;

    KeyCode fireballKey = KeyCode.Q;
    public bool canThrow;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.grenadeCooldownFill.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }
        UpdateGrenadeUI();
    }

    void ThrowFireball()
    {
        canThrow = false;

        // Instantiae fireball object
        GameObject fireballObj = Instantiate(fireballProjectile, fireballPosition.transform.position, fireballPosition.transform.rotation);

        audioManager.PlaySFXAbilities(audioManager.fireballSound);

        // Get rigidbody component from fireball
        Rigidbody rb = fireballObj.GetComponent<Rigidbody>();

        // Add throwing force
        Vector3 forwardForce = GameObject.FindGameObjectWithTag("MainCamera").transform.forward * fireballSpeed;
        rb.AddForce(forwardForce, ForceMode.Impulse);
    }

    // Updates the Fireball UI
    void UpdateGrenadeUI()
    {
        // When grenadeKey is pressed and canThrow is true, you can throw a grenade
        // also checks to see if the game is paused
        if (Input.GetKeyDown(fireballKey) && !canThrow && Time.timeScale != 0)
        {
            //change function to fireballCooldown
            gameManager.instance.grenadeCooldownFill.fillAmount = 0;
            ThrowFireball();
            canThrow = true;
        }

        // When canThrow is true, start incrementing the grenade ability image fill amount
        if (canThrow)
        {
            //change function to fireballCooldown
            gameManager.instance.grenadeCooldownFill.fillAmount += 1 / gameManager.instance.playerScript.grenadeCooldown * Time.deltaTime;

            // When image fill amount is equal to one, set canThrow to false
            //change function to fireballCooldown
            if (gameManager.instance.grenadeCooldownFill.fillAmount == 1)
            {
                canThrow = false;
            }
        }
    }

    public void UpdateCooldownGrenade(float time)
    {
        //change function to fireballCooldown
        gameManager.instance.playerScript.grenadeCooldown = gameManager.instance.playerScript.grenadeCooldown - time;
        canThrow = true;
        UpdateGrenadeUI();
    }
}
