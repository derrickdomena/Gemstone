//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MeleePickup : MonoBehaviour
//{
//    [SerializeField] WeaponStats melee;

//    void Start()
//    {
//        melee.attackDamage = melee.attackDamageOrig;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {        
//            gameManager.instance.playerScript.MeleePickup(melee);
//            Destroy(gameObject);
//        }
//    }
//}

