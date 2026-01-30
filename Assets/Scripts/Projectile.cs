using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    [SerializeField] PostmortemParticles impactEffectPrefab;
    PostmortemParticles impactEffect;
    Vector3 startPosition;
    bool isInitialized = false;
    ObjectPool<Projectile> pool;

    // Stats
    float maxRange;
    float speed;
    float damage;
    float penetration;

    Rigidbody2D rb;

    void Awake()
    {
        impactEffect = Instantiate(impactEffectPrefab);
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float range, float speed, float damage, float penetration, ObjectPool<Projectile> pool)
    {
        this.pool = pool;
        this.speed = speed;
        this.damage = damage;
        this.penetration = penetration;
        this.maxRange = range;

        startPosition = transform.position;
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        
        // Check if projectile has exceeded its range
        if (isInitialized)
        {
            float distanceTraveled = Vector3.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxRange)
            {
                isInitialized = false;
                pool.Return(this);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                float effectiveDamage = damage;

                if (penetration < 1f)
                {
                    effectiveDamage *= penetration; // Reduce damage based on penetration
                }

                enemyHealth.DecreaseHealth(effectiveDamage); // Apply damage to the enemy
                penetration -= 1f; // Decrease penetration count
            }

            if (penetration <= 0f)
            {
                Kill();
            }
        }
        else
        {
            Kill();
        }
    }

    void Kill()
    {
        impactEffect.transform.position = transform.position;
        impactEffect.gameObject.SetActive(true);

        isInitialized = false;
        pool.Return(this);
    }
}
