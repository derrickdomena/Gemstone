using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAbility : MonoBehaviour
{
    [Header("----- Throw Components -----")]
    [SerializeField] GameObject grenade;
    [SerializeField] Transform grenadePosition;

    [Header("----- Throw Stats -----")]
    [SerializeField] float throwForce;
    [SerializeField] float throwUpwardForce;
    //[SerializeField] public float throwCooldownTime;

    public KeyCode grenadeKey = KeyCode.Q;
    bool canThrow;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.grenadeCooldownFill.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrenadeUI();
    }

    // Throws the grenade
    void ThrowGrenade()
    {
        canThrow = false;

        // Instantiae grenade object
        GameObject grenadeObj = Instantiate(grenade, grenadePosition.transform.position, grenadePosition.transform.rotation);

        // Get rigidbody component from grenade
        Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();

        // Add throwing force
        Vector3 upwardForce = transform.forward * throwForce + transform.up * throwUpwardForce;
        rb.AddForce(upwardForce, ForceMode.Impulse);
    }

    // Updates the Grenade UI
    void UpdateGrenadeUI()
    {
        // When grenadeKey is pressed and canThrow is true, you can throw a grenade
        if (Input.GetKeyDown(grenadeKey) && !canThrow)
        {
            gameManager.instance.grenadeCooldownFill.fillAmount = 0;
            ThrowGrenade();
            canThrow = true;          
        }

        // When canThrow is true, start incrementing the grenade ability image fill amount
        if (canThrow)
        {
            gameManager.instance.grenadeCooldownFill.fillAmount += 1 / gameManager.instance.playerScript.grenadeCooldown * Time.deltaTime;

            // When image fill amount is equal to one, set canThrow to false
            if (gameManager.instance.grenadeCooldownFill.fillAmount == 1)
            {          
                canThrow = false;
            }
        }
    }

    public void UpdateCooldownGrenade(float time)
    {
        gameManager.instance.playerScript.grenadeCooldown = gameManager.instance.playerScript.grenadeCooldown - time;
        canThrow = true;
        UpdateGrenadeUI();
    }
}
