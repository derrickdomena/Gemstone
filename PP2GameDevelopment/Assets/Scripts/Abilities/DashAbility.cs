using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    playerController playerScript;

    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCoolDownTime;

    public KeyCode dashKey = KeyCode.E;

    private float dashAvailableTime;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >  dashAvailableTime)
        {
            if (Input.GetKeyDown(dashKey))
            {
                StartCoroutine(Dash());
                dashAvailableTime = Time.time + dashCoolDownTime;
            }
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            playerScript.controller.Move(playerScript.move *  dashSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
