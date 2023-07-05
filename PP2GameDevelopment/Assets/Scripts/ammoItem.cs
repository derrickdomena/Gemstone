using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoItem : MonoBehaviour
{
    int magAmount;

    // Update rotates the X, Y axis of the iteam
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime, 50f * Time.deltaTime, 0, Space.Self);
    }

    // When item collision occurs with Player, the item gets destroyed and magAmmount gets passed as an ammount to MoreAmmo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<playerController>().MoreAmmo(magAmount);
            Destroy(gameObject);
        }
    }
}
