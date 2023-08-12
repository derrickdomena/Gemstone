using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] public GameObject trap;
    private bool isSprung = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSprung)
        {
            trap.transform.Translate(Vector3.up * Time.deltaTime * 25, Space.World);
            isSprung = true;
        }
    }
}
