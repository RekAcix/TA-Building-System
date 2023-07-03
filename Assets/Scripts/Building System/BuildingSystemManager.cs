using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystemManager : MonoBehaviour
{
    // Object that will be placed
    public GameObject currentObjectToPlace;

    [SerializeField]
    private GridManager gridManager;

    // Basic building state machine
    public enum buildingMode
    {
        none, // 0
        freeform, // 1
        grid, // 2
    }

    private buildingMode currentBuildingMode;

    void Start()
    {
        currentBuildingMode = buildingMode.none;
    }

    public void ChangeCurrentObject(GameObject newObject)
    {
        if (newObject == null)
        {
            // No building mode
            ChangeMode(buildingMode.none);
        }
    }

    public void ChangeMode(buildingMode newMode)
    {
        // Changes the building mode to a newMode
        currentBuildingMode = newMode;
    }

    void Update()
    {
        if (currentBuildingMode == buildingMode.grid)
        {
            HandleGridModeLogic();
        } else if (currentBuildingMode == buildingMode.freeform)
        {
            HandleFreeformModeLogic();
        } else
        {
            HandleNoModeLogic();
        }
    }

    private void HandleGridModeLogic()
    {
        // Updates every frame to handle placement via Grid
        GridCell selectedGridCell = gridManager.GetGridCell(input.mousePosition);

        // Fail safe for no grid
        if (selectedGridCell == null)
        {
            return;
        }



    }

    private void HandleFreeformModeLogic()
    {
        // Updates every frame to handle freeform placement

    }

    private void HandleNoModeLogic()
    {
        // Handles the case when no building mode is selected, i.e. Edit Mode
    }

}
