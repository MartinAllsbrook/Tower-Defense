using System;
using UnityEngine;

public class Defense : Structure
{
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask obstacleLayers;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float projectileRange = 10f;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float fireRateHz = 10f;
    [SerializeField] Projectile projectilePrefab; 
    [SerializeField] Transform cannonTransform;

    ObjectPool<Projectile> projectilePool;
    int projectilePoolSize = 64;
    float fireCooldown = 0f;
    Target target;
    
    void Start()
    {
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, projectilePoolSize);
        target = FindFirstObjectByType<Target>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isVisualPreview) return;

        Collider2D closestEnemy = FindClosestEnemyWithLineOfSight();
        if (closestEnemy != null)
        {
            // Predictive aiming
            Vector2 enemyVelocity = closestEnemy.attachedRigidbody.linearVelocity;
            Debug.Log("Enemy Velocity: " + enemyVelocity);
            float distanceToEnemy = Vector2.Distance(transform.position, closestEnemy.transform.position);
            float timeToImpact = distanceToEnemy / projectileSpeed;
            Vector2 predictedPosition = (Vector2)closestEnemy.transform.position + enemyVelocity * timeToImpact;
            
            // Rotate cannon towards predicted position
            Vector2 direction = predictedPosition - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            cannonTransform.rotation = Quaternion.Euler(0, 0, angle);

            TryFire();
        }
    }

    Collider2D FindClosestEnemyWithLineOfSight()
    {
        // Perform a 2D circle cast to find enemies in range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        if (enemiesInRange.Length == 0) return null;

        Collider2D closest = null;
        float minDist = float.MaxValue;

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            Vector2 direction = enemiesInRange[i].transform.position - transform.position;
            float dist = direction.magnitude;

            LayerMask combinedMask = enemyLayer | obstacleLayers;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, dist, combinedMask);

            // Check if raycast hit the enemy (not blocked by obstacles)
            if (hit.collider == enemiesInRange[i])
            {
                // Calculate distance to both turret and target, use the minimum
                float distToTurret = dist;
                float distToTarget = target != null ? 
                    Vector2.Distance(enemiesInRange[i].transform.position, target.transform.position) : 
                    float.MaxValue;
                float minDistToEither = Mathf.Min(distToTurret, distToTarget);

                if (minDistToEither < minDist)
                {
                    closest = enemiesInRange[i];
                    minDist = minDistToEither;
                }
            }
        }

        return closest;
    }

    void TryFire()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Projectile projectile = projectilePool.Get(cannonTransform.position, cannonTransform.rotation);
            projectile.Initialize(projectileRange, projectileSpeed, projectilePool);
            
            fireCooldown = 1f / fireRateHz;
        }
    }
}
