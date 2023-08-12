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
    
    //[SerializeField] public float dashCooldownTime;

    public KeyCode dashKey = KeyCode.E;

    public int remainingDashes;
    bool canDash;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<playerController>();
        gameManager.instance.dashCooldownFill.fillAmount = 1;
        remainingDashes = 1;
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

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(gameManager.instance.playerScript.dashCooldown);
        remainingDashes++;
    }

    // Updates the Dash ability UI image fill amount
    void UpdateDashUI()
    {
        // When the dashKey is pressed and canDash is true, set dash ability UI to zero and start the startcoroutine Dash() and set canDash to true
        // also checks to see if the game is paused
        if (Input.GetKeyDown(dashKey) && !canDash && Time.timeScale != 0 && remainingDashes != 0)
        {
            StartCoroutine(Dash());
            remainingDashes--;

            if (remainingDashes < gameManager.instance.playerScript.dashCount && canDash == false)
            {
                StartCoroutine(DashCooldown());
            }
            if (remainingDashes == 0)
            {
                gameManager.instance.dashCooldownFill.fillAmount = 0;
                canDash = true;
            }
        }

        // When canDash is true, start incrementing the dash ability image fill amount
        if (canDash)
        {
            gameManager.instance.dashCooldownFill.fillAmount += 1 / gameManager.instance.playerScript.dashCooldown * Time.deltaTime;

            // When image fill amount is equal to one, set canThrow to false
            if (gameManager.instance.dashCooldownFill.fillAmount == 1)
            {                
                canDash = false;
            }
        }
    }

    public void UpdateCooldownDash(float time)
    {
        if(gameManager.instance.playerScript.dashCooldown != 0)
        {
            gameManager.instance.playerScript.dashCooldown = gameManager.instance.playerScript.dashCooldown - time;
            canDash = true;
        }
        UpdateDashUI();
    }

    public void IncreaseDashDistance(float time)
    {
        dashTime += time;
    }
}
