using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage
{
    public event EventHandler OnDead;
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] private ColliderTrigger1 DamageTrigger;
    [SerializeField] GameObject phase2Platform;
    [SerializeField]public GameObject shoot;
    [SerializeField] Collider Phase3Slam;
    [SerializeField] ParticleSystem slam;
    GameObject spawner;
    
    
    [Header("Boss Stats")]
    [SerializeField] int moveSpeed;
    [SerializeField] int hpAmount;
    [SerializeField] int viewAngle;
    [SerializeField] int Phase1Damage;
    [SerializeField] int Phase2Damage;
    [SerializeField] int Phase3Damage;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int immuneTimer;
    [SerializeField] int slamTimer;
    [SerializeField] int deathTimer;
    public HealthSystem HealthSystem { get; private set; }

    [Header("Attack stuff")]
    [SerializeField] int meleeTimer;
    //[SerializeField] GameObject poison;
    [SerializeField] Transform shootPos;


    [Header("Boss Phase stuff")]
    public Texture m_MainTexture, PhaseTwoTexture, PhaseThreeTexture;
    Renderer m_Renderer;
    public GameObject bossPrefab;
    private Material Material;


    Vector3 playerDir;
    GameObject player;
    bool isShoot;
    public int phaseCounter = 0;
    float stoppingDistanceOrig;
    float angleToPlayer;
    bool immuneToDamage = false;
    bool playerInRange;
    bool isDead;
   
    AudioManager audioManager;

    private void Awake()
    {
        HealthSystem = new HealthSystem(hpAmount);
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        stoppingDistanceOrig = agent.stoppingDistance;
        agent = GetComponent<NavMeshAgent>();
        player = gameManager.instance.player;
        m_Renderer = bossPrefab.GetComponent<Renderer>();
        GetSpawner();
    }
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (phaseCounter == 1)
            {
                if (CanSeePlayer())
                {
                    pursuePlayer();
                }
                if (playerInRange)
                {
                    StartCoroutine(Melee());
                }
            }
            if (phaseCounter == 2)
            {
                if(gameManager.instance.enemiesRemaining <=0)
                {
                    immuneToDamage = false;
                    agent.GetComponent<SphereCollider>().enabled = true;
                }
                if (CanSeePlayer() && !isShoot)
                {
                    StartCasting();
                }
            }
            if (phaseCounter == 3)
            {
                StartCoroutine(Slam());
            }

        }
        else
        {
            isDead = true;
        }
    }
    //walk to the player only on phase 1 and 2
    void pursuePlayer()
    {
        if (phaseCounter != 3)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            animator.SetBool("isRun", true);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("isRun", false);
                //agent.SetDestination(agent.)

            }
        }
    }
    //if you cant see the player and he enters your View angle turn to the player
    bool CanSeePlayer()
    {
        agent.stoppingDistance = stoppingDistanceOrig;
        playerDir = gameManager.instance.playerScript.midMass.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer < viewAngle)
            {
                return true;
            }
        }
        return false;
    }
    //if you cant see the player and he enters your View angle turn to the player
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
    public void PhaseOne()
    {
        animator.SetBool("startBoss", false);

    }
    public void PhaseTwo()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossHiss);
        animator.SetBool("isRun", false);
        if (agent.transform.position != phase2Platform.transform.position)
        {
            //agent.transform.position = phase2Platform.transform.position;
            agent.speed = 0;

            StartCoroutine(immunityPhase());
            
            m_Renderer.material.SetTexture("_MainTex", PhaseTwoTexture);
            playerFaceSpeed = 100;
        }
    }
    //final phase
    public void PhaseThree()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossHiss);
        agent.SetDestination(gameManager.instance.player.transform.position);
       
        StartCoroutine(immunityPhase());
        agent.speed = 10;
        m_Renderer.material.SetTexture("_MainTex", PhaseThreeTexture);
       

    }
    //Allows other functions to call the health system for the boss
    public HealthSystem GetHealthSystem()
    {
        return HealthSystem;
    }

    public void TakeDamage(int amount)
    {
        if(immuneToDamage == true)
        {
            //hes immune
            return;
        }
        else
        {
            HealthSystem.Damage(amount);
        }
    }

    IEnumerator Slam()
    {
       
        animator.SetBool("isAttackP3", true);
        yield return new WaitForSeconds(slamTimer);
        animator.SetBool("isAttackP3", false);
        
    }
    IEnumerator Melee()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(meleeTimer);
        animator.SetBool("isAttack", false);
    }
    public void InstantiateMagicShot()
    {
        playerDir = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z));
        Instantiate(shoot, shootPos.transform.position, transform.rotation);
        //Sound
        audioManager.PlaySFXEnemy(audioManager.spiderBossProjectile);
    }

    void StartCasting() //starts the casting animation
    {
        isShoot = true;
        playerDir = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 50);

        animator.SetBool("isAttackP2", true);
    }

    void FinishCasting() //called at the end frame of the casting animation
    {
        isShoot = false;
    }
    public void doDamage()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossMelee);
        IDamage playerD = gameManager.instance.player.GetComponent<IDamage>();
        if (playerD != null && playerInRange)
        {
            if (phaseCounter == 1)
            {
                playerD.TakeDamage(Phase1Damage);
            }
            //phase 2 damage is handled by the bullet

            if (phaseCounter == 3)
            {
                playerD.TakeDamage(Phase3Damage);
            }
        }
    }
    IEnumerator immunityPhase()
    {
        if (phaseCounter == 2)
        {
            animator.SetBool("isAttack", false);
            immuneToDamage = true;

            animator.SetBool("HitToP2", true);
            //uncomment this when we have spider waves
            //Boss will be immune until all enemies are dead
            agent.GetComponent<SphereCollider>().enabled = false;
            spawner.SetActive(true);
            yield return new WaitForSeconds(immuneTimer);
            animator.SetBool("HitToP2", false);
            
        }
        else if (phaseCounter == 3)
        {
            animator.SetBool("isAttackP2", false);
            animator.SetBool("HitToP3", true);
            //uncomment this when we have spider waves
            //Boss will be immune until all enemies are dead
            //agent.GetComponent<SphereCollider>().enabled = false;    
            yield return new WaitForSeconds(immuneTimer);
            animator.SetBool("HitToP3", false);
            
        }
    }
    IEnumerator Dead()
    {
        //play a effect here at some point
        yield return new WaitForSeconds(deathTimer);
        Destroy(gameObject);
    }

    void Emit()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossSlam);
        Phase3Slam.enabled = true;
        slam.Emit(100);
    }
    void colliderEnd()
    {
        Phase3Slam.enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    private void GetSpawner()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "Spawner")
            {
                spawner = child.gameObject;
            }
        }
    }

    private void BossSpiderSlam()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossSlam);
    }

    private void BossSpiderDeath()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossDeath);
    }

    private void BossSpiderWalk()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderBossWalk);
    }

}
