using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stasis : MonoBehaviour
{
    public string enemyTag = "Enemy";

    public float stasisDuration;

    private Dictionary<GameObject, float> originalAnimationSpeeds = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> originalVelocities = new Dictionary<GameObject, Vector3>();

    KeyCode stasisKey = KeyCode.C;
    bool stasisUsed;

    AudioManager audioManager;
    Animator animator;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void Update()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }
        if (animator == null)
        {
            animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        }
        UpdateStasisUI();
    }
    public void FreezeEnemiesForDuration(float duration)
    {      
        stasisUsed = true;
        audioManager.PlaySFXAbilities(audioManager.stasisSound);
        StartCoroutine(FreezeCoroutine(duration));
    }

    void UpdateStasisUI()
    {

        if (Input.GetKeyDown(stasisKey) && !stasisUsed && Time.timeScale != 0)
        {         
            animator.SetBool("isStasis", true);
            gameManager.instance.stasisCooldownFill.fillAmount = 0;
            FreezeEnemiesForDuration(stasisDuration);          
            stasisUsed = true;
        }
        else
        {
            animator.SetBool("isStasis", false);
        }

        if (stasisUsed)
        {
 
            gameManager.instance.stasisCooldownFill.fillAmount += 1 / gameManager.instance.playerScript.stasisCooldown * Time.deltaTime;

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

            if (enemy.name == "Mage(Clone)" || enemy.name == "Spider_Normal(Clone)" || enemy.name == "Spider_Melee(Clone)")
            {
                EnemyCasterAI enemyScript = enemy.GetComponent<EnemyCasterAI>();
                enemyAgent = enemyScript.agent;
                enemyAnimator = enemyScript.animator;
                enemyScript.model.material.color = Color.blue;
            }
            else
            {
                EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
                enemyAgent = enemyScript.agent;
                enemyAnimator = enemyScript.animator;
                enemyScript.model.material.color = Color.blue;
            }

            originalVelocities[enemy] = enemyAgent.velocity;
            originalAnimationSpeeds[enemy] = enemyAnimator.speed;

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
                    enemyScript.model.material.color = Color.grey;
                }
                else
                {
                    EnemyAI enemyScript = enemy.GetComponent<EnemyAI>();
                    enemyScript.agent.velocity = originalVelocity;
                    enemyScript.agent.isStopped = false;
                    enemyScript.animator.speed = originalSpeed;
                    enemyScript.model.material.color = Color.grey;
                }
            }
        }

        originalAnimationSpeeds.Clear();
        originalVelocities.Clear();
    }
}
