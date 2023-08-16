using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject door2;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            door.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            door2.SetActive(true);
        }
    }
}
