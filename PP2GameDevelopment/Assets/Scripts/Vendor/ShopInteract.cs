using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteract : MonoBehaviour
{
    [SerializeField] private UI_Script uiShop;
    private void OnTriggerEnter(Collider other)
    {
        ShopCustomer customer = other.GetComponent<ShopCustomer>();
        if(customer != null)
        {
            gameManager.instance.Vendor();
        }
    
    }

    public void OnButtonClick()
    {
        gameManager.instance.ShopMask.SetActive(false);
        gameManager.instance.stateUnpaused();
        gameManager.instance.shop.SetActive(false);
        gameManager.instance.activeMenu = null;
    }
}
