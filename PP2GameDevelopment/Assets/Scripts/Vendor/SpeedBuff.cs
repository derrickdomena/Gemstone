using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]
public class SpeedBuff : PowerupEffect
{
    public override void Apply(float amount)
    {
        gameManager.instance.playerScript.walkSpeed += amount;
    }
}
