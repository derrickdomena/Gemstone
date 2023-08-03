using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPOS;

    public bool isSpawning;

    public Wave[] waves;

    private void OnEnable()
    {
        if (waves.Length > 0)
        {
            Wave currentWave = waves[Random.Range(0, waves.Length)];
            SpawnWave(currentWave);
        }
    }

    private void SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.NumberToSpawn; i++)
        {
            int num = Random.Range(0, wave.EnemiesInWave.Length);
            SpawnSingleEnemy(wave.EnemiesInWave[num]);
        }
    }

    public void SpawnSingleEnemy(GameObject objectToSpawn)
    {
        isSpawning = true;
        gameManager.instance.updateGameGoal(1);

        Instantiate(objectToSpawn, spawnPOS[Random.Range(0, spawnPOS.Length)].position, transform.rotation);

        isSpawning = false;
    }
}