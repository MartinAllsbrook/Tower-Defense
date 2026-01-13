using UnityEngine;

public class StructureMenu : MonoBehaviour
{
    [SerializeField] RectTransform buttonContainer;
    [SerializeField] RectTransform menuPanel;
    [SerializeField] GameObject placeStructureButtonPrefab;

    StructurePlacer structurePlacer;

    void Awake()
    {
        structurePlacer = FindFirstObjectByType<StructurePlacer>();
    }

    void Start()
    {
        StructureTile[] structures = structurePlacer.Structures;
        foreach (StructureTile structure in structures)
        {
            GameObject buttonObj = Instantiate(placeStructureButtonPrefab, buttonContainer);
            PlaceStructureButton button = buttonObj.GetComponent<PlaceStructureButton>();
            button.Initialize(structure, structurePlacer);
        }

        menuPanel.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        menuPanel.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.gameObject.SetActive(false);
    }

    public void EnterRemoveMode()
    {
        structurePlacer.EnterRemoveMode();
        CloseMenu();
    }

}
