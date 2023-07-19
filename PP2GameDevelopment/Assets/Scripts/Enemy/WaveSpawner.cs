using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveSpawner : MonoBehaviour
{
    public Wave[] waves;

    private Wave currentWave;

    [SerializeField] float waveTimer;

    [SerializeField] Spawner spawner;

    private int i = 0;
    
    private bool stopSpawning = false;
    private void Awake()
    {
        currentWave = waves[i];
      
    }
    void Start()
    {
        gameManager.instance.maxWaves = waves.Length;
    }
    private void Update()
    {
        if(stopSpawning)
        {
            return;
        }
        if(gameManager.instance.enemiesRemaining == 0)
        {
            StartCoroutine(Countdown());
        }


    }
    private void SpawnWave()
    {
        for(int i = 0; i < currentWave.NumberToSpawn; i++)
        {
            int num = Random.Range(0, currentWave.EnemiesInWave.Length);          
            spawner.SpawnSingleEnemy(currentWave.EnemiesInWave[num]);
        }
    }
    private void IncWave()
    {
        if(i + 1 < waves.Length)
        {
            i++;
            currentWave = waves[i];
        }
        else
        {
            stopSpawning = true;
        }
    }

    IEnumerator Countdown()
    {
        gameManager.instance.nextWave.SetActive(true);
        yield return new WaitForSeconds(waveTimer);
        if (gameManager.instance.enemiesRemaining == 0)
        {
            SpawnWave();
            gameManager.instance.wave++;
            IncWave();
        }
        gameManager.instance.nextWave.SetActive(false);
    }
}
