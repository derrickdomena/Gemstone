using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunStats gun;

    void Start()
    {
        gun.shootDamage = gun.shootDamageOrig;
        gun.ammoCurr = gun.ammoMax;
        gun.ammoReserve = gun.ammoReserveMax;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Picked up a weapon");
            gameManager.instance.playerScript.GunPickup(gun);
            Destroy(gameObject);
        }
    }

}
