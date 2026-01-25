using UnityEngine;

public class Gun : MonoBehaviour
{


    [SerializeField] Projectile projectilePrefab; 
    [SerializeField] Transform muzzleTransform;
    [SerializeField] Animator animator;


    [Header("Audio")]
    [SerializeField] VariedAudioClip firingSound;

    bool isFiring = false;
    Cannon[] cannons;

    void Awake()
    {
        cannons = GetComponentsInChildren<Cannon>();
        foreach (var cannon in cannons)
        {
            cannon.Initialize(firingSound, GetComponentInParent<TurretStats>(), null, 0f);
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

    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}