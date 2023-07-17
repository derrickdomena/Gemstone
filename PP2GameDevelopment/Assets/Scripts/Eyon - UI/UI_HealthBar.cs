using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{

    private HealthSystem _healthSystem;
    public void Setup(HealthSystem healthSystem)
    {
        this._healthSystem = healthSystem;
    }

    //private void Update()
    //{
    //    transform.GetComponent<Image>().fillAmount = _healthSystem.GetHealthPercent();
    //}
}
