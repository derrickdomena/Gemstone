using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour, ICollectible
{
    public enum CollectibleTypes { Ammo, Gem, HealthPack, CooldownE, CooldownQ, MaxHPUp, MaxSpeedUp, CritUp, DashUp}

    public CollectibleTypes CollectibleType;

    public bool rotate;
    public float rotationSpeed;

    [SerializeField] public int ammoAmount;
    [SerializeField] public float healingAmount;
    [SerializeField] public int gemAmount;
    [SerializeField] public float cooldownQAmount;
    [SerializeField] public float cooldownEAmount;
    [SerializeField] public float maxHPAddition;
    [SerializeField] public float walkSpeedUp;
    [SerializeField] public float sprintSpeedUp;
    [SerializeField] public float critChance;
    [SerializeField] public float critDam;
    [SerializeField] public int addDashes;
    [SerializeField] public float dashTime;

    [SerializeField] CollectibleDrops dropItem;

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

            #region old code
            //if (CollectibleType == CollectibleTypes.Gem)
            //{
            //    GiveGem(gemAmount);
            //}
            //if (CollectibleType == CollectibleTypes.HealthPack)
            //{
            //    //checks to see if the player already has full hp
            //    if (gameManager.instance.playerScript.hp >= gameManager.instance.playerScript.hpOrig)
            //    {
            //        return;
            //    }
            //    GiveHP(healingAmount);
            //}
            //if(CollectibleType == CollectibleTypes.Ammo)
            //{
            //    GiveAmmo(ammoAmount);
            //}
            #endregion
        switch ((int)CollectibleType)
        {
            default:
            case 0:
                GiveAmmo(ammoAmount);
                break;
            case 1:
                GiveGem(gemAmount);
                break;
            case 2:
                if (gameManager.instance.playerScript.hp >= gameManager.instance.playerScript.hpOrig)
                {
                    return;
                }
                else
                {
                    GiveHP((int)healingAmount);
                }
                break;
            case 3:
                ReduceECooldown(cooldownEAmount);
                break;
            case 4:
                ReduceQCooldown(cooldownQAmount);
                break;
            case 5:
                MaxHPUp(maxHPAddition, healingAmount);
                break;
            case 6:
                PlayerSpeedUp(walkSpeedUp, sprintSpeedUp); 
                break;
            case 7:
                CritChanceUp(critChance, critDam);
                break;
            case 8:
                DashUp(addDashes, dashTime);
                break;
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
        gameManager.instance.playerScript.hp += amount;
        gameManager.instance.playerScript.hpOrig += amount;       
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    //give player a gem ammount
    public void GiveGem(int amount)
    {
        gameManager.instance.updateGemCount(amount);
    }

    public void ReduceECooldown(float amount)
    {
        gameManager.instance.player.GetComponent<DashAbility>().UpdateCooldownDash(amount);
    }

    public void ReduceQCooldown(float amount)
    {
        gameManager.instance.player.GetComponent<FireballAbility>().UpdateCooldownGrenade(amount);
    }

    public void MaxHPUp(float increase, float heal)
    {
        float temp = gameManager.instance.playerScript.hpMax * increase;
        gameManager.instance.playerScript.hpMax = (int)temp;
        gameManager.instance.playerScript.hp += (int)(gameManager.instance.playerScript.hpMax * heal);
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    public void PlayerSpeedUp(float speed, float sprint)
    {
        float tempSpd = gameManager.instance.playerScript.walkSpeed * speed;
        float tempSpt = gameManager.instance.playerScript.sprintSpeed * sprint;
        gameManager.instance.playerScript.walkSpeed = (int)tempSpd;
        gameManager.instance.playerScript.sprintSpeed = (int)tempSpt;
    }

    public void CritChanceUp(float chance, float dam)
    {
        gameManager.instance.playerScript.critChance += chance;
        float tempDam = Mathf.Pow(gameManager.instance.playerScript.shootDamageOrig, (1 + dam));
        gameManager.instance.playerScript.shootDamage = (int)tempDam;

    }

    public void DashUp(int uses, float time)
    {
        gameManager.instance.player.GetComponent<DashAbility>().IncreaseDashDistance(time);
        gameManager.instance.playerScript.dashCount += uses;
        gameManager.instance.player.GetComponent<DashAbility>().remainingDashes += uses;
    }
}
