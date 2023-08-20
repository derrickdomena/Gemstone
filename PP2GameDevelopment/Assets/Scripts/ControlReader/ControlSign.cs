using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSign : MonoBehaviour
{
    public void Interact()
    {
        gameManager.instance.statePaused();
        gameManager.instance.activeMenu = gameManager.instance.controlMenu;
        gameManager.instance.activeMenu.SetActive(true);
    }
}
