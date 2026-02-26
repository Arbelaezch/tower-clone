using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnInterval = 2f;       // Seconds between spawns
    public float spawnX = 11f;             // How far right enemies spawn
    public float spawnYMin = -3f;          // Vertical spawn range min
    public float spawnYMax = 3f;           // Vertical spawn range max

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyPool.Instance == null) return;

        float randomY = Random.Range(spawnYMin, spawnYMax);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0f);
        EnemyPool.Instance.Get(spawnPos, Quaternion.identity);
    }
}
