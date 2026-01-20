using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] PostmortemParticles impactEffectPrefab;
    [SerializeField] AudioSource shotSound;

    PostmortemParticles impactEffect;
    Vector3 startPosition;
    float maxRange;
    float speed;
    bool isInitialized = false;
    ObjectPool<Projectile> pool;

    void Awake()
    {
        impactEffect = Instantiate(impactEffectPrefab);
    }

    void OnEnable()
    {
        shotSound.pitch = Random.Range(0.9f, 1.1f);
        shotSound.volume = Random.Range(0.4f, 0.5f);
    }

    public void Initialize(float range, float speed, ObjectPool<Projectile> pool)
    {
        this.pool = pool;
        startPosition = transform.position;
        this.speed = speed;
        maxRange = range;
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
        impactEffect.transform.position = transform.position;
        impactEffect.gameObject.SetActive(true);

        isInitialized = false;
        pool.Return(this);
    }
}
