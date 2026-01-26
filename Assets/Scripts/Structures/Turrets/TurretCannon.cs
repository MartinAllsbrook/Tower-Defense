using UnityEngine;

class Cannon : MonoBehaviour
{
    [SerializeField] Transform muzzleTransform;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] VariedAudioClip firingSound;

    Animator animator;
    float fireCooldown = 0f;
    bool isFiring = false;

    // References passed from Turret
    TurretStats<GunTurretStat> stats;
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
            TryFire(stats.GetStat(GunTurretStat.FireRate));
    }

    public void Initialize( TurretStats<GunTurretStat> stats, float firingOffset)
    {
        this.stats = stats;
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
        Debug.Log("Firing cannon"); 
        Projectile projectile = projectilePool.Get(muzzleTransform.position, muzzleTransform.rotation);
        projectile.Initialize(stats.GetStat(GunTurretStat.Range), stats.GetStat(GunTurretStat.ProjectileSpeed), stats.GetStat(GunTurretStat.Damage), projectilePool);

        AudioManager.PlayAudioAt(firingSound, transform.position);

        float timeBetweenShots = 1f / stats.GetStat(GunTurretStat.FireRate);

        animator.SetTrigger("Fire");
        animator.speed = stats.GetStat(GunTurretStat.FireRate);

        fireCooldown = timeBetweenShots;
    }
}