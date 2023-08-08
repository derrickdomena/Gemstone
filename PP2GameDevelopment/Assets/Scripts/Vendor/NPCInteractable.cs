using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public void Interact()
    {
        gameManager.instance.Vendor();
    }
}
