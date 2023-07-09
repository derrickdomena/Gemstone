using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class healingItem : MonoBehaviour, IHealth
{
    int healAmount = 10;

    // Update rotates the X, Y axis of the iteam
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime, 50f * Time.deltaTime, 0, Space.Self);
    }

    // When item collision occurs with Player, the item gets destroyed and healAmmount gets passed as an ammount to Heal
    private void OnTriggerEnter(Collider other)
    {
        GiveHealth(healAmount);
        Destroy(gameObject);
    }

    // Receive health when picking up a health pack
    public void GiveHealth(int amount)
    {
        //hp += amount; Future use, when health packs vary in healing ammount.
        gameManager.instance.playerScript.hp = amount;
        gameManager.instance.playerScript.UpdatePlayerUI();
    }
}
