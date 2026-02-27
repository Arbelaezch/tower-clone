using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnInterval = 2f;    // Seconds between spawns
    public float spawnRadius = 12f;     // Distance from tower centre to spawn point

    // Tower centre to spawn around — auto-finds if left empty
    public Transform towerTransform;

    private float _timer;

    private void Start()
    {
        if (towerTransform == null)
        {
            Tower tower = FindFirstObjectByType<Tower>();
            if (tower != null) towerTransform = tower.transform;
        }
    }

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

        Vector3 centre = towerTransform != null ? towerTransform.position : Vector3.zero;

        // Pick a random angle and project outward to the spawn radius
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnPos = centre + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;

        GameObject enemyGO = EnemyPool.Instance.Get(spawnPos, Quaternion.identity);

        // Tell the enemy which transform to walk toward
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        if (enemy != null)
            enemy.SetTarget(towerTransform != null ? towerTransform : transform);
    }
}
