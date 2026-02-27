using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;

    [Header("Combat")]
    public int health = 1;
    public float damagePerSecond = 1f;  // Damage dealt to tower per second while touching

    /// <summary>
    /// Half the width of the enemy sprite (default 1x1 square = 0.5).
    /// Adjust if you change the enemy's scale.
    /// </summary>
    public float radius = 0.5f;

    // Session ID — increments on each pool reuse so projectiles can detect recycling
    public int SessionId { get; private set; }

    private int _maxHealth;
    private Transform _towerTransform;
    private TowerHealth _towerHealth;
    private float _stopDistance;        // Computed once when target is set
    private bool _atTower;              // True once the enemy has reached the tower
    private float _damageAccumulator;

    private void Awake()
    {
        _maxHealth = health;
    }

    private void OnEnable()
    {
        SessionId++;
        health = _maxHealth;
        _atTower = false;
        _damageAccumulator = 0f;
    }

    /// <summary>Called by EnemySpawner after pulling from the pool.</summary>
    public void SetTarget(Transform towerTransform)
    {
        _towerTransform = towerTransform;
        _towerHealth = towerTransform != null ? towerTransform.GetComponent<TowerHealth>() : null;

        // Stop distance = enemy radius + tower radius so sprites sit edge-to-edge
        float towerRadius = 0.5f; // Default 1x1 tower sprite
        if (towerTransform != null)
        {
            // Use the larger half-extent of the tower's lossy scale as its radius
            Vector3 s = towerTransform.lossyScale;
            towerRadius = Mathf.Max(s.x, s.y) * 0.5f;
        }
        _stopDistance = radius + towerRadius;
    }

    private void Update()
    {
        if (_towerTransform == null) return;

        float dist = Vector2.Distance(transform.position, _towerTransform.position);

        if (!_atTower)
        {
            if (dist > _stopDistance)
            {
                // Walk toward the tower
                Vector2 dir = ((Vector2)_towerTransform.position - (Vector2)transform.position).normalized;
                transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                // Arrived — snap to the exact edge and stay there
                Vector2 dir = ((Vector2)transform.position - (Vector2)_towerTransform.position).normalized;
                transform.position = (Vector2)_towerTransform.position + dir * _stopDistance;
                _atTower = true;
            }
        }
        else
        {
            // Keep position locked to the tower edge (handles the tower moving in future)
            Vector2 dir = ((Vector2)transform.position - (Vector2)_towerTransform.position).normalized;
            transform.position = (Vector2)_towerTransform.position + dir * _stopDistance;

            // Deal damage over time
            if (_towerHealth != null)
            {
                _damageAccumulator += damagePerSecond * Time.deltaTime;
                if (_damageAccumulator >= 1f)
                {
                    int dmg = Mathf.FloorToInt(_damageAccumulator);
                    _towerHealth.TakeDamage(dmg);
                    _damageAccumulator -= dmg;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    private void Die()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (EnemyPool.Instance != null)
            EnemyPool.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
