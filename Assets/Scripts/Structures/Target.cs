using UnityEngine;

public class Target : Structure
{
    [SerializeField] private HealthBar healthBar;

    public void DealDamage(float damage)
    {
        health -= damage;
        healthBar.SetFill(health / 100f);
        if (health <= 0f)
        {
            Destroy();
        }
    }

    void Destroy()
    {
        GameController gameController = FindFirstObjectByType<GameController>();
        gameController.EndGame();
        Destroy(gameObject);
    }
}
