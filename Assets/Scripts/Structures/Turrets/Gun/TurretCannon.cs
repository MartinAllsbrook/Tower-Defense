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
    Stats stats;
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
            TryFire(stats.GetStat((int)GunStat.FireRate));
    }

    public void Initialize(Stats stats, float firingOffset)
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
        
        float accuracy = Mathf.Clamp(stats.GetStat(GunStat.Accuracy), 0f, 1f);
        float inaccuracy =  (1f - accuracy) * 45f; // Max 10 degrees of inaccuracy
        Quaternion rotation = muzzleTransform.rotation * Quaternion.Euler(0f, 0f, Random.Range(-inaccuracy / 2f, inaccuracy / 2f));

        Projectile projectile = projectilePool.Get(muzzleTransform.position, rotation);
        
        projectile.Initialize(stats.GetStat(GunStat.Range), stats.GetStat(GunStat.ProjectileSpeed), stats.GetStat(GunStat.Damage), stats.GetStat(GunStat.Penetration), projectilePool);

        AudioManager.PlayAudioAt(firingSound, transform.position);

        float timeBetweenShots = 1f / stats.GetStat(GunStat.FireRate);

        animator.SetTrigger("Fire");
        animator.speed = stats.GetStat(GunStat.FireRate);

        fireCooldown = timeBetweenShots;
    }
}