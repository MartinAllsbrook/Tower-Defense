using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 100f;
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
