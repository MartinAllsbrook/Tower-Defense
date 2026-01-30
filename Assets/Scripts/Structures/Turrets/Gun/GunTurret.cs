using UnityEngine;

class GunTurret : Turret<GunStat>
{
    bool isFiring = false;
    [SerializeField] Cannon[] cannons;
    [SerializeField] VariedAudioClip firingSound;

    protected override void Awake()
    {
        base.Awake();
        foreach (var cannon in cannons)
        {
            cannon.Initialize(stats, 0f);
        }
    }

    protected override void Update()    
    {
        base.Update();
        if (isVisualPreview) return;

        Collider2D closestEnemy = FindClosestEnemyWithLineOfSight(stats.GetStat(GunStat.Range));
        if (closestEnemy != null)
        {
            // Predictive aiming
            Vector2 enemyVelocity = closestEnemy.attachedRigidbody.linearVelocity;
            float distanceToEnemy = Vector2.Distance(transform.position, closestEnemy.transform.position);
            float timeToImpact = distanceToEnemy / stats.GetStat((int)GunStat.ProjectileSpeed);
            Vector2 predictedPosition = (Vector2)closestEnemy.transform.position + enemyVelocity * timeToImpact;
            
            // Rotate cannon towards predicted position
            Vector2 direction = predictedPosition - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            swivel.SetRotation(angle);
            SetFiring(true);
        }
        else    
        {
            SetFiring(false);
        }
    }

    public void AddCannon()
    {
        // TODO: Implement cannon addition
        // Cannon newCannon = Instantiate(cannons[0], transform);
    }

    public void StartFiring()
    {
        foreach (var cannon in cannons)
        {
            cannon.StartFiring();
        }
    }

    public void StopFiring()
    {
        foreach (var cannon in cannons)
        {
            cannon.StopFiring();
        }
    }

    public void SetFiring(bool firing)
    {
        if (firing && !isFiring)
        {
            StartFiring();
            isFiring = true;
        }
        else if (!firing && isFiring)
        {
            StopFiring();
            isFiring = false;
        }
    } 
    
}