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

    bool readyToThrow;

    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(grenadeKey) && readyToThrow)
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        readyToThrow = false;

        // Instantiae grenade object
        GameObject grenadeObj = Instantiate(grenade, grenadePosition.transform.position, grenadePosition.transform.rotation);

        // Get rigidbody component from grenade
        Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();

        // Add throwing force
        Vector3 upwardForce = transform.forward * throwForce + transform.up * throwUpwardForce;
        rb.AddForce(upwardForce, ForceMode.Impulse);

        // Throw Cooldown
        Invoke(nameof(ResetThrow), throwCooldownTime);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}
