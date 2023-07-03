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

    // Preview Material properties
    [System.Serializable]
    public struct MaterialProperties
    {
        public float alpha;
        public Color color;
        public Texture texture;
    }

    public MaterialProperties previewMaterialProperties;
    [SerializeField]
    private Material basicPreviewMaterial;

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

            GameObject previousObject = currentObjectToPlace;

            // Change the instantiated object
            SetNewObjectToPlace(newObject);

            // Change material to preview
            MeshRenderer meshRenderer = currentObjectToPlace.GetComponent<MeshRenderer>();
            SetMaterialProperties(meshRenderer);

            // Check for building mode, set freeform if not in any
            if (currentBuildingMode == buildingMode.none)
            {
                ChangeMode(buildingMode.freeform);
            }

            // Destroys old one
            Destroy(previousObject);
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
        CheckForPlacementClick();

    }

    private void HandleFreeformModeLogic()
    {
        // Updates every frame to handle freeform placement

        // Failsafe for no object
        if (currentObjectToPlace == null) return;

        // Follows the mouse based off physics raycast
        MoveCurrentObjectToMouseRaycast();

        // Checks for placing the shape
        CheckForPlacementClick();
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

    public void CheckForPlacementClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Places the object
            PlaceObject(Instantiate(currentObjectToPlace, objectParentTransform));
            
            // Allows to place more similiar objects
            // GameObject newObject = Instantiate(currentObjectToPlace);
            ChangeCurrentObject(currentObjectToPlace);
        }
    }

    public void PlaceObject(GameObject currentObject)
    {
        MeshRenderer meshRenderer = currentObject.GetComponent<MeshRenderer>();
        ObjectDetailsScript objectDetails = currentObject.GetComponent<ObjectDetailsScript>();
        // Sets no transparency
        SetMaterialProperties(meshRenderer, 1f);
        objectDetails.Initialize();
        
    }

    public void ChangeTexture(Texture newTexture)
    {
        previewMaterialProperties.texture = newTexture;
        SetMaterialProperties();
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

    public void SetNewObjectToPlace(GameObject newObject)
    {
        // Place and assign Object
        currentObjectToPlace = Instantiate(newObject, objectParentTransform);
        currentObjectToPlace.transform.position = GetMouseWorldPositionOnPlane();
    }

    public void SetMaterialProperties()
    {
        if (currentObjectToPlace == null) return;
        MeshRenderer meshRenderer = currentObjectToPlace.GetComponent<MeshRenderer>();
        SetMaterialProperties(meshRenderer);
    }

    public void SetMaterialProperties(MeshRenderer meshRenderer)
    {
        SetMaterialProperties(meshRenderer, previewMaterialProperties.alpha);
    }

    public void SetMaterialProperties(MeshRenderer meshRenderer, float alpha)
    {
        // Change material to preview
        meshRenderer.material.CopyPropertiesFromMaterial(basicPreviewMaterial);

        // Set detailed material properties
        Color newColor = previewMaterialProperties.color;
        newColor.a = previewMaterialProperties.alpha;
        meshRenderer.material.SetColor("_BaseColor", previewMaterialProperties.color);
        meshRenderer.material.SetTexture("_BaseMap", previewMaterialProperties.texture);
    }

}
