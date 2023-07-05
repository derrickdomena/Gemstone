using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healingItem : MonoBehaviour
{
    int healAmount;

    private void Update()
    {
        transform.Rotate(0, 50f * Time.deltaTime, 0, Space.Self);
        transform.Rotate(50f * Time.deltaTime, 0 , 0, Space.Self);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<playerController>().Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
