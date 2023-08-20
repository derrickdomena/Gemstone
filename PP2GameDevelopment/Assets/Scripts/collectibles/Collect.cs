using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour, ICollectible
{
    public enum CollectibleTypes {Ammo, Gem, HealthPack, CooldownE, CooldownQ, MaxHPUp, MaxSpeedUp, CritUp, DashUp, SpeedUp}

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
                GiveAmmo(ammoAmount);
                indicator.SetCollecibleText("");
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
            case 6:
                PlayerSpeedUp(walkSpeedUp, sprintSpeedUp);
                indicator.SetCollecibleText("Player Speed up");
                break;
            case 7:
                CritChanceUp(critChance, critPercent);
                indicator.SetCollecibleText("Crit up");
                break;
            case 8:
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
        gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].ammoReserve += amount;

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
        gameManager.instance.player.GetComponent<DashAbility>().UpdateCooldownDash(amount);
    }

    public void ReduceQCooldown(float amount)
    {
        gameManager.instance.player.GetComponent<FireballAbility>().UpdateCooldownGrenade(amount);
    }

    public void MaxHPUp(float increase, float heal)
    {
        float temp = gameManager.instance.playerScript.hpOrig * increase;
        gameManager.instance.playerScript.hpOrig = (int)temp;
        gameManager.instance.playerScript.hp += (int)(gameManager.instance.playerScript.hpOrig * heal);
        gameManager.instance.playerScript.UpdatePlayerUI();
    }

    public void PlayerSpeedUp(float speed, float sprint)
    {
        float tempSpd = gameManager.instance.playerScript.walkSpeed * speed;
        float tempSpt = gameManager.instance.playerScript.sprintSpeed * sprint;
        gameManager.instance.playerScript.walkSpeed = (int)tempSpd;
        gameManager.instance.playerScript.sprintSpeed = (int)tempSpt;
    }

    public void CritChanceUp(int chance, float critMul)
    {
        gameManager.instance.playerScript.critChance += chance;
        gameManager.instance.playerScript.critDam += gameManager.instance.playerScript.shootDamage * critMul;
    }

    public void DashUp(int uses, float time)
    {
        gameManager.instance.player.GetComponent<DashAbility>().IncreaseDashDistance(time);
        gameManager.instance.playerScript.dashCount += uses;
        gameManager.instance.player.GetComponent<DashAbility>().remainingDashes += uses;
    }

    public void SpeedUp(float speed, float sprint)
    {
        gameManager.instance.playerScript.walkSpeed += speed;
        gameManager.instance.playerScript.sprintSpeed += sprint;
    }

}
