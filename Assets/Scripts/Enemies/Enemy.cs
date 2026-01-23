using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    // Serialized fields
    [Header("References")]
    [SerializeField] GameObject spriteObject;
    [SerializeField] GameObject[] legObjects;
    [SerializeField] GameObject deathFXPrefab;
    [SerializeField] AudioPlayer bugMoveAudioPlayer;
    
    [Header("Stats")]
    [SerializeField] float radius = 2f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] int moneyReward = 5;

    [Header("Audio")]
    [SerializeField] VariedAudioClip bugHitSound;
    [SerializeField] VariedAudioClip bugDeathSound;
    [SerializeField] VariedAudioClip bugMoveSound;

    // Private fields
    Health health;
    Coroutine moveCoroutine;

    // Events / Public properties
    public event Action<Enemy> OnEnemyDestroyed;
    public float AttackDamage => attackDamage;
    public float AttackInterval => attackInterval;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.OnDeath += KillEnemy;
        World.Instance.OnWorldUpdate += OnUpdateGrid;
    }

    void OnDisable()
    {
        health.OnDeath -= KillEnemy;
        World.Instance.OnWorldUpdate -= OnUpdateGrid;
    }

    void Start()
    {   
        AnimateLegs();
        bugMoveAudioPlayer.PlayRepeating(bugMoveSound);
    }

    void AnimateLegs()
    {
        // Randomize leg animation starting points
        foreach (GameObject leg in legObjects)
        {
            Animation anim = leg.GetComponent<Animation>();
            if (anim != null && anim.clip != null)
            {
                anim[anim.clip.name].time = UnityEngine.Random.Range(0f, anim.clip.length);
                anim.Play();
            }
        }
    }

    void KillEnemy()
    {
        // Death FX // TODO: Object pooling
        Instantiate(deathFXPrefab, transform.position, Quaternion.identity);

        // Audio
        AudioManager.PlayAudioAt(bugDeathSound, transform.position);
        bugMoveAudioPlayer.StopRepeating();

        // Events / Communication
        OnEnemyDestroyed?.Invoke(this);
        Player.Instance.AddMoney(moneyReward);
        
        Destroy(gameObject);
    }

    private void OnUpdateGrid()
    {
        // Debug.Log("World grid updated, recalculating path.");
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            // MoveToTarget();
        }
    }
}
