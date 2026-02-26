using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public int damage = 1;

    private Transform _target;
    private Vector2 _lastDirection = Vector2.right; // Fallback direction if target is gone

    /// <summary>Call this right after instantiating the projectile.</summary>
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        Vector2 direction;

        if (_target != null)
        {
            // Home in on the live target and remember the direction
            direction = (_target.position - transform.position).normalized;
            _lastDirection = direction;
        }
        else
        {
            // Target is gone — keep flying in the last known direction
            direction = _lastDirection;
        }

        // Move
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Rotate to face direction of travel (optional visual touch)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Check arrival (only when target is still alive)
        if (_target != null && Vector2.Distance(transform.position, _target.position) < 0.15f)
        {
            HitTarget();
        }

        // Destroy if it flies off-screen
        Vector3 p = transform.position;
        if (p.x > 14f || p.x < -14f || p.y > 10f || p.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void HitTarget()
    {
        if (_target != null)
        {
            Enemy enemy = _target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
