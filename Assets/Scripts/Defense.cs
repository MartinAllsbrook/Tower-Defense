using UnityEngine;

public class Defense : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Range to search for enemies
    [SerializeField] private float detectionRange = 5f;

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
        }
    }
}
