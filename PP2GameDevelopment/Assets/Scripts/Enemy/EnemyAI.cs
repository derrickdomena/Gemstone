using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] floatingHealthBar healthBar;
    [SerializeField] Animator animator;
    [SerializeField] private Collect[] drops;

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] int hp;
    [Range(1, 20)][SerializeField] int moveSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int roamTimer;
    [SerializeField] int roamDist;
    [SerializeField] GameObject enemyHPBar;
    [SerializeField] float enemyHPBarTimer;
    [SerializeField] bool meleeOrRange;
    [SerializeField] int itemDropRate;

    [Header("----- Gun stuff -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;

    private Renderer[] model;
    bool playerInRange;
    bool destinationChosen;
    float angleToPlayer;
    bool isShooting;
    bool isMelee;
    bool inMeleeRange;
    float stoppingDistanceOrig;
    Vector3 playerDir;
    int hpOrig;

    bool isDead = false;

    // Awake is called when the script instance is loaded, before Start()
    public void Awake()
    {
        healthBar = GetComponentInChildren<floatingHealthBar>();
        model = GetComponentsInChildren<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hpOrig = hp;
        stoppingDistanceOrig = agent.stoppingDistance;
        healthBar.UpdateHealthBar(hp, hpOrig);
        enemyHPBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();

        if (!isDead) 
        {
            PursuePlayer();
        }

    }
    bool CanSeePlayer()
    {
        agent.stoppingDistance = stoppingDistanceOrig; 
        playerDir = gameManager.instance.player.transform.position - headPos.position; 
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward); 
       // Debug.DrawRay(headPos.position, playerDir);         
       // Debug.Log(angleToPlayer);          
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

    void PursuePlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetBool("isWalking", true);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("isWalking", false);
            FacePlayer();
            if (playerInRange)
            {
                StartCoroutine(Melee());
            }
        }
    }

    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    IEnumerator Melee()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(shootRate);
        animator.SetBool("isAttack", false);   
    }

    void DoDamage()
    {
        IDamage playerDam = gameManager.instance.player.GetComponent<IDamage>();
        if (playerDam != null)
        {
            playerDam.TakeDamage(shootDamage);
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

    public void TakeDamage(int amount)
    {
        hp -= amount;
        healthBar.UpdateHealthBar(hp, (float)hpOrig);

        //flashes the enemy hp bar above their heads for a fraction of a second.
        //will probably be changed with testing
        StartCoroutine(showTempHp());

        if (hp <= 0)
        {
            //agent.SetDestination(agent.transform.position);
            isDead = true;
            animator.SetBool("isDead", true);

        }
    }

    private void StopMoving()
    {
        agent.SetDestination(agent.transform.position);
    }

    private void Death()
    {
        int selectedChance = Random.Range(1, 100);
        if (selectedChance <= itemDropRate)
        {
            int itemToDrop = Random.Range(0, 3);
            Collect droppedItem = Instantiate(drops[itemToDrop], new Vector3(agent.transform.position.x, agent.transform.position.y + 1, agent.transform.position.z), Quaternion.identity);
        }
        Destroy(gameObject);
        gameManager.instance.enemyCheckOut();

    }

    //enables the enemy hpbar to show up for a fraction of a second.
    //will probably get changed with testing
    IEnumerator showTempHp()
    {
        enemyHPBar.SetActive(true);
        yield return new WaitForSeconds(enemyHPBarTimer);
        enemyHPBar.SetActive(false);
    }



}
