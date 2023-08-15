using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossRoomMessage : MonoBehaviour
{
    [SerializeField] GameObject Text;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Text.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Text.SetActive(false);
        }
    }
}
