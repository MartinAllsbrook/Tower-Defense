using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float angle = 0f;
    public float radius = 2f;
    public float speed = 1f;
    private Vector3 center;

    void Start()
    {
        center = transform.position;
    }

    void Update()
    {
        angle += speed * Time.deltaTime;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        transform.position = center + new Vector3(x, y, 0);
    }
}
