using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ParticleSystem impactEffectPrefab;
    private ParticleSystem impactEffect;

    [SerializeField] float speed = 10f;
    Vector3 startPosition;
    float maxRange;
    bool isInitialized = false;

    void Awake()
    {
        impactEffect = Instantiate(impactEffectPrefab);
        impactEffect.gameObject.SetActive(false);
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
        impactEffect.Play();
        StartCoroutine(DisableImpactEffectAfterDuration(impactEffect.main.duration));
        
        isInitialized = false;
        gameObject.SetActive(false);
    }

    IEnumerator DisableImpactEffectAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        impactEffect.gameObject.SetActive(false);
    }
}
