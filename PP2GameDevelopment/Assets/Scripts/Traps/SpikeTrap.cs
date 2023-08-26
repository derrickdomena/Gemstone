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
        trap.transform.position = new Vector3(trap.transform.position.x, trap.transform.position.y + 0.75f, trap.transform.position.z);
        yield return new WaitForSeconds(4);
        trap.transform.position = new Vector3(trap.transform.position.x, trap.transform.position.y - 0.75f, trap.transform.position.z);
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
