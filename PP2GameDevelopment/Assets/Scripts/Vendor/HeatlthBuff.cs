using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealthBuff")]
public class HeatlthBuff : PowerupEffect
{
    public int amount;
    public override void Apply()
    {
       //gameManager.instance.playerScript.hpOrig += (int)amount;
       //gameManager.instance.playerScript.hp += (int)amount;

    }
}
