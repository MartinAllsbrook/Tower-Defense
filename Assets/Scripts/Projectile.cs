using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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

    void Awake()
    {
        impactEffect = Instantiate(impactEffectPrefab);
    }

    public void Initialize(float range, float speed, float damage, ObjectPool<Projectile> pool)
    {
        this.pool = pool;
        startPosition = transform.position;
        this.speed = speed;
        maxRange = range;
        this.damage = damage;
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
                gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DecreaseHealth(damage); // Apply damage to the enemy
            }
        }

        impactEffect.transform.position = transform.position;
        impactEffect.gameObject.SetActive(true);

        isInitialized = false;
        pool.Return(this);
    }
}
