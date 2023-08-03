using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Wave[] waves;

    private Wave currentWave;

    [SerializeField] float waveTimer;

    private int i = 0;
    int wavesLeft;

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
        if (stopSpawning || gameManager.instance.enemiesRemaining != 0)
        {
            return;
        }

        StartCoroutine(Countdown());
    }

    private void SpawnWave()
    {
        /*if (Spawner.ActiveSpawners.Count > 0)
        {
            Spawner spawner = Spawner.ActiveSpawners[Random.Range(0, Spawner.ActiveSpawners.Count)];

            for (int i = 0; i < currentWave.NumberToSpawn; i++)
            {
                int num = Random.Range(0, currentWave.EnemiesInWave.Length);
                spawner.SpawnSingleEnemy(currentWave.EnemiesInWave[num]);
            }
        }*/
    }

    private void IncWave()
    {
        if (i + 1 < waves.Length)
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
        wavesLeft = waves.Length - 1 - i;
        yield return new WaitForSeconds(waveTimer);

        if (gameManager.instance.enemiesRemaining == 0)
        {
            SpawnWave();
            gameManager.instance.wavesLeftText.text = wavesLeft.ToString();
            gameManager.instance.wave++;
            IncWave();
        }

        gameManager.instance.nextWave.SetActive(false);
    }
}
