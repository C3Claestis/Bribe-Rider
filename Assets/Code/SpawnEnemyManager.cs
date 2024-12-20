using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private float minZ, maxZ;
    private float minRandom, maxRandom;
    private float spawnTimer;
    private float timeToNextSpawn;
    private bool isCanSpawn;
    private float setTimer;

    // Menyimpan nilai asli dari minZ dan maxZ
    private float originalMinZ;
    private float originalMaxZ;

    public void SetRandomTime(float min, float max)
    {
        minRandom = min;
        maxRandom = max;
    }

    void Start()
    {
        // Menyimpan nilai asli minZ dan maxZ untuk digunakan dalam perubahan rentang
        originalMinZ = minZ;
        originalMaxZ = maxZ;

        SetRandomSpawnTime();
        isCanSpawn = true;
    }

    void Update()
    {
        setTimer = gameManager.GetTimer();

        AdjustSpawnRange();

        if (isCanSpawn)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= timeToNextSpawn)
            {
                SpawnEnemy();
                spawnTimer = 0f;
                SetRandomSpawnTime();
            }
        }
    }

    private void AdjustSpawnRange()
    {
        // Jika timer kurang dari setengah, rentang minZ dan maxZ jadi setengah
        if (setTimer > gameManager.GetMaxTimer() * 0.25f && setTimer < gameManager.GetMaxTimer() * 0.5f)
        {
            minZ = originalMinZ * 0.5f;
            maxZ = originalMaxZ * 0.5f;
        }
        // Jika timer kurang dari 25%, rentang minZ dan maxZ jadi seperempat
        else if (setTimer < gameManager.GetMaxTimer() * 0.25f)
        {
            minZ = originalMinZ * 0.25f;
            maxZ = originalMaxZ * 0.25f;
        }
        else
        {
            minZ = originalMinZ;
            maxZ = originalMaxZ;
        }
    }

    private void SetRandomSpawnTime()
    {
        if (setTimer > gameManager.GetMaxTimer() * 0.25f && setTimer < gameManager.GetMaxTimer() * 0.5f)
        {
            timeToNextSpawn = Random.Range(minRandom * 0.5f, maxRandom * 0.5f);
        }
        else if (setTimer < gameManager.GetMaxTimer() * 0.25f)
        {
            timeToNextSpawn = Random.Range(minRandom * 0.25f, maxRandom * 0.25f);
        }
        else
        {
            timeToNextSpawn = Random.Range(minRandom, maxRandom);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        if (setTimer > gameManager.GetMaxTimer() * 0.25f && setTimer < gameManager.GetMaxTimer() * 0.5f)
        {
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector3 randomPosition = new Vector3(Random.Range(-20f, 20f), -0.275f, Random.Range(minZ, maxZ));
            Instantiate(enemyToSpawn, randomPosition, Quaternion.identity);
        }
        else if (setTimer < gameManager.GetMaxTimer() * 0.25f)
        {
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, 1)];
            Vector3 randomPosition = new Vector3(Random.Range(-20f, 20f), -0.275f, Random.Range(minZ, maxZ));
            Instantiate(enemyToSpawn, randomPosition, Quaternion.identity);
        }
        else
        {
            GameObject enemyToSpawn = enemyPrefabs[0];
            Vector3 randomPosition = new Vector3(Random.Range(-20f, 20f), -0.275f, Random.Range(minZ, maxZ));
            Instantiate(enemyToSpawn, randomPosition, Quaternion.identity);
        }
    }

    public void SetIsCanSpawn(bool spawn) => this.isCanSpawn = spawn;
}
