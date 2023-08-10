using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDungeonUI : MonoBehaviour
{
    [SerializeField] private GameObject containerObject;
    [SerializeField] private PlayerInteractDungeon dungeon;

    private void Start()
    {
        dungeon = gameManager.instance.player.GetComponentInChildren<PlayerInteractDungeon>();
    }
    private void Update()
    {
        if (dungeon.GetInteractableObject() != null)
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
