using System;
using UnityEngine;

public class Defense : Structure
{
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float projectileRange = 10f;
    [SerializeField] float fireRateHz = 10f;
    [SerializeField] Projectile projectilePrefab; 
    [SerializeField] Transform cannonTransform;

    ObjectPool<Projectile> projectilePool;
    int projectilePoolSize = 64;
    float fireCooldown = 0f;
    
    void Start()
    {
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, projectilePoolSize);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isVisualPreview) return;

        // Perform a 2D circle cast to find enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        if (hits.Length > 0)
        {
            // Find the closest enemy
            Collider2D closest = hits[0];
            float minDist = Vector2.Distance(transform.position, closest.transform.position);
            for (int i = 1; i < hits.Length; i++)
            {
                float dist = Vector2.Distance(transform.position, hits[i].transform.position);
                if (dist < minDist)
                {
                    closest = hits[i];
                    minDist = dist;
                }
            }
            // Look at the closest enemy (2D)
            Vector3 direction = closest.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            cannonTransform.rotation = Quaternion.Euler(0, 0, angle);

            TryFire();
        }
    }

    void TryFire()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Projectile projectile = projectilePool.Get(cannonTransform.position, cannonTransform.rotation);
            projectile.Initialize(projectileRange, projectilePool);
            
            fireCooldown = 1f / fireRateHz;
        }
    }
}
