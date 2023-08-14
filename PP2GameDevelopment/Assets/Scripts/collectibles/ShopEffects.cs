using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEffects : ShopCollectibles
{
    public string itemName;
    public int cost;
    [SerializeField] public float amount1;
    [SerializeField] public float amount2;
    public Collect.CollectibleTypes collectibleTypes;

    public override void Apply()
    {
        switch (collectibleTypes)
        {
            default:
            case Collect.CollectibleTypes.Ammo:
                gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].ammoReserve += (int)amount1;
                break;
            case Collect.CollectibleTypes.HealthPack:
                if (gameManager.instance.playerScript.hp + amount1 > gameManager.instance.playerScript.hpOrig)
                {
                    gameManager.instance.playerScript.hp = gameManager.instance.playerScript.hpOrig;
                }
                else
                {
                    gameManager.instance.playerScript.hp += (int)amount1;

                }
                gameManager.instance.playerScript.UpdatePlayerUI();
                break;
            case Collect.CollectibleTypes.CooldownE:
                gameManager.instance.player.GetComponent<DashAbility>().UpdateCooldownDash(amount1);
                break;
            case Collect.CollectibleTypes.CooldownQ:
                gameManager.instance.player.GetComponent<FireballAbility>().UpdateCooldownGrenade(amount1);
                break;
            case Collect.CollectibleTypes.MaxHPUp:
                float temp = gameManager.instance.playerScript.hpOrig * amount1;
                gameManager.instance.playerScript.hpOrig = (int)temp;
                gameManager.instance.playerScript.hp += (int)(gameManager.instance.playerScript.hpOrig * amount2);
                gameManager.instance.playerScript.UpdatePlayerUI();
                break;
            case Collect.CollectibleTypes.MaxSpeedUp:
                float tempSpd = gameManager.instance.playerScript.walkSpeed * amount1;
                float tempSpt = gameManager.instance.playerScript.sprintSpeed * amount2;
                gameManager.instance.playerScript.walkSpeed = (int)tempSpd;
                gameManager.instance.playerScript.sprintSpeed = (int)tempSpt;
                break;
            case Collect.CollectibleTypes.CritUp:
                gameManager.instance.playerScript.critChance += amount1;
                gameManager.instance.playerScript.critDam += gameManager.instance.playerScript.shootDamage * amount2;
                break;
            case Collect.CollectibleTypes.DashUp:
                gameManager.instance.player.GetComponent<DashAbility>().IncreaseDashDistance(amount2);
                gameManager.instance.playerScript.dashCount += (int)amount1;
                gameManager.instance.player.GetComponent<DashAbility>().remainingDashes += (int)amount1;
                break;

        }
    }
}
