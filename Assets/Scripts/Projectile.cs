using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public int damage = 1;

    private Enemy _targetEnemy;
    private int _targetSessionId;
    private Vector2 _lastDirection = Vector2.right;

    /// <summary>
    /// Call this right after instantiating the projectile.
    /// Stores both the target and its current session ID so we can detect recycling.
    /// </summary>
    public void SetTarget(Transform target)
    {
        _targetEnemy = target != null ? target.GetComponent<Enemy>() : null;
        _targetSessionId = _targetEnemy != null ? _targetEnemy.SessionId : -1;
    }

    /// <summary>
    /// Returns true only if the target is active AND hasn't been recycled since we fired.
    /// </summary>
    private bool TargetIsAlive()
    {
        return _targetEnemy != null
            && _targetEnemy.gameObject.activeInHierarchy
            && _targetEnemy.SessionId == _targetSessionId;
    }

    private void Update()
    {
        Vector2 direction;

        if (TargetIsAlive())
        {
            direction = ((Vector2)_targetEnemy.transform.position - (Vector2)transform.position).normalized;
            _lastDirection = direction;
        }
        else
        {
            // Target is dead or recycled — fly straight on
            direction = _lastDirection;
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Check arrival only against a still-living target
        if (TargetIsAlive() && Vector2.Distance(transform.position, _targetEnemy.transform.position) < 0.15f)
        {
            HitTarget();
        }

        // Destroy if off-screen
        Vector3 p = transform.position;
        if (p.x > 14f || p.x < -14f || p.y > 10f || p.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void HitTarget()
    {
        if (TargetIsAlive())
        {
            _targetEnemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
