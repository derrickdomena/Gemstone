using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] public GameObject trap;
    public bool isSprung = false;
    
    private IEnumerator SpikeRoutine()
    {
        yield return new WaitForSeconds(1);
        trap.transform.Translate(Vector3.up * Time.deltaTime * 100, Space.World);
        yield return new WaitForSeconds(4);
        trap.transform.Translate(Vector3.down * Time.deltaTime * 100, Space.World);
        isSprung = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSprung)
        {
            isSprung=true;
            StartCoroutine(SpikeRoutine());
        }
    }
}
