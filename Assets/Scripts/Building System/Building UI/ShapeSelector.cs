using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSelector : MonoBehaviour
{
    public List<ShapeUIScriptableObject> objectList;

    [SerializeField]
    private BuildingSystemManager buildingSystem;

    [SerializeField]
    private GameObject ObjectUIEntryPrefab;

    [SerializeField]
    private Transform objectShapeContentViewTransform;

    void Start()
    {
        InitializeSelectionUI();
    }

    // Start is called before the first frame update
    public void SelectObject(GameObject newObject)
    {
        // Selects object
        buildingSystem.ChangeCurrentObject(newObject);
    }

    public void InitializeSelectionUI()
    {
        foreach (ShapeUIScriptableObject entryData in objectList)
        {
            GameObject entry = Instantiate(ObjectUIEntryPrefab, objectShapeContentViewTransform);
            entry.GetComponent<ShapeUIObjectEntry>().Initialize(entryData, this);
        }
    }

    public void ClearSelectionUI()
    {
        foreach (Transform child in objectShapeContentViewTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
