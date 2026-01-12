using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float maxOffset = 0.1f;
    [SerializeField] float rotationSmoothing = 6f;
    [SerializeField] float velocitySmoothing = 6f;
    
    Vector2[] path;
    Rigidbody2D rb;
    Vector2 velocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPath(Vector2[] newPath)
    {
        path = newPath;
    }

    public IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0)
            yield break;

        for (int i = 0; i < path.Length; i++)
        {
            Vector2 nextPos = path[i];
            while (Vector2.Distance(rb.position, nextPos) > maxOffset)
            {
                velocity = Vector2.Lerp(velocity, (nextPos - rb.position).normalized * speed, velocitySmoothing * Time.deltaTime);

                Vector2 newPosition = rb.position + velocity * Time.deltaTime;

                rb.MovePosition(newPosition);

                float targetAngle = Mathf.Atan2(nextPos.y - rb.position.y, nextPos.x - rb.position.x) * Mathf.Rad2Deg;
                float angle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSmoothing * Time.deltaTime);
                rb.MoveRotation(angle);
                yield return null;
            }
        }
    }
}