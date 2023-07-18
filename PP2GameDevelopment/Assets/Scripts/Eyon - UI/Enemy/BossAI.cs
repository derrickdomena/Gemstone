using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BossAI : MonoBehaviour, IDamage
{
    public event EventHandler OnDead;
    [Header("Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] private ColliderTrigger1 DamageTrigger;
    [SerializeField] GameObject phase2Platform;
    

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
    public HealthSystem HealthSystem { get; private set; }
 
    [Header("Attack stuff")]
    [SerializeField] float range;
    [SerializeField] int meleeTimer;
    [SerializeField] float rangeTimer;
    [SerializeField] GameObject poison;
    [SerializeField] Transform shootPos;


    [Header("Boss Phase stuff")]
    public Texture m_MainTexture, PhaseTwoTexture, PhaseThreeTexture;
    Renderer m_Renderer;
    public GameObject bossPrefab;
    private Material Material;


    Vector3 playerDir;
    bool isMelee;
    bool isShoot;
    public int phaseCounter = 0;
    float stoppingDistanceOrig;
    float angleToPlayer;
    bool immuneToDamage;
    bool playerInRange;
    bool isDead;
    bool isSlam;



    private void Awake()
    {
        HealthSystem = new HealthSystem(hpAmount);
    }
    //private void Setup(HealthSystem healthSystem)
    //{
    //    this.HealthSystem = healthSystem;
    //    healthSystem.OnDead += HealthSystem_OnDead;
    //}
    private void Start()
    {
        stoppingDistanceOrig = agent.stoppingDistance;
        agent = GetComponent<NavMeshAgent>();
        m_Renderer = bossPrefab.GetComponent<Renderer>();

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
                if (CanSeePlayer() && !isShoot)
                {
                    StartCoroutine(Shoot());
                }
            }
            if (phaseCounter == 3)
            {
                StartCoroutine(Slam());
            }
        }
    }
    void pursuePlayer()
    {
        if(phaseCounter != 3)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            animator.SetBool("isRun", true);

            if (agent.remainingDistance <= agent.stoppingDistance +2)
            {
                animator.SetBool("isRun", false);
                //agent.SetDestination(agent.)
                playerInRange = true;
                
            }
            else
            {
                playerInRange = false;
            }
        }
    }

    bool CanSeePlayer()
    {
        agent.stoppingDistance = stoppingDistanceOrig;
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer < viewAngle)
            {
                FacePlayer();
                return true;
            }
        }
        return false;
    }

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
        animator.SetBool("isRun", false);
        if (agent.transform.position != phase2Platform.transform.position)
        {
            animator.SetBool("isRunP2", true);
            //agent.SetDestination(phase2Platform.transform.position);

            agent.transform.position = phase2Platform.transform.position;
            moveSpeed = 0;
            
            StartCoroutine(immunityPhase());
            m_Renderer.material.SetTexture("_MainTex", PhaseTwoTexture);
            playerFaceSpeed = 100;
        }
    }
    public void PhaseThree()
    {
        animator.SetBool("isRunP2", false);
        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetBool("HitToP3", true);

        moveSpeed = 4;
        m_Renderer.material.SetTexture("_MainTex", PhaseThreeTexture);
        animator.SetBool("HitToP3", false);

    }
    public HealthSystem GetHealthSystem()
    {
        return HealthSystem;
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //It died destroy it
        isDead = true;
        animator.SetBool("isDead", true);
        if(OnDead != null) OnDead(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        if(immuneToDamage == true)
        {
            HealthSystem.Heal(amount);
        }
        else
        {
            HealthSystem.Damage(amount);
        }
    }

    IEnumerator Slam()
    {
        isSlam = true;
        animator.SetBool("isAttackP3", true);
        yield return new WaitForSeconds(slamTimer);
        animator.SetBool("isAttackP3", false);
    }
    IEnumerator Melee()
    {
        isMelee = true;
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(meleeTimer);
        isMelee =false;
        animator.SetBool("isAttack", false);
    }
    IEnumerator Shoot()
    {
        isShoot = true;
        animator.SetBool("isAttackP2", true);
        Instantiate(poison, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(rangeTimer);
        isShoot = false;
    }

    public void doDamage()
    {
        IDamage playerD = gameManager.instance.player.GetComponent<IDamage>();
        if (playerD != null)
        {
            if (phaseCounter == 1)
            {
                playerD.TakeDamage(Phase1Damage);
            }
            if(phaseCounter == 2)
            {
                playerD.TakeDamage(Phase2Damage);
            }
            if (phaseCounter == 3)
            {
                playerD.TakeDamage(Phase3Damage);
            }
        }
    }
    IEnumerator immunityPhase()
    {
        if (phaseCounter == 2) {
            immuneToDamage = true;
            animator.SetBool("HitToP2", true);
            yield return new WaitForSeconds(immuneTimer);
            animator.SetBool("HitToP2", false);
            immuneToDamage = false;
        }
        else
        {
            immuneToDamage = true;
            animator.SetBool("HitToP3", true);
            yield return new WaitForSeconds(immuneTimer);
            animator.SetBool("HitToP3", true);
            immuneToDamage = false;
        }
    }
    
}
