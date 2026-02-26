using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton pool for Enemy GameObjects.
/// Instead of Instantiate/Destroy, enemies are taken from and returned to this pool.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Header("Pool Settings")]
    public GameObject enemyPrefab;
    public int initialPoolSize = 20;    // Pre-warmed enemies on Start

    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Pre-warm the pool so there are no allocations on the first spawns
        for (int i = 0; i < initialPoolSize; i++)
        {
            _pool.Enqueue(CreateNewEnemy());
        }
    }

    /// <summary>Get an enemy from the pool, positioned and activated.</summary>
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject enemy = _pool.Count > 0 ? _pool.Dequeue() : CreateNewEnemy();

        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.SetActive(true);
        return enemy;
    }

    /// <summary>Return an enemy to the pool instead of destroying it.</summary>
    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        _pool.Enqueue(enemy);
    }

    private GameObject CreateNewEnemy()
    {
        GameObject go = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, transform);
        go.SetActive(false);
        return go;
    }
}
