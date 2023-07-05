using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    floatingHealthBar healthBar;

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] int hp;
    [Range(1, 20)][SerializeField] int moveSpeed;
    [Range(1, 180)][SerializeField] int viewAngle;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [SerializeField] int roamTimer;
    [SerializeField] int roamDist;

    [Header("----- Gun stuff -----")]
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRange;
    [SerializeField] int shootDamage;

    public GameObject enemyPrefab;
    bool playerInRange;
    bool destinationChosen;
    float angleToPlayer;
    bool isShooting;
    float stoppingDistanceOrig;
    Vector3 playerDir;
    Vector3 startingPos;
    int hpOrig;

    void Awake()
    {
        healthBar = GetComponentInChildren<floatingHealthBar>();
        Debug.Log(healthBar);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
        spawnEnemies();
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;
        hpOrig = hp;
        //healthBar.UpdateHealthBar(hp, hpOrig);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !CanSeePlayer())
        {
            StartCoroutine(roam());
        }
        else if (agent.destination != gameManager.instance.player.transform.position)
        {
            StartCoroutine(roam());

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
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    FacePlayer();
                }

                if (!isShooting)
                {
                    StartCoroutine(Shoot());
                }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
            startingPos = transform.position;
        }
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(FlashDamage());
        healthBar.UpdateHealthBar(hp, hpOrig);

        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    public void spawnEnemies()
    {
        int remainingSpawn = gameManager.instance.enemyCount;
        
        for (int i = 0; i < gameManager.instance.enemySpawnLocs.Length; i++)
        {
            int enemyRandomSpawn = Random.Range(1, gameManager.instance.enemySpawnLocs.Length);
            Instantiate(enemyPrefab, gameManager.instance.enemySpawnLocs[enemyRandomSpawn].transform.position, Quaternion.identity);
            remainingSpawn--;
        }

        // spawn based on number in room after deadline
    }

    IEnumerator FlashDamage()
    {
        Color orig = model.material.color;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = orig;
    }

    IEnumerator roam()
    {
        if (agent.remainingDistance < 0.5f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamTimer);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
            destinationChosen = false;
        }
    }
}
