using System;
using UnityEngine;

public class Defense : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float fireRateHz = 10f;
    [SerializeField] private GameObject projectilePrefab; 

    private GameObject[] projectiles;
    private int projectileIndex = 0;
    private int projectilePoolSize = 64;
    private float fireCooldown = 0f;


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
            transform.rotation = Quaternion.Euler(0, 0, angle);

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
            proj.transform.position = transform.position;
            proj.transform.rotation = transform.rotation;
            proj.SetActive(true);

            // Update index and reset cooldown
            projectileIndex = (projectileIndex + 1) % projectilePoolSize;
            fireCooldown = 1f / fireRateHz;
        }
    }
}
