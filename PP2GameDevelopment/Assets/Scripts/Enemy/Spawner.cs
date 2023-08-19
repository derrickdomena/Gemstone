using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPOS;
    [SerializeField] GameObject particleEffectPrefab;  // Particle effect to play before spawning enemy.
    [SerializeField] float particleEffectDuration = 1f;  // Duration to play the particle effect.

    public bool isSpawning;

    public Wave[] waves;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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

        // Determine the spawn position.
        Transform spawnPoint = spawnPOS[Random.Range(0, spawnPOS.Length)];

        // Play the particle effect.
        GameObject particleInstance = Instantiate(particleEffectPrefab, spawnPoint.position, transform.rotation);
        Destroy(particleInstance, particleEffectDuration);  // Destroy the particle effect after the set duration.

        //Audio
        audioManager.PlaySFXEnemy(audioManager.enemySpawnSound);

        // Start the SpawnAfterDelay coroutine.
        StartCoroutine(SpawnAfterDelay(objectToSpawn, spawnPoint));
    }

    private IEnumerator SpawnAfterDelay(GameObject objectToSpawn, Transform spawnPoint)
    {
        yield return new WaitForSeconds(1f);  // Wait for 0.5 seconds.

        // Spawn the enemy.
        Instantiate(objectToSpawn, spawnPoint.position, transform.rotation);

        isSpawning = false;
    }
}
