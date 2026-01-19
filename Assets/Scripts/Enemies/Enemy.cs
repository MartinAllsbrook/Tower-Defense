using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] float radius = 2f;
    [SerializeField] GameObject spriteObject;
    [SerializeField] HealthBar healthBar;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] GameObject[] legObjects;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] PostmortemParticles deathParticlesPrefab;
    [SerializeField] RepeatingAudioSource bugHitSound;
    
    public event Action<Enemy> OnEnemyDestroyed;
    public float AttackDamage => attackDamage;
    public float AttackInterval => attackInterval;

    ObjectPool<RepeatingAudioSource> hitSoundPool;
    PostmortemParticles deathParticlesInstance;
    float health = 100f;
    World world;
    Target target;
    Coroutine moveCoroutine;
    bool gameOver = false;
    GameController gameController;

    void Awake()
    {
        target = FindFirstObjectByType<Target>();
        world = FindFirstObjectByType<World>();

        gameController = FindFirstObjectByType<GameController>();
        
    
        deathParticlesInstance = Instantiate(deathParticlesPrefab, this.transform.position, Quaternion.identity);

        hitSoundPool = new ObjectPool<RepeatingAudioSource>(bugHitSound.GetComponent<RepeatingAudioSource>(), 8);
    }

    void OnEnable()
    {
        gameController.OnGameOver += OnGameEnd;
        world.OnWorldUpdate += OnUpdateGrid;
    }

    void OnDisable()
    {
        gameController.OnGameOver -= OnGameEnd;
        world.OnWorldUpdate -= OnUpdateGrid;
    }

    void Start()
    {   
        AnimateLegs();
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

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(10f);
        }
    }

    void OnGameEnd()
    {
        gameOver = true;
    }

    void DecreaseHealth(float amount)
    {
        health -= amount;
        healthBar.SetFill(health / maxHealth);
        
        RepeatingAudioSource hitSound = hitSoundPool.Get(transform.position, Quaternion.identity);

        hitSoundPool.ReturnAfterDelay(hitSound, 0.25f);

        if (health <= 0)
        {
            KillEnemy();
        }
    }

    void KillEnemy()
    {
        deathParticlesInstance.transform.position = transform.position;
        deathParticlesInstance.gameObject.SetActive(true);

        OnEnemyDestroyed?.Invoke(this);
        world.OnWorldUpdate -= OnUpdateGrid;
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
