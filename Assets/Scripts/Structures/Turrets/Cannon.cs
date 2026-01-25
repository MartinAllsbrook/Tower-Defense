using UnityEngine;

class Cannon : MonoBehaviour
{
    [SerializeField] Transform muzzleTransform;
    [SerializeField] Projectile projectilePrefab;

    Animator animator;
    float fireCooldown = 0f;
    bool isFiring = false;

    // References passed from Turret
    VariedAudioClip firingSound;
    TurretStats stats;
    ObjectPool<Projectile> projectilePool;
    float firingOffset = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 10, 0);
    }

    void Update()
    {
        if (isFiring)
            TryFire(stats.FireRate);
    }

    public void Initialize(VariedAudioClip firingSound, TurretStats stats, ObjectPool<Projectile> projectilePool, float firingOffset)
    {
        this.firingSound = firingSound;
        this.stats = stats;
        this.projectilePool = projectilePool;
        this.firingOffset = firingOffset;
        Reset();
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
        Reset();
    }

    void Reset()
    {
        fireCooldown = firingOffset;
    }

    // Called on Update when firing
    void TryFire(float fireRate)
    {
        if (fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = 1f / fireRate;
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }
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
}