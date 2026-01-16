using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] PostmortemParticles impactEffectPrefab;
    private PostmortemParticles impactEffect;

    [SerializeField] float speed = 10f;
    Vector3 startPosition;
    float maxRange;
    bool isInitialized = false;

    void Awake()
    {
        impactEffect = Instantiate(impactEffectPrefab);
    }

    public void Initialize(Vector3 startPos, float range)
    {
        startPosition = startPos;
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
        gameObject.SetActive(false);
    }
}
