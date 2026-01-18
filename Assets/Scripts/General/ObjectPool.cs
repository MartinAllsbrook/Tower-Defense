using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool for Unity GameObjects to reduce instantiation overhead
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Queue<T> availableObjects;
    private readonly List<T> allObjects;
    private readonly Transform parent;
    private readonly int initialSize;
    private readonly int maxSize;
    private readonly bool autoExpand;

    /// <summary>
    /// Creates a new object pool
    /// </summary>
    /// <param name="prefab">The prefab to pool</param>
    /// <param name="initialSize">Initial number of objects to create</param>
    /// <param name="maxSize">Maximum pool size (0 = unlimited)</param>
    /// <param name="parent">Parent transform for pooled objects</param>
    /// <param name="autoExpand">Automatically create new objects when pool is empty</param>
    public ObjectPool(T prefab, int initialSize = 10, int maxSize = 0, Transform parent = null, bool autoExpand = true)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        this.maxSize = maxSize;
        this.parent = parent;
        this.autoExpand = autoExpand;

        availableObjects = new Queue<T>();
        allObjects = new List<T>();

        // Pre-instantiate initial pool
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// Gets an object from the pool
    /// </summary>
    /// <returns>Pooled object instance</returns>
    public T Get()
    {
        T obj;

        // Try to get from available pool
        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        else if (autoExpand && (maxSize == 0 || allObjects.Count < maxSize))
        {
            // Create new object if auto-expand is enabled
            obj = CreateNewObject();
        }
        else
        {
            Debug.LogWarning($"Object pool exhausted for {prefab.name}. Consider increasing pool size.");
            return null;
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Gets an object from the pool at a specific position and rotation
    /// </summary>
    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = Get();
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;
    }

    /// <summary>
    /// Returns an object to the pool
    /// </summary>
    /// <param name="obj">Object to return</param>
    public void Return(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Attempting to return null object to pool");
            return;
        }

        if (!allObjects.Contains(obj))
        {
            Debug.LogWarning($"Attempting to return object {obj.name} that doesn't belong to this pool");
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        availableObjects.Enqueue(obj);
    }

    /// <summary>
    /// Returns an object to the pool after a delay
    /// </summary>
    public void ReturnAfterDelay(T obj, float delay)
    {
        if (obj != null)
        {
            obj.StartCoroutine(ReturnAfterDelayCoroutine(obj, delay));
        }
    }

    private System.Collections.IEnumerator ReturnAfterDelayCoroutine(T obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Return(obj);
    }

    /// <summary>
    /// Clears the pool and destroys all objects
    /// </summary>
    public void Clear()
    {
        foreach (T obj in allObjects)
        {
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }

        availableObjects.Clear();
        allObjects.Clear();
    }

    /// <summary>
    /// Preloads additional objects into the pool
    /// </summary>
    /// <param name="count">Number of objects to preload</param>
    public void Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (maxSize == 0 || allObjects.Count < maxSize)
            {
                CreateNewObject();
            }
        }
    }

    private T CreateNewObject()
    {
        T obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        allObjects.Add(obj);
        availableObjects.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// Gets the current pool statistics
    /// </summary>
    public (int total, int available, int active) GetStats()
    {
        int total = allObjects.Count;
        int available = availableObjects.Count;
        int active = total - available;
        return (total, available, active);
    }
}
