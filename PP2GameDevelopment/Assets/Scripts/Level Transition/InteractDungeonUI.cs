using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDungeonUI : MonoBehaviour
{
    [SerializeField] private GameObject containerObject;
    [SerializeField] private GameObject noWeaponNoti;
    [SerializeField] private PlayerInteract dungeon;

    private void Start()
    {
        dungeon = gameManager.instance.player.GetComponentInChildren<PlayerInteract>();
    }
    private void Update()
    {
        if (dungeon.GetInteractObject() != null)
        {
            Show();
            if (gameManager.instance.playerScript.weaponList.Count == 0)
            {
                ShowNoti();
            }
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
    private void ShowNoti()
    {
        noWeaponNoti.SetActive(true);
    }
    private void Hide()
    {
        containerObject.SetActive(false);
        noWeaponNoti.SetActive(false);
    }
}
