using UnityEngine;

public class EnemyDeathFX : MonoBehaviour
{
    [SerializeField] float fxLifetime = 1.5f;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        Invoke(nameof(DestroyFX), fxLifetime);
    }

    void DestroyFX()
    {
        Destroy(gameObject);
    }
}
