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
    int currentPathIndex = -1;
    bool isFollowingPath = false;
    Vector2? lookAtTarget = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Handle path following and movement
        if (isFollowingPath && path != null && currentPathIndex >= 0 && currentPathIndex < path.Length)
        {
            Vector2 nextPos = path[currentPathIndex];
            
            if (Vector2.Distance(rb.position, nextPos) <= maxOffset)
            {
                // Reached current waypoint, move to next
                currentPathIndex++;
                if (currentPathIndex >= path.Length)
                {
                    // Path complete
                    isFollowingPath = false;
                    velocity = Vector2.zero;
                }
            }
            else
            {
                // Move toward current waypoint
                velocity = Vector2.Lerp(velocity, (nextPos - rb.position).normalized * speed, velocitySmoothing * Time.fixedDeltaTime);
                Vector2 newPosition = rb.position + velocity * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
        }

        // Handle rotation (independent of path following)
        Vector2? targetToLookAt = lookAtTarget;
        
        // If no custom lookAtTarget is set, use next waypoint from path
        if (!targetToLookAt.HasValue && isFollowingPath && path != null && currentPathIndex >= 0 && currentPathIndex < path.Length)
        {
            targetToLookAt = path[currentPathIndex];
        }

        // Apply rotation if we have a target to look at
        if (targetToLookAt.HasValue)
        {
            float targetAngle = Mathf.Atan2(targetToLookAt.Value.y - rb.position.y, targetToLookAt.Value.x - rb.position.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSmoothing * Time.fixedDeltaTime);
            rb.MoveRotation(angle);
        }
    }

    public IEnumerator FollowPath(Vector2[] newPath)
    {
        path = newPath;

        if (path == null || path.Length == 0)
            yield break;

        currentPathIndex = 0;
        isFollowingPath = true;

        // Wait until path following is complete
        while (isFollowingPath)
        {
            yield return null;
        }

        yield return null;
    }

    public void StopMovement()
    {
        isFollowingPath = false;
        velocity = Vector2.zero;
    }

    public void SetLookAtTarget(Vector2 target)
    {
        lookAtTarget = target;
    }

    public void ClearLookAtTarget()
    {
        lookAtTarget = null;
    }
}