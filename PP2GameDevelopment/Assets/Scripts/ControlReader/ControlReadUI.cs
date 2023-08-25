using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlReadUI : MonoBehaviour
{
    [SerializeField] private GameObject containerObject;
    [SerializeField] private PlayerInteract sign;

    private void Start()
    {
        sign = gameManager.instance.player.GetComponentInChildren<PlayerInteract>();
    }
    private void Update()
    {
        if (sign.GetInteractable() != null && gameManager.instance.activeMenu != gameManager.instance.controlMenu)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void Show()
    {
        containerObject.SetActive(true);
    }
    private void Hide()
    {
        containerObject.SetActive(false);
    }
}
