using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{   
    // Static
    public static Player Instance { get; private set; }
    public static event Action<int> OnMoneyChanged;

    // Serialized fields
    [Header("Economy")]
    [SerializeField] int startingMoney = 100;
    [SerializeField] float passiveIncomeInterval = 1f;
    [SerializeField] int passiveIncomeAmount = 1;

    int money;
    Coroutine passiveIncomeCoroutine;


    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        money = startingMoney;
    }

    void Start()
    {
        OnMoneyChanged?.Invoke(money);
    }

    void OnEnable()
    {
        GameController.OnRoundStart += StartPassiveIncome;
        GameController.OnRoundEnd += StopPassiveIncome;
    }

    void OnDisable()
    {
        GameController.OnRoundStart -= StartPassiveIncome;
        GameController.OnRoundEnd -= StopPassiveIncome;
    }

    void StartPassiveIncome(int roundNumber)
    {
        Debug.Log("Starting passive income");
        passiveIncomeCoroutine = StartCoroutine(PassiveIncomeCoroutine());
    }

    void StopPassiveIncome(int roundNumber)
    {
        Debug.Log("Stopping passive income");
        if (passiveIncomeCoroutine != null)
        {
            StopCoroutine(passiveIncomeCoroutine);
            passiveIncomeCoroutine = null;
        }
    }

    IEnumerator PassiveIncomeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveIncomeInterval);
            AddMoney(passiveIncomeAmount);
        }
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            OnMoneyChanged?.Invoke(money);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }

    public int GetMoney()
    {
        return money;
    }

    Target target;

    public void SetTarget(Target target) {
        this.target = target;
    }

    public Target GetTarget() {
        return target;
    }
}







// Old logic for highlighting hex tiles under the cursor
/*
[SerializeField] Tilemap hexTilemap;
[SerializeField] GameObject hexagonHighlightPrefab;
GameObject hexHighlightInstance;
Vector3Int lastHighlightedCell = new Vector3Int(int.MinValue, int.MinValue, 0);

void Update()
{
    // Get mouse position in world space
    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    mouseWorldPos.z = 0f;

    // Highlight the hexagonal tile under the cursor
    if (hexTilemap != null && hexagonHighlightPrefab != null)
    {
        // Convert world position to cell coordinates
        Vector3Int cellPosition = hexTilemap.WorldToCell(mouseWorldPos);
        
        // Only update if we're over a different cell
        if (cellPosition != lastHighlightedCell)
        {
            lastHighlightedCell = cellPosition;
            
            // Get the center of the cell in world space
            Vector3 cellCenterWorld = hexTilemap.GetCellCenterWorld(cellPosition);
            
            // Create highlight GameObject if it doesn't exist
            if (hexHighlightInstance == null)
            {
                hexHighlightInstance = Instantiate(hexagonHighlightPrefab);                    
            }
            
            // Position the highlight at the center of the hexagonal tile
            hexHighlightInstance.transform.position = cellCenterWorld;
        }
    }
}
*/