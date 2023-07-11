using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    public Image enemyHPBar;
    public Transform cameraPos;
    [SerializeField] public Transform target;
    //[SerializeField] private Vector3 offset;


    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        //decrements value depending on the damage taken
        enemyHPBar.fillAmount = currentValue/maxValue; 
    }
    private void Start()
    {
        //sets the way the healthbar faces to the main camera
        cameraPos = FindObjectOfType<Camera>(Camera.main).transform;


    }
    // Update is called once per frame
    void Update()
    {
        //rotates the healthbar to always face the player
        transform.rotation = cameraPos.transform.rotation; 
        //offsets the position of the camera to the enemy by the offset
       // transform.position = target.position + offset;
    }
}
