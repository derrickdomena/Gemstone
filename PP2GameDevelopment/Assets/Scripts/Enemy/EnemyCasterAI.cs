using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCasterAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] GameObject magicShot;
    [SerializeField] GameObject staffTip;
    [SerializeField] GameObject enemyHPBar;
    [SerializeField] floatingHealthBar healthBar;
    [SerializeField] float enemyHPBarTimer;
    [SerializeField] private Collect[] drops;
    [SerializeField] private Collect gem;
    [SerializeField] public Renderer model;

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] public int hp;
    [Range(1f, 2f)][SerializeField] float speedMod;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int itemDropRate;

    [Header("----- Navigation Stats -----")]
    [SerializeField] int maxAttackDistance; //enemy will not attack if player is farther than this
    [SerializeField] int retreatDistance; //enemy will retreat if player is closer than this

    [SerializeField] float innerCircleRadius;
    [SerializeField] float outerCircleRadius;

    [Header("----- Enemy Type -----")]
    [SerializeField] string enemyType = "default"; // Change in inspector to "mage" for mage enemies

    GameObject player;
    Vector3 directionToPlayer;
    float stoppingDistOrig;
    bool isCasting;
    int hpOrig;

    private Vector3 circleCenter;

    public GameObject damageText;
    bool isDead;

    AudioManager audioManager;

    enum State
    {
        Idle,
        Pursue,
        Attack,
        Retreat,
        Moving
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<floatingHealthBar>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = gameManager.instance.player;
        stoppingDistOrig = agent.stoppingDistance;
        hpOrig = hp;
        hp = hp * (int)gameManager.instance.difficulty;
        healthBar.UpdateHealthBar(hp, hpOrig);
        enemyHPBar.SetActive(false);
        agent.speed = gameManager.instance.playerScript.walkSpeed * speedMod;
    }

    State currentState = State.Pursue;

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                DoIdle();
                break;
            case State.Pursue:
                PursuePlayer();
                break;
            case State.Attack:
                FacePlayer();
                StartCasting();
                break;
            case State.Retreat:
                RetreatFromPlayer();
                break;
            case State.Moving:
                MoveToNewDestination();
                break;
        }
    }

    void DoIdle()
    {

        agent.SetDestination(transform.position);
        animator.SetBool("isWalking", false);
    }
    void PursuePlayer()
    {
        if (CanHitPlayerFromHere())
        {
            currentState = State.Attack;
        }
        else if (IsPlayerTooClose())
        {
            currentState = State.Retreat;
        }
        else
        {
            // Move towards the player
            agent.SetDestination(player.transform.position);
            animator.SetBool("isWalking", true);
        }
    }
    void RetreatFromPlayer()
    {
        Vector3 retreatDirection = transform.position - player.transform.position;
        Vector3 retreatPosition = transform.position + retreatDirection.normalized * retreatDistance;
        agent.SetDestination(retreatPosition);
        animator.SetBool("isWalking", true);

        // Check if we've retreated far enough
        if (Vector3.Distance(transform.position, player.transform.position) > retreatDistance)
        {
            currentState = State.Pursue; // You can change this to State.Idle if you want it to stop after retreating.
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
    void MoveToNewDestination()
    {
        // Move to a new random destination
        Vector3 newDestination = GetRandomPointInCircle(player.transform.position, innerCircleRadius, outerCircleRadius);
        agent.SetDestination(newDestination);
        animator.SetBool("isWalking", true);

        // Check if we've reached the destination
        if (Vector3.Distance(transform.position, newDestination) <= agent.stoppingDistance)
        {
            currentState = State.Attack;  // Go back to the attack state
        }
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

        if(enemyType == "Spider")
        {
            audioManager.PlaySFXEnemy(audioManager.spiderProjectileSound);
        }
        else
        {
            audioManager.PlaySFXEnemy(audioManager.enemyProjectileSound);
        }
    }

    void StartCasting()
    {
        isCasting = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttack", true);

        // Change the state after shooting
        currentState = State.Moving;
    }

    void FinishCasting() //called at the end frame of the casting animation
    {
        isCasting = false;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(FlashDmg());

        if (hp <= 0)
        {
            //agent.SetDestination(agent.transform.position);
            isDead = true;
            animator.SetBool("isDead", true);
            GetComponent<Collider>().enabled = false;
        }

        healthBar.UpdateHealthBar(hp, (float)hpOrig);
        DamageIndicator indicator = Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamageIndicator>();
        indicator.SetDamageText(amount);
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
    IEnumerator FlashDmg()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.15f);
        model.material.color = Color.grey;
    }
    private void StopMoving()
    {
        agent.SetDestination(agent.transform.position);
    }

    private void DeathSound()
    {
        audioManager.PlaySFXEnemy(audioManager.enemyDeathSound);
    }

    private void SpiderWalk()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderWalkSound);
    }

    private void SpiderHiss()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderHissSound);
    }

    private void SpiderDeath()
    {
        audioManager.PlaySFXEnemy(audioManager.spiderDeathSound);
    }
}
