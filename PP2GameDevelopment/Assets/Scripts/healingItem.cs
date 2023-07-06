using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healingItem : MonoBehaviour
{
    int healAmount;

    // Update rotates the X, Y axis of the iteam
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime, 50f * Time.deltaTime, 0, Space.Self);
    }

    // When item collision occurs with Player, the item gets destroyed and healAmmount gets passed as an ammount to Heal
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<playerController>().Heal(healAmount);
            Destroy(gameObject);
        }
    }
}