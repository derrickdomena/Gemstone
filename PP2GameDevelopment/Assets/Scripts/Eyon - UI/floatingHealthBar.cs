using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform cameraPos;
    [SerializeField] public Transform target;
    [SerializeField] private Vector3 offset;


    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        //decrements the slider value depending on the damage taken
        slider.value = currentValue/maxValue; 
    }
    private void Start()
    {
        //sets the way the healthbar faces to the main camera
        cameraPos = FindObjectOfType<Camera>(Camera.main).transform;
        //sets slider to the hp bar slider
        slider = GetComponentInChildren<Slider>();

    }
    // Update is called once per frame
    void Update()
    {
        //rotates the healthbar to always face the player
        transform.rotation = cameraPos.transform.rotation; 
        //offsets the position of the camera to the enemy by the offset
        transform.position = target.position + offset;
    }
}
