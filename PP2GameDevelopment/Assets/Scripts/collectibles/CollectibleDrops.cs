using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using static Collect;

[CreateAssetMenu(menuName = "Drops")] 
public class CollectibleDrops : ShopCollectibles
{
    public Collect.CollectibleTypes drops;
    public string itemName;
    public int numberOfPrefabsToCreate;
    public int cost;
    [SerializeField] public float amount1;
    [SerializeField] public float amount2;

    public override void Apply()
    {
        switch (drops)
        {
            default:
            case Collect.CollectibleTypes.Ammo:
                //gameManager.instance.playerScript.weaponList[gameManager.instance.playerScript.selectedWeapon].ammoReserve += (int)amount1;
                gameManager.instance.playerScript.weaponList[0].ammoReserve += (int)amount1;
                gameManager.instance.playerScript.UpdatePlayerUI();
                break;
            case Collect.CollectibleTypes.HealthPack:
                if (gameManager.instance.playerScript.hp < gameManager.instance.playerScript.hpOrig)
                {
                    gameManager.instance.playerScript.hp = gameManager.instance.playerScript.hpOrig;
                }
                else
                {
                    return;

                }
                gameManager.instance.playerScript.UpdatePlayerUI();
                break;
            case Collect.CollectibleTypes.CooldownE:
                if (gameManager.instance.playerScript.dashCooldown > gameManager.instance.playerScript.dashCooldownMin)
                {
                    gameManager.instance.player.GetComponent<DashAbility>().UpdateCooldownDash(amount1);
                }
                else if (gameManager.instance.playerScript.dashCooldown <= gameManager.instance.playerScript.dashCooldownMin)
                {
                    gameManager.instance.playerScript.dashCooldown = gameManager.instance.playerScript.dashCooldownMin;
                }
                break;
            case Collect.CollectibleTypes.CooldownQ:
                if (gameManager.instance.playerScript.fireballCooldown > gameManager.instance.playerScript.fireballCooldownMin)
                {
                    gameManager.instance.player.GetComponent<FireballAbility>().UpdateCooldownFireball(amount1);
                }
                else if (gameManager.instance.playerScript.fireballCooldown <= gameManager.instance.playerScript.fireballCooldownMin)
                {
                    gameManager.instance.playerScript.fireballCooldown = gameManager.instance.playerScript.fireballCooldownMin;
                }
                break;
            case Collect.CollectibleTypes.MaxHPUp:
                gameManager.instance.playerScript.hpOrig += (int)amount1;
                //gameManager.instance.playerScript.hp = gameManager.instance.playerScript.hpOrig;
                gameManager.instance.playerScript.UpdatePlayerUI();
                break;
            case Collect.CollectibleTypes.CritUp:
                gameManager.instance.playerScript.critChance += amount1;
                gameManager.instance.playerScript.critDam += gameManager.instance.playerScript.shootDamage * amount2;
                break;
            case Collect.CollectibleTypes.DashUp:
                gameManager.instance.player.GetComponent<DashAbility>().IncreaseDashDistance(amount2);
                if (gameManager.instance.playerScript.dashCount >= gameManager.instance.playerScript.dashCountMax)
                {
                    gameManager.instance.playerScript.dashCount = gameManager.instance.playerScript.dashCountMax;
                }
                else
                {
                    gameManager.instance.playerScript.dashCount += (int)amount1;
                    gameManager.instance.player.GetComponent<DashAbility>().remainingDashes += (int)amount1;
                }
                break;

        }
    }

    public Collect.CollectibleTypes Drop(int select)
    {
        switch (select)
        {
            default:
            case 0:
                drops = Collect.CollectibleTypes.Ammo;
                break;
            case 1:
                drops = Collect.CollectibleTypes.Gem;
                break;
            case 2:
                drops = Collect.CollectibleTypes.HealthPack;
                break;
            case 3:
                drops = Collect.CollectibleTypes.CooldownE;
                break;
            case 4:
                drops = Collect.CollectibleTypes.CooldownQ;
                break;
            case 5:
                drops = Collect.CollectibleTypes.MaxHPUp;
                break;
            case 6:
                drops = Collect.CollectibleTypes.CritUp;
                break;
            case 7:
                drops = Collect.CollectibleTypes.DashUp;
                break;
        }
        return drops;
    }
}
