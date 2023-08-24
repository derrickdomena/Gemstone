using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] WeaponStats weapon;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        //Gun
        weapon.shootDamage = weapon.shootDamageOrig;
        weapon.ammoCurr = weapon.ammoMax;
        weapon.ammoReserve = weapon.ammoReserveMax;
        //Melee
        weapon.attackDamage = weapon.attackDamageOrig;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Picked up a weapon");
            if(weapon.weaponType == "Gun")
            {
                audioManager.PlaySFXInteractSounds(audioManager.gunPickUpSound);
            }
            else if (weapon.weaponType == "Melee")
            {
                audioManager.PlaySFXInteractSounds(audioManager.meleePickUpSound);
            }
            
            gameManager.instance.playerScript.WeaponPickup(weapon);
            Destroy(gameObject);
        }
    }

}
