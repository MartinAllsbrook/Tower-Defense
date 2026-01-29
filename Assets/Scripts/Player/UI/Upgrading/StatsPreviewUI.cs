using UnityEngine;

public class StatsPreviewUI : MonoBehaviour
{
    [SerializeField] StatDisplayUI[] statDisplayUIs;

    void Awake()
    {
        if (statDisplayUIs.Length != 6)
            Debug.LogError("StatsPreviewUI requires exactly 6 StatDisplayUIs assigned.");
    }

    public void Set(TurretStat[] stats)
    {
        for (int i = 0; i < statDisplayUIs.Length; i++)
        {
            if (i >= stats.Length)
            {
                statDisplayUIs[i].gameObject.SetActive(false);
                continue;   
            }

            statDisplayUIs[i].gameObject.SetActive(true);
            var stat = stats[i];
            statDisplayUIs[i].Set("test", stat.Value, 0, 100);
        }
    }

    public void PreviewUpgrade(TurretUpgrade upgrade)
    {
        
    }

    public void ClearPreview()
    {
        
    }
}
