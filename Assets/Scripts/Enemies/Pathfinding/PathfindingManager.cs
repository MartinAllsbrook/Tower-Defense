using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Diagnostics;

/// <summary>
/// Centralized pathfinding manager that batches pathfinding requests to prevent performance spikes.
/// Processes a limited number of pathfinding operations per frame.
/// </summary>
class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }
    
    [SerializeField] int maxRequestsPerFrame = 1;
    [SerializeField] bool useManager = true; // Toggle to compare performance
    
    ThetaStar cachedThetaStar;
    bool gridDirty = true;
    World world;
    
    Queue<PathRequest> pendingRequests = new Queue<PathRequest>();
    List<PathRequest> processingRequests = new List<PathRequest>();
    bool isProcessing = false;
    
    class PathRequest
    {
        public Vector2Int startCell;
        public Vector2Int endCell;
        public System.Action<List<Vector2Int>> callback;
        public bool isCancelled;
    }
    
    #region Lifecycle
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        world = FindFirstObjectByType<World>();
    }
    
    void OnEnable()
    {
        world.OnWorldUpdate += MarkGridDirty;
    }
    
    void OnDisable()
    {
        world.OnWorldUpdate -= MarkGridDirty;
    }
    #endregion
    
    public void MarkGridDirty()
    {
        gridDirty = true;
    }
    
    void Update()
    {
        if (!useManager || isProcessing)
            return;
            
        ProcessPendingRequests();
    }
    
    /// <summary>
    /// Request a path asynchronously. Callback will be invoked when path is ready (on main thread).
    /// </summary>
    public async Awaitable<List<Vector2Int>> RequestPath(Vector2Int startCell, Vector2Int endCell)
    {
        // If were not using the manager, compute path directly
        if (!useManager)
        {
            // Bypass manager and compute immediately
            await EnsureThetaStarReady();
            await Awaitable.BackgroundThreadAsync();
            List<Vector2Int> directPath = cachedThetaStar.CreatePath(startCell, endCell);
            await Awaitable.MainThreadAsync();
            return directPath;
        }
        
        // Create completion source for async/await pattern
        var tcs = new TaskCompletionSource<List<Vector2Int>>();

        PathRequest request = new PathRequest
        {
            startCell = startCell,
            endCell = endCell,
            callback = (path) => tcs.SetResult(path),
            isCancelled = false
        };

        pendingRequests.Enqueue(request);

        // Wait for the result. It will resolve when the callback in the request is invoked, i.e. when the path in the TCS is set.
        return await tcs.Task;
    }
    
    /// <summary>
    /// Checks if ThetaStar is is null or dirty and rebuilds it if necessary.
    /// </summary>
    async Awaitable EnsureThetaStarReady()
    {
        if (gridDirty || cachedThetaStar == null)
        {
            GridCell[,] gridCells = world.GetGrid();
            BoundsInt bounds = world.GetBounds();
            Vector2Int offset = new Vector2Int(bounds.xMin, bounds.yMin);
            
            await Awaitable.BackgroundThreadAsync();
            cachedThetaStar = new ThetaStar(gridCells, offset);
            await Awaitable.MainThreadAsync();
            
            gridDirty = false;
        }
    }
    
    async void ProcessPendingRequests()
    {
        if (pendingRequests.Count == 0)
            return;
        
        isProcessing = true;
        
        try
        {
            // Ensure ThetaStar is ready
            await EnsureThetaStarReady();
        
            // Process up to maxRequestsPerFrame requests
            int requestsThisFrame = Mathf.Min(maxRequestsPerFrame, pendingRequests.Count);
            
            for (int i = 0; i < requestsThisFrame; i++)
            {
                if (pendingRequests.Count == 0)
                    break;
                    
                PathRequest request = pendingRequests.Dequeue();
                
                if (request.isCancelled)
                    continue;
                    
                processingRequests.Add(request);
            }
            
            // Process all requests on background thread
            if (processingRequests.Count > 0)
            {
                await Awaitable.BackgroundThreadAsync();
                
                List<List<Vector2Int>> results = new List<List<Vector2Int>>();
                foreach (PathRequest request in processingRequests)
                {
                    List<Vector2Int> path = cachedThetaStar.CreatePath(request.startCell, request.endCell);
                    results.Add(path);
                }
                
                await Awaitable.MainThreadAsync();

                // Invoke callbacks on main thread
                for (int i = 0; i < processingRequests.Count; i++)
                {
                    if (!processingRequests[i].isCancelled)
                    {
                        processingRequests[i].callback?.Invoke(results[i]);
                    }
                }
                
                processingRequests.Clear();
            }
        }
        finally
        {
            isProcessing = false;
        }
    }
    
    /// <summary>
    /// Get statistics for monitoring performance
    /// </summary>
    public (int pending, int processing) GetQueueStats()
    {
        return (pendingRequests.Count, processingRequests.Count);
    }
}
