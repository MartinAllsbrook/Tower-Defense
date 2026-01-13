using System;
using UnityEngine;

public class Defense : Structure
{
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float projectileRange = 10f;
    [SerializeField] float fireRateHz = 10f;
    [SerializeField] GameObject projectilePrefab; 
    [SerializeField] Transform cannonTransform;

    GameObject[] projectiles;
    int projectileIndex = 0;
    int projectilePoolSize = 64;
    float fireCooldown = 0f;
    
    void Start()
    {
        projectiles = new GameObject[projectilePoolSize];
        for (int i = 0; i < projectiles.Length; i++)
        {
            projectiles[i] = Instantiate(projectilePrefab);
            projectiles[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
            // Fire a projectile from the pool
            GameObject proj = projectiles[projectileIndex];
            proj.transform.position = cannonTransform.position;
            proj.transform.rotation = cannonTransform.rotation;
            
            // Set the projectile's max range
            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(cannonTransform.position, projectileRange);
            }
            
            proj.SetActive(true);

            // Update index and reset cooldown
            projectileIndex = (projectileIndex + 1) % projectilePoolSize;
            fireCooldown = 1f / fireRateHz;
        }
    }
}
