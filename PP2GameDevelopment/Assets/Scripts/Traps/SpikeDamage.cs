using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gameManager.instance.playerScript.TakeDamage((gameManager.instance.playerScript.hpMax / 10) * 2);
    }
}
