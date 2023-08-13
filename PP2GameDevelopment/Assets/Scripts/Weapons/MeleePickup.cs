using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePickup : MonoBehaviour
{
    [SerializeField] MeleeStats melee;

    void Start()
    {
        melee.attackDamage = melee.attackDamageOrig;
        melee.attackSpeed = melee.attackSpeedOrig;
        melee.attackDelay = melee.attackDelayOrig;
        melee.attackDistance = melee.attackDistanceOrig;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {        
            gameManager.instance.playerScript.MeleePickup(melee);
            Destroy(gameObject);
        }
    }
}

