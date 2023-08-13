using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MiniBossWally : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] GameObject magicShot;
    [SerializeField] GameObject staffTip;
    [SerializeField] Image enemyHPBar;

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] int hp;
    [Range(1f, 2f)][SerializeField] float speedMod;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;

    [Header("----- Navigation Stats -----")]
    [SerializeField] int maxAttackDistance; //enemy will not attack if player is farther than this
    [SerializeField] int retreatDistance; //enemy will retreat if player is closer than this

    [SerializeField] float innerCircleRadius;
    [SerializeField] float outerCircleRadius;

    [SerializeField] float deathTimer;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        player = gameManager.instance.player;
        stoppingDistOrig = agent.stoppingDistance;
        hpOrig = hp;
        agent.speed = gameManager.instance.playerScript.walkSpeed * speedMod;
    }

    // Update is called once per frame
    void Update()
    {
        FacePlayer();

        circleCenter = player.transform.position;

        if (CanHitPlayerFromHere()/* && !IsPlayerTooClose()*/)
        {
            agent.destination = transform.position;
            StartCasting();
        }
        else if (!isCasting)
        {
            animator.SetBool("isAttack", false);
            ChooseNewDestination();
        }
    }

    void UpdateHP()
    {
        enemyHPBar.fillAmount = (float)hp / hpOrig;
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
        StartCasting();
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
    void FacePlayer()
    {
        directionToPlayer = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
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

    IEnumerator onDeath()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(deathTimer);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            isDead = true;
            GetComponent<Collider>().enabled = false;
            UpdateHP();
            StartCoroutine(onDeath());
            animator.SetBool("isDead", true);
        }
        else
        {
            UpdateHP();
        }
    }

}
