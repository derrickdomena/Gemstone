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

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] int hp;
    [Range(1, 20)][SerializeField] int moveSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;

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
        gameManager.instance.enemyCheckIn();
        hpOrig = hp;
        healthBar.UpdateHealthBar(hp, hpOrig);
        enemyHPBar.SetActive(false);
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
            Debug.Log("Player is too close");
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
        Debug.Log("Choose new Destination");
        Vector3 directionToPlayer = transform.position - player.transform.position;
        Vector3 newDestination;

        // Choose a destination within the max attack distance and outside of the retreat distance
        if (IsPlayerTooClose())
        {
            // Move away from player
            agent.stoppingDistance = 0;
            newDestination = transform.position + directionToPlayer.normalized * maxAttackDistance;
            Debug.Log("Moving away from player");
        }
        else
        {
            // Move toward player
            agent.stoppingDistance = stoppingDistOrig;

            // Calculate a random point in the donut area
            newDestination = GetRandomPointInCircle(player.transform.position, innerCircleRadius, outerCircleRadius);
            Debug.Log("Moving toward player");
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
                    Debug.Log("player is in range");
                    return true;
                }
            }
        }

        Debug.Log("can't hit player from here");
        ChooseNewDestination();
        return false;
    }

    public void InstantiateMagicShot()
    {
        Instantiate(magicShot, staffTip.transform.position, transform.rotation);
    }

    void StartCasting() //todo: StartCasting()
    {
        animator.SetBool("isWalking", false);
        directionToPlayer = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 50);
        Debug.DrawRay(directionToPlayer, transform.position);

        animator.SetBool("isAttack", true);
    }

    void FinishCasting() //todo: FinishCasting()
    {

    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        healthBar.UpdateHealthBar((float)hp, hpOrig);
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

    IEnumerator showTempHp()
    {
        enemyHPBar.SetActive(true);
        yield return new WaitForSeconds(enemyHPBarTimer);
        enemyHPBar.SetActive(false);
    }
}
