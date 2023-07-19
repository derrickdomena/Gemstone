using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCasterAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] GameObject magicShot;
    [SerializeField] GameObject staffTip;
    [SerializeField] GameObject enemyHPBar;
    [SerializeField] floatingHealthBar healthBar;
    [SerializeField] float enemyHPBarTimer;
    [SerializeField] private Collect[] drops;

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] int hp;
    [Range(1f, 2f)][SerializeField] float speedMod;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int itemDropRate;

    [Header("----- Navigation Stats -----")]
    [SerializeField] int maxAttackDistance; //enemy will not attack if player is farther than this
    [SerializeField] int retreatDistance; //enemy will retreat if player is closer than this

    [SerializeField] float innerCircleRadius;
    [SerializeField] float outerCircleRadius;

    GameObject player;
    Vector3 directionToPlayer;
    float stoppingDistOrig;
    bool isCasting;
    int hpOrig;

    private Vector3 circleCenter;

    bool isDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<floatingHealthBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = gameManager.instance.player;
        stoppingDistOrig = agent.stoppingDistance;
        hpOrig = hp;
        healthBar.UpdateHealthBar(hp, hpOrig);
        enemyHPBar.SetActive(false);
        agent.speed = gameManager.instance.playerScript.walkSpeed * speedMod;
    }

    // Update is called once per frame
    void Update()
    {
        circleCenter = player.transform.position;

        if (CanHitPlayerFromHere() && !IsPlayerTooClose())
        {
            agent.destination = transform.position;
            StartCasting();
        }
        else if (!isCasting)
        {
            animator.SetBool("isAttack" , false);
            ChooseNewDestination();
        }
    }

    bool IsPlayerTooClose()
    {
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (playerDistance < retreatDistance)
        {
            return true;
        }

        return false;
    }
    Vector3 GetRandomPointInCircle(Vector3 center, float innerRadius, float outerRadius)
    {
        Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * (outerRadius - innerRadius);
        Vector3 randomPoint = center + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

        return randomPoint;
    }
    void ChooseNewDestination()
    {
        Vector3 directionToPlayer = transform.position - player.transform.position;
        Vector3 newDestination;

        // Choose a destination within the max attack distance and outside of the retreat distance
        if (IsPlayerTooClose())
        {
            // Move away from player
            agent.stoppingDistance = 0;
            newDestination = transform.position + directionToPlayer.normalized * maxAttackDistance;
        }
        else
        {
            // Move toward player
            agent.stoppingDistance = stoppingDistOrig;

            // Calculate a random point in the donut area
            newDestination = GetRandomPointInCircle(player.transform.position, innerCircleRadius, outerCircleRadius);
        }

        // Make sure the new destination is not the current position of the agent
        if (Vector3.Distance(newDestination, transform.position) <= agent.radius)
        {
            // If the new destination is too close to the agent, choose a new one
            ChooseNewDestination();
            return;
        }

        agent.SetDestination(newDestination);
        animator.SetBool("isWalking", true);
    }

    bool CanHitPlayerFromHere()
    {
        directionToPlayer = player.transform.position - transform.position;

        if (Vector3.Distance(transform.position, player.transform.position) < maxAttackDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void InstantiateMagicShot()
    {
        directionToPlayer = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(directionToPlayer.x, directionToPlayer.y, directionToPlayer.z));
        Instantiate(magicShot, staffTip.transform.position, rot);
    }

    void StartCasting() //starts the casting animation
    {
        isCasting = true;
        animator.SetBool("isWalking", false);
        directionToPlayer = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 50);

        animator.SetBool("isAttack", true);
    }

    void FinishCasting() //called at the end frame of the casting animation
    {
        isCasting = false;
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

    private void StopMoving()
    {
        agent.SetDestination(agent.transform.position);
    }

    public void TakeDamage(int amount)
    {
        if (hp <= 0)
        {
            //agent.SetDestination(agent.transform.position);
            isDead = true;
            animator.SetBool("isDead", true);
            GetComponent<Collider>().enabled = false;
        }

        hp -= amount;
        healthBar.UpdateHealthBar(hp, (float)hpOrig);
        //flashes the enemy hp bar above their heads for a fraction of a second.
        //will probably be changed with testing
        StartCoroutine(showTempHp());
    }

    IEnumerator showTempHp()
    {
        enemyHPBar.SetActive(true);
        yield return new WaitForSeconds(enemyHPBarTimer);
        enemyHPBar.SetActive(false);
    }
}
