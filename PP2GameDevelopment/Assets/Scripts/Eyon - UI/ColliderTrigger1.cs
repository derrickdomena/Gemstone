using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger1 : MonoBehaviour
{
    public event EventHandler OnPlayerEnterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();
        if(player != null )
        {
            OnPlayerEnterTrigger?.Invoke(this, EventArgs.Empty);
        }
    }
}
