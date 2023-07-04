using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    public Slider slider;
    public Camera camera;
    public Transform target;
    private GameObject parent;
    [SerializeField] private Vector3 offset;


    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue/maxValue; 
    }
    private void Start()
    {
        camera = FindObjectOfType<Camera>(Camera.main);
        slider = GetComponentInChildren<Slider>();
        target = parent.GetComponentInParent<Transform>();

        
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation; 
        transform.position = target.position + offset;
    }
}
