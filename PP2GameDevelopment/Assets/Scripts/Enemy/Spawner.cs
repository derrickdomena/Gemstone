using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPOS;

    public bool isSpawning;

    public void SpawnSingleEnemy(GameObject objectToSpawn)
    {
        isSpawning = true;
        gameManager.instance.updateGameGoal(1);

        Instantiate(objectToSpawn, spawnPOS[Random.Range(0, spawnPOS.Length)].position, transform.rotation);

        isSpawning = false;

    }
}