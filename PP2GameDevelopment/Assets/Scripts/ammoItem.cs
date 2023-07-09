using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoItem : MonoBehaviour, IAmmo
{
    int magAmount = 1;

    // Update rotates the X, Y axis of the iteam
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime, 50f * Time.deltaTime, 0, Space.Self);
    }

    // When item collision occurs with Player, the item gets destroyed and magAmmount gets passed as an ammount to MoreAmmo
    private void OnTriggerEnter(Collider other)
    {
        GiveAmmo(magAmount);
        Destroy(gameObject);
    }

    // When ammo pack is picked up, increases magazine
    public void GiveAmmo(int amount)
    {
        Weapon.instance.magazines += amount;
    }

}
