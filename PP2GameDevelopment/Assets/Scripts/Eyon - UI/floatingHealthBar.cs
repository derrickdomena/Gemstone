using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform cameraPos;
    [SerializeField] public Transform target;
    private GameObject parent;
    [SerializeField] private Vector3 offset;


    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue/maxValue; 
    }
    private void Start()
    {
        
        cameraPos = FindObjectOfType<Camera>(Camera.main).transform;
        slider = GetComponentInChildren<Slider>();
        //target = parent.GetComponent<Transform>();


        
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = cameraPos.transform.rotation; 
        transform.position = target.position + offset;
    }
}
