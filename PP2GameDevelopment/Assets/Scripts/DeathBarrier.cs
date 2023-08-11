using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    [SerializeField] GameObject respawnPos;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.anim.SetTrigger("FadeOut");
            gameManager.instance.playerScript.TakeDamage(2);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        gameManager.instance.statePaused();
        other.transform.position = respawnPos.transform.position;
    }
}
