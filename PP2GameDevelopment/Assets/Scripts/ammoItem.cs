using UnityEngine;

public class ammoItem : MonoBehaviour, IAmmo
{
    int ammoAmount = 10;

    // Update rotates the X, Y axis of the iteam
    private void Update()
    {
        transform.Rotate(50f * Time.deltaTime, 50f * Time.deltaTime, 0, Space.Self);
    }

    // When item collision occurs with Player, the item gets destroyed and magAmmount gets passed as an ammount to MoreAmmo
    private void OnTriggerEnter(Collider other)
    {
        GiveAmmo(ammoAmount);
        Destroy(gameObject);
    }

    // When ammo pack is picked up, increases ammoReserve
    public void GiveAmmo(int amount)
    {
        //IMPORTANT: kinda... This doesnt seem efficient.. is there a better way to increase that number?
        gameManager.instance.playerScript.gunList[gameManager.instance.playerScript.selectedGun].ammoReserve += amount;
    }

}
