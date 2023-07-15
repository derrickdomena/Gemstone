using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerup/Damage")]
public class DamageBuff : PowerupEffect
{

    public override void Apply()
    {
        //gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].shootDamage += (int)amount;
        //gameManager.instance.playerScript.ChangeGunStats();
    }
}
