using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour, ICollectible
{
    public enum CollectibleTypes {Ammo, Gem, HealthPack, CooldownE, CooldownQ, MaxHPUp, CritUp, DashUp}

    public CollectibleTypes CollectibleType;

    public bool rotate;
    public float rotationSpeed;

    [SerializeField] public int ammoAmount;
    [SerializeField] public float healingAmount;
    [SerializeField] public int gemAmount;
    [SerializeField] public float cooldownQAmount;
    [SerializeField] public float cooldownEAmount;
    [SerializeField] public float maxHPAddition;
    [SerializeField] public int critChance;
    [SerializeField] public float critPercent;
    [SerializeField] public int addDashes;
    [SerializeField] public float dashTime;

    [SerializeField] CollectibleDrops dropItem;

    public GameObject collectText;
    public GameObject collectEffect;

    AudioManager audioManager;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        { 
            CollectItem();
            audioManager.PlaySFXInteractSounds(audioManager.pickUpSound);
        }

    }

    public void CollectItem()
    {
        if (collectEffect)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        CollectibleText indicator = Instantiate(collectText, transform.position, Quaternion.identity).GetComponent<CollectibleText>(); // need to modify where


        switch ((int)CollectibleType)
        {
            default:
            case 0:
                if (gameManager.instance.playerScript.weaponList[0].weaponType != "Gun")
                {
                    return;
                }
                else
                {
                    GiveAmmo(ammoAmount);
                    indicator.SetCollecibleText("");
                }
                break;
            case 1:
                GiveGem(gemAmount);
                indicator.SetCollecibleText("");
                break;
            case 2:
                if (gameManager.instance.playerScript.hp >= gameManager.instance.playerScript.hpOrig)
                {
                    indicator.SetCollecibleText("");
                    return;
                }
                else
                {
                    GiveHP((int)healingAmount);
                    indicator.SetCollecibleText("");
                }
                break;
            case 3:
                ReduceECooldown(cooldownEAmount);
                indicator.SetCollecibleText("Dash time down");
                break;
            case 4:
                ReduceQCooldown(cooldownQAmount);
                indicator.SetCollecibleText("Fireball time down");
                break;
            case 5:
                MaxHPUp(maxHPAddition, healingAmount);
                indicator.SetCollecibleText("Max HP up");
                break;
            //case 6:
            //    PlayerSpeedUp(walkSpeedUp, sprintSpeedUp);
            //    indicator.SetCollecibleText("Player Speed up");
            //    break;
            case 6:
                CritChanceUp(critChance, critPercent);
                indicator.SetCollecibleText("Crit up");
                break;
            case 7:
                DashUp(addDashes, dashTime);
                indicator.SetCollecibleText("Dash up");

                break;
        }
        Destroy(gameObject);
    }
    // When ammo pack is picked up, increases ammoReserve
    public void GiveAmmo(int amount)
    {
        //IMPORTANT: kinda... This doesnt seem efficient.. is there a better way to increase that number?
        gameManager.instance.playerScript.weaponList[0].ammoReserve += (int)amount;
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    // Receive health when picking up a health pack
    public void GiveHP(int amount)
    {
        //gameManager.instance.playerScript.hpOrig += amount;
        if (gameManager.instance.playerScript.hp + amount > gameManager.instance.playerScript.hpOrig)
        {
            gameManager.instance.playerScript.hp = gameManager.instance.playerScript.hpOrig;
        }
        else
        {
            gameManager.instance.playerScript.hp += amount;

        }
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    //give player a gem ammount
    public void GiveGem(int amount)
    {
        gameManager.instance.updateGemCount(amount);
    }

    public void ReduceECooldown(float amount)
    {
        if(gameManager.instance.playerScript.dashCooldown > gameManager.instance.playerScript.dashCooldownMin)
        {
            gameManager.instance.player.GetComponent<DashAbility>().UpdateCooldownDash(amount);
        }
        else if(gameManager.instance.playerScript.dashCooldown <= gameManager.instance.playerScript.dashCooldownMin)
        {
            gameManager.instance.playerScript.dashCooldown = gameManager.instance.playerScript.dashCooldownMin;
        }
    }

    public void ReduceQCooldown(float amount)
    {
        if(gameManager.instance.playerScript.fireballCooldown > gameManager.instance.playerScript.fireballCooldownMin)
        {
            gameManager.instance.player.GetComponent<FireballAbility>().UpdateCooldownFireball(amount);
        }
        else if(gameManager.instance.playerScript.fireballCooldown <= gameManager.instance.playerScript.fireballCooldownMin)
        {
            gameManager.instance.playerScript.fireballCooldown = gameManager.instance.playerScript.fireballCooldownMin;
        }
    }

    public void MaxHPUp(float increase, float heal)
    {
        float temp = gameManager.instance.playerScript.hpOrig * increase;
        gameManager.instance.playerScript.hpOrig = (int)temp;
        gameManager.instance.playerScript.hp += (int)(gameManager.instance.playerScript.hpOrig * heal);
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    public void CritChanceUp(int chance, float critMul)
    {
        gameManager.instance.playerScript.critChance += chance;
        gameManager.instance.playerScript.critDam += gameManager.instance.playerScript.shootDamage * critMul;
    }

    public void DashUp(int uses, float time)
    {
        gameManager.instance.player.GetComponent<DashAbility>().IncreaseDashDistance(time);
        if (gameManager.instance.playerScript.dashCount >= gameManager.instance.playerScript.dashCountMax)
        {
            gameManager.instance.playerScript.dashCount = gameManager.instance.playerScript.dashCountMax;
        }
        else
        {
            gameManager.instance.playerScript.dashCount += (int)uses;
            gameManager.instance.player.GetComponent<DashAbility>().remainingDashes += (int)uses;
        }
    }
}
