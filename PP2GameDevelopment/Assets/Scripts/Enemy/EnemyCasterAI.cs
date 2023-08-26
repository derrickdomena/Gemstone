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

    [Header("----- State Transition Buffers -----")]
    [SerializeField] float retreatBufferDistance = 1.0f;
    [SerializeField] float pursueBufferDistance = 1.0f;

    [Header("----- State Transition Delay -----")]
    [SerializeField] float attackStateDuration = 2.0f;  // Time enemy spends in the attack state before re-evaluating
    float lastAttackStartTime;

    [Header("----- Navigation Buffer -----")]
    [SerializeField] float arrivalBuffer = 0.5f;

    GameObject player;
    Vector3 directionToPlayer;
    float stoppingDistOrig;
    bool isCasting;
    int hpOrig;

    private Vector3 circleCenter;
    Vector3 startPOS;

    public GameObject damageText;
    bool isDead;

    AudioManager audioManager;
    [SerializeField] int roamDistance;
    bool needsNewDestination = true;
    bool hasReachedDestination = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<floatingHealthBar>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPOS = transform.position;
        player = gameManager.instance.player;
        stoppingDistOrig = agent.stoppingDistance;
        hpOrig = hp;
        hp = hp * (int)gameManager.instance.difficulty;
        healthBar.UpdateHealthBar(hp, hpOrig);
        enemyHPBar.SetActive(false);
        agent.speed = gameManager.instance.playerScript.walkSpeed * speedMod;
    }
    void Update()
    {
        CheckForPlayer();
    }


    void CheckForPlayer()
    {
        FacePlayer();

        if (hasReachedDestination && Time.time - lastAttackStartTime > attackStateDuration)
        {
            ShootAtPlayer();
            lastAttackStartTime = Time.time;
        }
    }


    void ShootAtPlayer()
    {
        if (!isCasting)  // Assuming enemy shouldn't move or shoot while casting
        {
            animator.SetBool("isAttack", true);        
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * playerFaceSpeed);
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
