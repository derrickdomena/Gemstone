using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("----- Dash Components -----")]
    playerController playerScript;

    [Header("----- Dash Stats -----")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldownTime;

    public KeyCode dashKey = KeyCode.E;

    bool canDash;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<playerController>();
        gameManager.instance.dashCooldownFill.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDashUI();
    }

    // Starts the Dash ability
    IEnumerator Dash()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            playerScript.controller.Move(playerScript.move *  dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Updates the Dash ability UI image fill amount
    void UpdateDashUI()
    {
        // When the dashKey is pressed and canDash is true, set dash ability UI to zero and start the startcoroutine Dash() and set canDash to true
        if (Input.GetKeyDown(dashKey) && !canDash)
        {
            gameManager.instance.dashCooldownFill.fillAmount = 0;
            StartCoroutine(Dash());
            canDash = true;
        }

        // When canDash is true, start incrementing the dash ability image fill amount
        if (canDash)
        {
            gameManager.instance.dashCooldownFill.fillAmount += 1 / dashCooldownTime * Time.deltaTime;

            // When image fill amount is equal to one, set canThrow to false
            if (gameManager.instance.dashCooldownFill.fillAmount == 1)
            {                
                canDash = false;
            }
        }
    }
}
