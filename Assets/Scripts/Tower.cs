using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combat")]
    public GameObject projectilePrefab;
    public float fireRate = 1f;       // Shots per second
    public float range = 6f;          // Detection radius

    [Header("Visuals")]
    public Transform turretPivot;     // Child object that rotates to face enemies

    private float _fireCooldown;

    private void Update()
    {
        _fireCooldown -= Time.deltaTime;

        Transform target = FindNearestEnemy();

        if (target != null)
        {
            // Rotate turret to face the target
            if (turretPivot != null)
            {
                Vector2 dir = target.position - turretPivot.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                turretPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            // Shoot
            if (_fireCooldown <= 0f)
            {
                Shoot(target);
                _fireCooldown = 1f / fireRate;
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        // Find all enemies in the scene
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist <= range && dist < minDist)
            {
                minDist = dist;
                nearest = e.transform;
            }
        }

        return nearest;
    }

    private void Shoot(Transform target)
    {
        if (projectilePrefab == null) return;

        // Spawn from turret pivot (or tower center if no pivot)
        Vector3 spawnPos = turretPivot != null ? turretPivot.position : transform.position;
        GameObject projGO = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetTarget(target);
        }
    }

    // Draw range gizmo in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
