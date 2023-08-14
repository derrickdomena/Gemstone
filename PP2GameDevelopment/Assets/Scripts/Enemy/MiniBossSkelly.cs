using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MiniBossSkelly : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int hp;
    [SerializeField] Image hpBar;
    [SerializeField] Animator animator;
    [SerializeField] int damage;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] float attack1Timer;
    [SerializeField] float attack2Timer;
    [SerializeField] float deathTimer;

    public GameObject damageText;

    public bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;
    float stoppingDistanceOrig;
    bool dead;
    int origHP;
    Vector3 velocity;


    // Start is called before the first frame update
    void Start()
    {
        stoppingDistanceOrig = agent.stoppingDistance;
        velocity = agent.velocity;
        origHP = hp;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!dead)
        {
            CanSeePlayer();
            pursuePlayer();
            //if hp is less than or equal to 50% of the original hp
            if(hp <= origHP*.5 && playerInRange == true)
            {
                Attack2();
            }
            else if(playerInRange == true)
            {
                Attack1();
            }
        }
    }
    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            //agent.SetDestination(agent.transform.position);
            dead = true;
            GetComponent<Collider>().enabled = false;
            UpdateHP();
            StartCoroutine(onDeath());
            animator.SetTrigger("Dead");
        }
        else
        {
            UpdateHP();
        }
        DamageIndicator indicator = Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamageIndicator>();
        indicator.SetDamageText(amount);
    }

    void UpdateHP()
    {
        hpBar.fillAmount = (float)hp / origHP;
    }
    void DoDamage()
    {
        IDamage playerDam = gameManager.instance.player.GetComponent<IDamage>();
        if (playerDam != null && playerInRange == true)
        {
            playerDam.TakeDamage(damage);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    bool CanSeePlayer()
    {
        agent.stoppingDistance = stoppingDistanceOrig;
        playerDir = gameManager.instance.playerScript.midMass.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        // Debug.DrawRay(headPos.position, playerDir);         
        // Debug.Log(angleToPlayer);          
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (agent.remainingDistance <= agent.remainingDistance)
            {
                FacePlayer();
            }
            return true;
        }
        return false;
    }
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    void pursuePlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetTrigger("Walk");
   
    }
    void Attack1()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("Attack1",true);
        StartCoroutine(Attack1Timer());

    }
    void Attack2()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("Attack2", true);
        StartCoroutine(Attack2Timer());
      
        
    }

    IEnumerator Attack1Timer()
    {
        yield return new WaitForSeconds(attack1Timer);
        animator.SetBool("Attack1", false);
        agent.isStopped = false;
        agent.velocity = velocity;
    }
    IEnumerator Attack2Timer()
    {
        yield return new WaitForSeconds(attack2Timer);
        animator.SetBool("Attack2", false);
        agent.isStopped = false;
        agent.velocity = velocity;
    }
    IEnumerator onDeath() {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(deathTimer);
        //Destroy(gameObject);
        StartCoroutine(gameManager.instance.LevelCleared());
    }


}
