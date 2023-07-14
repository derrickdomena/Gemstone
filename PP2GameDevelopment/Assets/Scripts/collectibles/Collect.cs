using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour, ICollectible
{
    public enum CollectibleTypes { Ammo, Gem, HealthPack}

    public CollectibleTypes CollectibleType;

    public bool rotate;
    public float rotationSpeed;

    [SerializeField] public int ammoAmount;
    [SerializeField] public int healingAmount ;
    [SerializeField] public int gemAmount;

    public GameObject collectEffect;

    // Update is called once per frame
    void Update()
    {
        if (rotate)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CollectItem();
        }
        
    }

    public void CollectItem()
    {
        if (collectEffect)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        if (CollectibleType == CollectibleTypes.Gem)
        {
            GiveGem(gemAmount);
        }
        if (CollectibleType == CollectibleTypes.HealthPack)
        {
            //checks to see if the player already has full hp
            if (gameManager.instance.playerScript.hp >= gameManager.instance.playerScript.hpOrig)
            {
                return;
            }
            GiveHP(healingAmount);
        }
        if(CollectibleType == CollectibleTypes.Ammo)
        {
            GiveAmmo(ammoAmount);
        }
        Destroy(gameObject);
    }
    // When ammo pack is picked up, increases ammoReserve
    public void GiveAmmo(int amount)
    {
        //IMPORTANT: kinda... This doesnt seem efficient.. is there a better way to increase that number?
        gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].ammoReserve += amount;
    }

    // Receive health when picking up a health pack
    public void GiveHP(int amount)
    {
        //hp += amount; Future use, when health packs vary in healing ammount.
        gameManager.instance.playerScript.hp = amount;
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    //give player a gem ammount
    public void GiveGem(int amount)
    {
        gameManager.instance.updateGemCount(amount);
    }    
}
