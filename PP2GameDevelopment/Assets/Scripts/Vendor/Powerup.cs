using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    public PowerupEffect powerupEffect;

    public void OnButtonClick()
    {
        if(gameManager.instance.playerScript.TrySpendGemAmount(gameManager.instance.gemCount) == true)
        {
            Destroy(gameObject);
            powerupEffect.Apply();
        }
        else
        {
            return;
        }
    }
}
