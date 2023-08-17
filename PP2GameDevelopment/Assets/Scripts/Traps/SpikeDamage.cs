using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<playerController>(out playerController player))
        {
            player.TakeDamage(gameManager.instance.playerScript.hpOrig / 10 * 2);
        }
        if (other.TryGetComponent<EnemyAI>(out EnemyAI comp))
        {
            comp.TakeDamage(comp.hp / 10 * 2);
        }
        if (other.TryGetComponent<EnemyCasterAI>(out EnemyCasterAI casterAI))
        {
            casterAI.TakeDamage(casterAI.hp / 10 * 2);
        }
    }
}
