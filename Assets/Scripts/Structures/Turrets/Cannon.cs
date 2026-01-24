using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab; 
    [SerializeField] Transform muzzleTransform;
    [SerializeField] Animator animator;

    ObjectPool<Projectile> projectilePool;

    [Header("Audio")]
    [SerializeField] VariedAudioClip firingSound;

    TurretStats stats;

    float fireCooldown = 0f;

    void Awake()
    {
        stats = GetComponentInParent<TurretStats>();
    }

    void Start()
    {
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);
    }

    public void TryFire()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
            Fire();
    }

    void Fire()
    {
        Projectile projectile = projectilePool.Get(muzzleTransform.position, muzzleTransform.rotation);
        projectile.Initialize(stats.Range, stats.ProjectileSpeed, stats.Damage, projectilePool);

        AudioManager.PlayAudioAt(firingSound, transform.position);

        float timeBetweenShots = 1f / stats.FireRate;

        animator.SetTrigger("Fire");
        animator.speed = stats.FireRate;

        fireCooldown = timeBetweenShots;
    }

    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, (angle - 90f));
    }
}