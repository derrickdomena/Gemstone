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
    [SerializeField] Animator animator;
    [SerializeField] Transform headPos;

    [Header("Boss Stats")]
    [SerializeField] int moveSpeed;
    [SerializeField] int hpAmount;
    [SerializeField] int viewAngle;
    [SerializeField] int Phase1Damage;
    [SerializeField] int Phase2Damage;
    [SerializeField] int Phase3Damage;
    [SerializeField] int playerFaceSpeed;
    public HealthSystem HealthSystem { get; private set; }
 
    [Header("Attack stuff")]
    [SerializeField] float range;
    [SerializeField] int meleeTimer;
    [SerializeField] int rangeTimer;


    [Header("Boss Phase stuff")]
    public Texture m_MainTexture, PhaseTwoTexture, PhaseThreeTexture;
    Renderer m_Renderer;
    public GameObject bossPrefab;
    private Material Material;


    GameObject player;
    Vector3 playerDir;
    bool isMelee;
    bool isShoot;
    int phaseCounter = 0;
    float stoppingDistanceOrig;
    float angleToPlayer;
    bool immuneToDamage;
    

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
        player = gameManager.instance.player;
        m_Renderer = bossPrefab.GetComponent<Renderer>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void pursuePlayer()
    {
        agent.SetDestination(gameManager.instance.transform.position);
        animator.SetBool("isRun", true);
        if(phaseCounter > 0)
        {
            
            FacePlayer();
            if(phaseCounter == 1) {
               //melee only
               if()
                {

                }
            }
            if(phaseCounter == 2)
            {
                //range only
            }
            if(phaseCounter == 3)
            {
                //both? jumping spider?
            }
        }
    }

    bool CanSeePlayer()
    {
        agent.stoppingDistance = stoppingDistanceOrig;

        playerDir = gameManager.instance.player.transform.position = headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position,playerDir);

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
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
    public void PhaseOne()
    {
        phaseCounter++;
    }
    public void PhaseTwo()
    {
        phaseCounter++;
        m_Renderer.material.SetTexture("_MainTex", PhaseTwoTexture);
        
    }
    public void PhaseThree()
    {
        phaseCounter++;
        m_Renderer.material.SetTexture("_MainTex", PhaseThreeTexture);
        
    }
    public HealthSystem GetHealthSystem()
    {
        return HealthSystem;
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //It died destroy it
        if(OnDead != null) OnDead(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        HealthSystem.Damage(amount);
    }

    IEnumerator Melee()
    {
        isMelee = true;
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(meleeTimer);
        isMelee=false;
        animator.SetBool("isAttack", false);
    }
    //IEnumerator Shoot()
    //{
    //    isShoot = true;
    //}

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
}
