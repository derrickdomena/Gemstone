using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs")]
public class Buffs : PowerupEffect
{
    public string itemName;
    public int cost;
    [SerializeField] public float amount;
    public Items.ItemType itemType;
    
    public override void Apply()
    {
        switch(itemType)
        {
            default:
            case Items.ItemType.Damage:
                
                gameManager.instance.playerScript.weaponList[gameManager.instance.playerScript.selectedWeapon].shootDamage += (int)amount;
                gameManager.instance.playerScript.ChangeWeaponStats();
                break;
            case Items.ItemType.Health:
                gameManager.instance.playerScript.hpOrig += (int)amount;
                gameManager.instance.playerScript.hp += (int) amount;
                break;
            case Items.ItemType.Speed:
                gameManager.instance.playerScript.walkSpeed += amount;
                break;
        }
    }
}
