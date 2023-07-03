using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;

    [Header("----- Stats -----")]
    [Range(1, 50)][SerializeField] int hp;
    [Range(1, 20)][SerializeField] int moveSpeed;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;

    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        
        return false;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        StartCoroutine(FlashDamage());

        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator FlashDamage()
    {
        Color orig = model.material.color;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = orig;
    }
}
