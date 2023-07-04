using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BuildingSystemManager : MonoBehaviour
{
    // Object that will be placed
    public GameObject currentObjectToPlace;

    [SerializeField]
    private Transform objectParentTransform;

    [SerializeField]
    private Transform multipleObjectParentTransform;

    [SerializeField]
    private GridManager gridManager;

    [SerializeField] // Asset initial scale
    private float scale = 1f;

    private float[] currentScale = new float[3];

    // Preview Material properties
    [System.Serializable]
    public struct MaterialProperties
    {
        public float alpha;
        public Color color;
        public Texture texture;
    }

    // Materials
    public MaterialProperties previewMaterialProperties;
    [SerializeField]
    private Material basicPreviewMaterial;

    // Object lists
    [SerializeField]
    private Color highlightColor;

    private List<GameObject> placedObjects = new List<GameObject>();
    [SerializeField]
    private List<GameObject> highlightEffectObjects = new List<GameObject>();
    [SerializeField]
    private List<GameObject> selectedObjects = new List<GameObject>();

    // Changes menu
    [SerializeField]
    private GameObject changeMenu;

    private float combinedRotationSpeed = 60f;
    private Vector3 rotationAxisPoint;

    // Basic building state machine
    public enum buildingMode
    {
        none, // 0
        freeform, // 1
        grid, // 2
    }

    private buildingMode currentBuildingMode;

    // Interface
    [SerializeField]
    private TextMeshProUGUI modeText;

    void Awake()
    {
        currentScale[0] = 1f;
        currentScale[1] = 1f;
        currentScale[2] = 1f;
    }

    void Start()
    {
        currentBuildingMode = buildingMode.none;
    }

    public void ChangeCurrentObject(GameObject newObject)
    {
        // No effect if only changes shape of selected objects
        if (selectedObjects.Count > 0) return;

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

    public void UpdateSelectedShapes(GameObject newObject)
    {
        if (selectedObjects.Count == 0) return;

        List<GameObject> objToRemove = new List<GameObject>();
        List<GameObject> objToAdd = new List<GameObject>();

        foreach (GameObject obj in selectedObjects)
        {
            GameObject replacement = Instantiate(newObject, obj.transform.position, obj.transform.rotation, objectParentTransform);
            replacement.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(obj.GetComponent<MeshRenderer>().material);
            replacement.GetComponent<ObjectDetailsScript>().CopyDetails(obj.GetComponent<ObjectDetailsScript>());
            objToAdd.Add(replacement);
            objToRemove.Add(obj);
        }

        foreach (GameObject obj in objToAdd)
        {
            selectedObjects.Add(obj);
        }

        foreach (GameObject obj in objToRemove)
        {
            selectedObjects.Remove(obj);
            Destroy(obj);
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

        UpdateTextDisplay();
    }

    void Update()
    {
        // Control Modes with keyboard keys.
        SwitchModes();

        // Rotate and Control the Shape
        ObjectRotationControl();


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

        // Need to accomodate for the fact that things can overlap on a grid
        Vector3 mousePosition = GetMouseWorldPositionPhysicsRaycast();
        Vector3 direction = Vector3.Normalize(mousePosition - Camera.main.transform.position);
        GridCell selectedGridCell = gridManager.GetGridCell(mousePosition - direction * 0.25f * scale);

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
        // Can be changed depending on the anchor convention
        MoveCurrentObjectToMouseRaycast();

        // Checks for placing the shape
        CheckForPlacementClick();
    }

    private void HandleNoModeLogic()
    {
        // Handles the case when no building mode is selected, i.e. Edit Mode

        // Cast raycast to select objects
        GameObject highlightedObject = RaycastPlacedObjectsDetection();

        // Get rid of highlight if new object
        if (highlightEffectObjects.Count > 0)
        {
            foreach (GameObject highObject in highlightEffectObjects)
            {
                if (highObject == null)
                {
                    highlightEffectObjects.Remove(highObject);
                    return;
                }
                DehighlightSelectedObject(highObject);
            }
            highlightEffectObjects.Clear();
        }

        MoveSelectedObjects();

        // Fail safe for no object detection
        if (highlightedObject == null)
        {
            // Delete all selected on escape
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                RemoveAll();
            }
            
            if (highlightEffectObjects.Count > 0)
            {
                foreach (GameObject highObject in highlightEffectObjects)
                {
                    if (highObject == null)
                    {
                        highlightEffectObjects.Remove(highObject);
                        return;
                    }
                    DehighlightSelectedObject(highObject);
                }
                highlightEffectObjects.Clear();
            }

            // Right click to deselect all
            if (Input.GetMouseButtonDown(1))
            {
                DeselectAll();
            }

            return;
        }

        // Highlight selected Object
        if (!selectedObjects.Contains(highlightedObject) && !highlightEffectObjects.Contains(highlightedObject))
        {
            HighlightSelectedObject(highlightedObject);
            highlightEffectObjects.Add(highlightedObject);
        }

        // On left click add to the selectedObjects list
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(highlightEffectObjects.Count);
            highlightEffectObjects.Remove(highlightedObject);
            if (Input.GetKey(KeyCode.LeftControl))
            {
                selectedObjects.Add(highlightedObject);
            } else
            {
                DeselectAll();
                selectedObjects = new List<GameObject>();
                selectedObjects.Add(highlightedObject);
            }

        }



        // Click delete to remove all selected objects and higlighted ones
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(highlightedObject);
            RemoveAll();
        }

    }

    private void MoveSelectedObjects()
    {
        // First calculate offset from the cursor on pressing the button
        if (Input.GetMouseButtonDown(1))
        {
            foreach (GameObject objects in selectedObjects)
            {
                objects.GetComponent<ObjectDetailsScript>().CalculateOffset(GetMouseWorldPositionOnPlane());
                objects.layer = LayerMask.NameToLayer("Ignore Raycast");
                // Need to update this position with progress
                //objects.transform.parent = multipleObjectParentTransform;
            }
        }

        // As the button is being dragged, update the position along with the offset
        if (Input.GetMouseButton(1))
        {
            if (Input.GetKeyDown(KeyCode.RightBracket) || Input.GetKey(KeyCode.LeftBracket))
            {
                // rotationAxisPoint = GetMouseWorldPositionOnPlane() + new Vector3(0, scale * 0.5f, 0);
                rotationAxisPoint = CalculateCenterPoint(selectedObjects);
            }

            // Rotate it along the axis
            if (Input.GetKey(KeyCode.RightBracket) || Input.GetKey(KeyCode.LeftBracket))
            {
                //multipleObjectParentTransform.transform.Rotate(0f, combinedRotationSpeed * Time.deltaTime, 0f, Space.World);
                foreach (GameObject objects in selectedObjects)
                {
                    float reverseMultiplier = 1f + (-2f * Convert.ToInt32(Input.GetKey(KeyCode.LeftBracket)));
                    //objects.GetComponent<ObjectDetailsScript>().CalculateOffset(GetMouseWorldPositionPhysicsRaycast());
                    objects.transform.RotateAround(rotationAxisPoint, Vector3.up, combinedRotationSpeed * Time.deltaTime * reverseMultiplier);
                    objects.GetComponent<ObjectDetailsScript>().CalculateCurrentOffset(GetMouseWorldPositionOnPlane());
                }
            } else
            {
                // Adjust all of the selected objects positions by the mouse cursor position
                foreach (GameObject objects in selectedObjects)
                {
                    //objects.transform.position = GetMouseWorldPositionPhysicsRaycast() + objects.GetComponent<ObjectDetailsScript>().GetOffset();
                    objects.transform.position = GetMouseWorldPositionOnPlane() + objects.GetComponent<ObjectDetailsScript>().GetOffset();
                }

            }

        }

        // On release, update the positions
        if (Input.GetMouseButtonUp(1))
        {
            foreach (GameObject objects in selectedObjects)
            {
                objects.transform.parent = objectParentTransform;
                objects.GetComponent<ObjectDetailsScript>().UpdateLockedPosition();
                objects.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public void SwitchModes()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeMode(buildingMode.grid);
            DeselectAll();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeMode(buildingMode.freeform);
            DeselectAll();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeMode(buildingMode.none);
            DeselectAll();
        }
    }

    public void ObjectRotationControl()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            if (currentObjectToPlace != null)
            {
                //Debug.Log("Flips");
                currentObjectToPlace.GetComponent<ObjectDetailsScript>().Flip(90f);
            }

            if (selectedObjects.Count > 0)
            {
                foreach (GameObject obj in selectedObjects)
                {
                    obj.GetComponent<ObjectDetailsScript>().Flip(90f);
                }
            }
        }

        if (currentObjectToPlace == null) return;

        if (Input.GetKey(KeyCode.RightBracket))
        {
            currentObjectToPlace.GetComponent<ObjectDetailsScript>().Rotate(1f);
        }

        if (Input.GetKey(KeyCode.LeftBracket))
        {
            currentObjectToPlace.GetComponent<ObjectDetailsScript>().Rotate(-1f);
        }

        

    }

    public void CheckForPlacementClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Clicked on an UI element");
                return;
            }

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

    public Vector3 CalculateCenterPoint(List<GameObject> objectList)
    {
        Vector3 totalPosition = Vector3.zero;
        foreach (GameObject obj in objectList)
        {
            totalPosition += obj.transform.position;
        }
        totalPosition = totalPosition / objectList.Count;
        return totalPosition;
    }

    public void ChangeTexture(Texture newTexture)
    {
        previewMaterialProperties.texture = newTexture;
        SetMaterialProperties();

        // Change for all selected objects
        if (selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                SetMaterialProperties(obj);
            }
        }
    }

    public void UpdateObjectScales(float[] currentScale)
    {
        this.currentScale = currentScale;

        if (currentObjectToPlace != null)
        {
            UpdateScale(currentObjectToPlace);
        }

        if (selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                UpdateScale(obj);
            }
        }
    }

    public void UpdateScale(GameObject obj)
    {
        obj.transform.localScale = new Vector3(currentScale[0], currentScale[1], currentScale[2]);
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

    public GameObject RaycastPlacedObjectsDetection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject hitObject = null;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            hitObject = hit.transform.gameObject;
            if (hitObject.GetComponent<ObjectDetailsScript>() != null)
            {
                return hitObject;
            }
        }
        return null;
    }

    public void ActivateContextMenu()
    {
        changeMenu.SetActive(true);
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

    public void RemoveAll()
    {
        foreach (GameObject selectedObject in selectedObjects)
        {
            Destroy(selectedObject);
        }
        selectedObjects.Clear();
    }

    public void DeselectAll()
    {
        foreach (GameObject selectedObject in selectedObjects)
        {
            if (selectedObject != null) DehighlightSelectedObject(selectedObject);
            selectedObject.layer = LayerMask.NameToLayer("Default");
        }

        selectedObjects.Clear();
    }

    public void HighlightSelectedObject(GameObject highlightedObject)
    {
        highlightedObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", highlightColor);
    }

    public void DehighlightSelectedObject(GameObject highlightedObject)
    {
        highlightedObject.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", highlightedObject.GetComponent<ObjectDetailsScript>().GetSavedColor());
    }

    public void UpdateTextDisplay()
    {
        switch (currentBuildingMode)
        {
            case buildingMode.none:
                modeText.text = "Current Mode: Edit";
                break;
            case buildingMode.freeform:
                modeText.text = "Current Mode: Freeform";
                break;
            case buildingMode.grid:
                modeText.text = "Current Mode: Grid";
                break;
        }
    }

    public void SetNewObjectToPlace(GameObject newObject)
    {
        // Place and assign Object
        currentObjectToPlace = Instantiate(newObject, objectParentTransform);
        UpdateScale(currentObjectToPlace);
        currentObjectToPlace.transform.position = GetMouseWorldPositionOnPlane();
    }

    public void SetMaterialProperties()
    {
        if (currentObjectToPlace == null) return;
        MeshRenderer meshRenderer = currentObjectToPlace.GetComponent<MeshRenderer>();
        SetMaterialProperties(meshRenderer);
    }

    public void SetMaterialProperties(GameObject obj)
    {
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
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
        newColor.a = alpha;
        meshRenderer.material.SetColor("_BaseColor", newColor);
        meshRenderer.material.SetTexture("_BaseMap", previewMaterialProperties.texture);
    }

}
