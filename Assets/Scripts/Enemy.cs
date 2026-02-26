using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public int health = 1;

    private int _maxHealth;

    private void Awake()
    {
        // Cache the starting health so OnEnable can reset it
        _maxHealth = health;
    }

    private void OnEnable()
    {
        // Reset health every time the enemy is pulled from the pool
        health = _maxHealth;
    }

    private void Update()
    {
        // Move left across the screen
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        // Return to pool if it goes off the left edge
        if (transform.position.x < -12f)
        {
            ReturnToPool();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // You can add particle effects / sound here later
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (EnemyPool.Instance != null)
            EnemyPool.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject); // Fallback if no pool exists
    }
}
