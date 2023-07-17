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


    GameObject player;
    Vector3 directionToPlayer;
    float stoppingDistOrig;
    bool isCasting;
    int hpOrig;

    bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = gameManager.instance.player;
        stoppingDistOrig = agent.stoppingDistance;
        healthBar = GetComponentInChildren<floatingHealthBar>();
        gameManager.instance.enemyCheckIn();
        hpOrig = hp;
        healthBar.UpdateHealthBar(hp, hpOrig);
        enemyHPBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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

    void ChooseNewDestination()
    {
        Debug.Log("Choose new Destination");
        Vector3 directionToPlayer = transform.position - player.transform.position;
        Vector3 newDestination;

        //choose a destination within max attack distance and outside of retreat distance
        if (IsPlayerTooClose())
        {
            //move away from player
            agent.stoppingDistance = 0;
            newDestination = transform.position + directionToPlayer / 2;
            Debug.Log("moving away from player");
        }
        else
        {
            //move toward player
            agent.stoppingDistance = stoppingDistOrig;
            newDestination = transform.position - directionToPlayer / 2;
            Debug.Log("moving toward player");
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
        StartCoroutine(FlashDamage());
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

    IEnumerator FlashDamage()
    {
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator showTempHp()
    {
        enemyHPBar.SetActive(true);
        yield return new WaitForSeconds(enemyHPBarTimer);
        enemyHPBar.SetActive(false);
    }
}
