using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float maxOffset = 0.1f;
    [SerializeField] float rotationSmoothing = 1f;
    [SerializeField] float velocitySmoothing = 1f;
    
    List<Vector2> path;
    Rigidbody2D rb;
    Vector2 velocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPath(List<Vector2> newPath)
    {
        path = newPath;
    }

    public void StartMoving()
    {
        StartCoroutine(FollowPath());
    }

    public IEnumerator FollowPath()
    {
        if (path == null || path.Count == 0)
            yield break;

        foreach (Vector2 waypoint in path)
        {
            Vector2 targetPosition = waypoint;
            while (Vector2.Distance(rb.position, targetPosition) > maxOffset)
            {
                velocity = Vector2.Lerp(velocity, (targetPosition - rb.position).normalized * speed, velocitySmoothing * Time.deltaTime);

                Vector2 newPosition = rb.position + velocity * Time.deltaTime;

                rb.MovePosition(newPosition);

                float targetAngle = Mathf.Atan2(targetPosition.y - rb.position.y, targetPosition.x - rb.position.x) * Mathf.Rad2Deg;
                float angle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSmoothing * Time.deltaTime);
                rb.MoveRotation(angle);
                yield return null;
            }
        }
    }
}