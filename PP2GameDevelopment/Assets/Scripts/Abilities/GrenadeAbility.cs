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
    [SerializeField] float throwCooldownTime;

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
            ThrowGrenade();
            canThrow = true;          
        }

        // When canThrow is true, start decrementing the grenade ability image fill amount
        if (canThrow)
        {
            gameManager.instance.grenadeCooldownFill.fillAmount -= 1 / throwCooldownTime * Time.deltaTime;

            // When image fill amount is less than or equal zero, refill the grenade ability and set canThrow to false
            if (gameManager.instance.grenadeCooldownFill.fillAmount <= 0)
            {
                gameManager.instance.grenadeCooldownFill.fillAmount = 1;
                canThrow = false;
            }
        }
    }
}
