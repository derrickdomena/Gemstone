using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stasis : MonoBehaviour
{
    public string enemyTag = "Enemy";

    public float stasisDuration;

    // Dictionaries to store original values
    private Dictionary<GameObject, float> originalAnimationSpeeds = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> originalVelocities = new Dictionary<GameObject, Vector3>();

    KeyCode stasisKey = KeyCode.Alpha1;
    bool stasisUsed;

    private void Update()
    {
        UpdateStasisUI();
    }
    public void FreezeEnemiesForDuration(float duration)
    {
        stasisUsed = true;
        StartCoroutine(FreezeCoroutine(duration));
    }
    // Updates the stasis UI
    void UpdateStasisUI()
    {
        // When grenadeKey is pressed and canThrow is true, you can throw a grenade
        // also checks to see if the game is paused
        if (Input.GetKeyDown(stasisKey) && !stasisUsed&& Time.timeScale != 0)
        {
            //change function to fireballCooldown
            gameManager.instance.stasisCooldownFill.fillAmount = 0;
            FreezeEnemiesForDuration(stasisDuration);
            stasisUsed = true;
        }

        // When canThrow is true, start incrementing the grenade ability image fill amount
        if (stasisUsed)
        {
            //change function to fireballCooldown
            gameManager.instance.stasisCooldownFill.fillAmount += 1 / gameManager.instance.playerScript.grenadeCooldown * Time.deltaTime;

            // When image fill amount is equal to one, set canThrow to false
            //change function to fireballCooldown
            if (gameManager.instance.stasisCooldownFill.fillAmount == 1)
            {
                stasisUsed = false;
            }
        }
    }

    IEnumerator FreezeCoroutine(float duration)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies)
        {
            NavMeshAgent enemyAgent;
            Animator enemyAnimator;

            if (enemy.name == "Mage(Clone)")
            {
                EnemyCasterAI enemyScript = enemy.GetComponent<EnemyCasterAI>();
                enemyAgent = enemyScript.agent;
                enemyAnimator = enemyScript.animator;
            }
            else
            {
                EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
                enemyAgent = enemyScript.agent;
                enemyAnimator = enemyScript.animator;
            }

            // Store original values in dictionaries
            originalVelocities[enemy] = enemyAgent.velocity;
            originalAnimationSpeeds[enemy] = enemyAnimator.speed;

            // Freeze the enemy
            enemyAgent.velocity = Vector3.zero;
            enemyAgent.isStopped = true;
            enemyAnimator.speed = 0;
        }

        yield return new WaitForSeconds(duration);

        foreach (GameObject enemy in enemies)
        {
            float originalSpeed;
            Vector3 originalVelocity;

            if (originalAnimationSpeeds.TryGetValue(enemy, out originalSpeed) &&
                originalVelocities.TryGetValue(enemy, out originalVelocity))
            {
                if (enemy.name == "Mage(Clone)")
                {
                    EnemyCasterAI enemyScript = enemy.GetComponent<EnemyCasterAI>();
                    enemyScript.agent.velocity = originalVelocity;
                    enemyScript.agent.isStopped = false;
                    enemyScript.animator.speed = originalSpeed;
                }
                else
                {
                    EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
                    enemyScript.agent.velocity = originalVelocity;
                    enemyScript.agent.isStopped = false;
                    enemyScript.animator.speed = originalSpeed;
                }
            }
        }

        // Clear the dictionaries
        originalAnimationSpeeds.Clear();
        originalVelocities.Clear();
    }
}
