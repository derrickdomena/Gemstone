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
            gameManager.instance.isPoisoned = true;
        }
    }
}
