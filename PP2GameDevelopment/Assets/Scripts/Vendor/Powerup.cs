using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    public PowerupEffect powerupEffect;

    public void OnButtonClick(float amount)
    {
        Destroy(gameObject);
        powerupEffect.Apply(amount);
    }
}
