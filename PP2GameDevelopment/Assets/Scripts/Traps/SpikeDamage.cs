using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gameManager.instance.playerScript.TakeDamage(2);

    }
}
