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

    [Header("----- Gun stuff -----")]
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRange;
    [SerializeField] int shootDamage;

    private Renderer[] model;
    public GameObject enemyPrefab;
    bool playerInRange;
    bool destinationChosen;
    float angleToPlayer;
    bool isShooting;
    bool isMelee;
    bool inMeleeRange;
    float stoppingDistanceOrig;
    Vector3 playerDir;
    int hpOrig;


    // Awake is called when the script instance is loaded, before Start()
    public void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<floatingHealthBar>();
        gameManager.instance.enemyCheckIn();
        stoppingDistanceOrig = agent.stoppingDistance;
        hpOrig = hp;
        healthBar.UpdateHealthBar(hp, hpOrig);
        model = GetComponentsInChildren<Renderer>();
        enemyHPBar.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        pursuePlayer();
    }

    void pursuePlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (CanSeePlayer())
        {
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                FacePlayer();
                if(shootRange <= agent.remainingDistance)
                {
                    if (!isShooting && meleeOrRange)
                    {
                        StartCoroutine(Shoot());
                    }
                    else if (!isMelee && !meleeOrRange)
                    {
                        StartCoroutine(Melee());
                    }
                }
                else if(shootRange >= agent.remainingDistance){
                    StopCoroutine(Shoot());
                    StopCoroutine(Melee());
                }
            }
        }
    }

    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    bool CanSeePlayer()
    {
        agent.stoppingDistance = stoppingDistanceOrig;

        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        //Debug.Log(angleToPlayer);

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

    IEnumerator Shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation).SetActive(true);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator Melee()
    {
        isMelee = true;
        IDamage playerDam = gameManager.instance.player.GetComponent<IDamage>();
        if(playerDam != null)
        {
            playerDam.TakeDamage(shootDamage);
        }
        yield return new WaitForSeconds(shootRate);
        isMelee = false;
        
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
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(FlashDamage());
        healthBar.UpdateHealthBar((float)hp, hpOrig);
        //flashes the enemy hp bar above their heads for a fraction of a second.
        //will probably be changed with testing
        StartCoroutine(showTempHp());

        if (hp <= 0)
        {
            gameManager.instance.enemyCheckOut();
            Destroy(gameObject);
            enemyHPBar.SetActive(false);
        }
    }

    IEnumerator FlashDamage()
    {
        Color orig = model[0].material.color;
        foreach (Renderer i in model) i.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        foreach (Renderer i in model) i.material.color = orig;
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.5f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamTimer);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += gameObject.transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
            destinationChosen = false;
        }
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
