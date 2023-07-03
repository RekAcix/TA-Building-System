using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystemManager : MonoBehaviour
{
    // Object that will be placed
    public GameObject currentObjectToPlace;

    [SerializeField]
    private Transform objectParentTransform;

    [SerializeField]
    private GridManager gridManager;

    [SerializeField]
    private float scale = 1f;

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

            // Destroy placeholder object
            Destroy(currentObjectToPlace);

        } else
        {
            // Changes the object
            Destroy(currentObjectToPlace);

            // Change the instantiated object
            currentObjectToPlace = Instantiate(newObject, objectParentTransform);
            currentObjectToPlace.transform.position = GetMouseWorldPositionOnPlane();
        }
    }

    public void ChangeMode(buildingMode newMode)
    {
        // Changes the building mode to a newMode
        currentBuildingMode = newMode;

        if (currentBuildingMode == buildingMode.none)
        {
            Destroy(currentObjectToPlace);
        }
    }

    void Update()
    {
        // Control Modes with keyboard keys.
        SwitchModes();


        // Different State Logic
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
        GridCell selectedGridCell = gridManager.GetGridCell(GetMouseWorldPositionOnPlane());

        // Failsafe for no object
        if (currentObjectToPlace == null) return;

        // Moving the object to be placed
        if (selectedGridCell == null)
        {
            MoveCurrentObjectToMouseOnPlane();
            return;
        }
        MoveCurrentObjectToGridCell(selectedGridCell);


    }

    private void HandleFreeformModeLogic()
    {
        // Updates every frame to handle freeform placement

        // Failsafe for no object
        if (currentObjectToPlace == null) return;

        // Follows the mouse based off physics raycast
        MoveCurrentObjectToMouseRaycast();
    }

    private void HandleNoModeLogic()
    {
        // Handles the case when no building mode is selected, i.e. Edit Mode
    }

    public void SwitchModes()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeMode(buildingMode.grid);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeMode(buildingMode.freeform);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeMode(buildingMode.none);
        }
    }

    public void MoveCurrentObjectToMouseOnPlane()
    {
        currentObjectToPlace.transform.position = GetMouseWorldPositionOnPlane() + new Vector3(0, scale * 0.5f, 0);
    }

    public void MoveCurrentObjectToMouseRaycast()
    {
        currentObjectToPlace.transform.position = GetMouseWorldPositionPhysicsRaycast() + new Vector3(0, scale * 0.5f, 0);
    }

    public void MoveCurrentObjectToGridCell(GridCell selectedGridCell)
    {
        float cellSize = selectedGridCell.GetGrid().GetCellSize();
        // Accomodates offset to put the shape in the middle and above the grid
        currentObjectToPlace.transform.position = selectedGridCell.GetWorldPosition() + new Vector3(cellSize, cellSize, cellSize) * 0.5f;
    }

    public Vector3 GetMouseWorldPositionOnPlane()
    {
        Vector3 worldPosition = Vector3.zero;

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            worldPosition = ray.GetPoint(enter);
        }

        return worldPosition;
    }

    public Vector3 GetMouseWorldPositionPhysicsRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 raycastHitPosition = Vector3.zero;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            raycastHitPosition = hit.point;
        } else
        {
            raycastHitPosition = GetMouseWorldPositionOnPlane();
        }
        return raycastHitPosition;
    }

}
