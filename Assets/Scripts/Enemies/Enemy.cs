using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject spriteObject;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject[] legObjects;
    [SerializeField] GameObject deathFXPrefab;
    [SerializeField] AudioPlayer bugMoveAudioPlayer;
    
    [Header("Stats")]
    [SerializeField] float radius = 2f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] int moneyReward = 5;

    [Header("Audio")]
    [SerializeField] VariedAudioClip bugHitSound;
    [SerializeField] VariedAudioClip bugDeathSound;
    [SerializeField] VariedAudioClip bugMoveSound;

    public event Action<Enemy> OnEnemyDestroyed;
    public float AttackDamage => attackDamage;
    public float AttackInterval => attackInterval;

    float health = 100f;
    Coroutine moveCoroutine;

    // References to other systems
    World world;
    Player player;

    void Awake()
    {
        world = FindFirstObjectByType<World>();
        player = FindFirstObjectByType<Player>();
    }

    void OnEnable()
    {
        world.OnWorldUpdate += OnUpdateGrid;
    }

    void OnDisable()
    {
        world.OnWorldUpdate -= OnUpdateGrid;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(10f);
        }
    }

    void DecreaseHealth(float amount)
    {
        health -= amount;
        healthBar.SetFill(health / maxHealth);
        
        AudioManager.PlayAudioAt(bugHitSound, transform.position);

        if (health <= 0)
        {
            KillEnemy();
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
        player.AddMoney(moneyReward);
        
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
