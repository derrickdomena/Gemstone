using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
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
    [SerializeField] private Collect gem;

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

    [Header("----- Enemy Type -----")]
    [SerializeField] string enemyType = "default"; // Change in inspector to "javelin" for javelin enemies

    [Header("----- Dash Mechanics -----")]
    [SerializeField] float dashSpeed = 40f;
    [SerializeField] float dashTriggerDistance = 10f; // The distance at which the enemy decides to dash
    [SerializeField] float dashDistance = 5f;
    bool isDashing = false;

    [Header("----- Dash Cooldown Mechanics -----")]
    [SerializeField] float dashCooldown = 10f; // Cooldown duration
    float lastDashTime = -10f; // Initialize with a value that allows dashing at the start.


    public GameObject damageText;

    private Renderer[] model;
    public bool playerInRange;
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
        hp = hp * (int)gameManager.instance.difficulty;
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
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

        // Check if enough time has elapsed since the last dash
        bool canDash = Time.time - lastDashTime >= dashCooldown;

        if (enemyType == "javelin" && distanceToPlayer > dashTriggerDistance && !isDashing && canDash)
        {
            StartCoroutine(DashTowardsPlayer());
            lastDashTime = Time.time; // Update the time of the last dash
            return; // Exit the method since we're dashing.
        }

        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetBool("isWalking", true);

        if (agent.remainingDistance <= agent.stoppingDistance && !isDashing)
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
        //added trigger for spider. much easier then bool. didnt touch the bool because that would break all of the enemies
        animator.SetTrigger("isAttack");
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(shootRate);
        animator.SetBool("isAttack", false);   
    }

    IEnumerator DashTowardsPlayer()
    {
        isDashing = true;
        Vector3 startPos = transform.position;
        Vector3 dashTarget = transform.position + (gameManager.instance.player.transform.position - transform.position).normalized * dashDistance;
        float dashTime = 0.5f; // Duration for the dash. Adjust as required.
        float elapsedTime = 0;

        while (elapsedTime < dashTime)
        {
            transform.position = Vector3.Lerp(startPos, dashTarget, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = dashTarget; // Set final position after dash
        isDashing = false;
    }


    void DoDamage()
    {
        IDamage playerDam = gameManager.instance.player.GetComponent<IDamage>();
        if (playerDam != null && playerInRange == true)
        {
            playerDam.TakeDamage(shootDamage * (int)gameManager.instance.difficulty);
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
        if (hp <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            GetComponent<Collider>().enabled = false;
        }

        hp -= amount;
        healthBar.UpdateHealthBar(hp, (float)hpOrig);
        DamageIndicator indicator = Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamageIndicator>();
        indicator.SetDamageText(amount);
        //flashes the enemy hp bar above their heads for a fraction of a second.
        //will probably be changed with testing
        StartCoroutine(showTempHp());
    }

    private void StopMoving()
    {
        agent.SetDestination(agent.transform.position);
    }

    private void Death()
    {
        int selectedChance = Random.Range(1, 100);
        float maxDropOffset = 1f;  // Adjust this value based on the size of your drops and how far apart you want them

        if (selectedChance <= itemDropRate)
        {
            int itemToDrop = Random.Range(0, drops.Length);
            Vector3 dropPosition = new Vector3(agent.transform.position.x, agent.transform.position.y + 1, agent.transform.position.z) + GetRandomOffset(maxDropOffset);
            Instantiate(drops[itemToDrop], dropPosition, Quaternion.identity);
        }
        Destroy(gameObject);

        Vector3 gemPosition = new Vector3(agent.transform.position.x, agent.transform.position.y + 1, agent.transform.position.z) + GetRandomOffset(maxDropOffset);
        Instantiate(gem, gemPosition, Quaternion.identity);

        gameManager.instance.enemyCheckOut();

    }
    Vector3 GetRandomOffset(float maxOffset)
    {
        return new Vector3(Random.Range(-maxOffset, maxOffset), 0, Random.Range(-maxOffset, maxOffset));
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
