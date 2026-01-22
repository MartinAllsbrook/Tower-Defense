using System;
using Mono.CSharp;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Serialized fields
    [SerializeField] HealthBar healthBarPrefab;
    [SerializeField] float maxHealth = 100f;

    HealthBar healthBar;

    // Private fields
    float currentHealth;

    // Events
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    void Awake()
    {
        healthBar = Instantiate(healthBarPrefab, transform);
    } 

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetFill(1f);
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        healthBar.SetFill(currentHealth / maxHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void IncreaseHealth(float amount)
    {
        SetHealth(currentHealth + amount);
    }

    public void DecreaseHealth(float amount)
    {
        SetHealth(currentHealth - amount);
    }

    void Die()
    {
        OnDeath?.Invoke();
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
    }
}
