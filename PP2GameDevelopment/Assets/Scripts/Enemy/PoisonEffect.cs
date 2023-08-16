using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!gameManager.instance.playerScript.isPoisoned) 
            {
                gameManager.instance.playerScript.poisonEffectDuration = gameManager.instance.playerScript.poisonDurOrig;
                gameManager.instance.playerScript.poisonTimer = 0;
                gameManager.instance.playerScript.isPoisoned = true;
            }
        }
    }
}
