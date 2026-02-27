using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Combat")]
    public GameObject projectilePrefab;
    public float fireRate = 1f;     // Shots per second
    public float range = 6f;        // Detection radius

    [Header("Visuals")]
    public Transform turretPivot;   // Child object that rotates to face enemies

    private float _fireCooldown;

    private void Update()
    {
        _fireCooldown -= Time.deltaTime;

        Transform target = FindNearestEnemy();

        if (target != null)
        {
            if (turretPivot != null)
            {
                Vector2 dir = target.position - turretPivot.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                turretPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            if (_fireCooldown <= 0f)
            {
                Shoot(target);
                _fireCooldown = 1f / fireRate;
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            // Include enemies already at the tower wall — they are within range by definition
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

        Vector3 spawnPos = turretPivot != null ? turretPivot.position : transform.position;
        GameObject projGO = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
            proj.SetTarget(target);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
