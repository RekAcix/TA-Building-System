using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int gridWidth = 5;

    [SerializeField]
    private int gridHeight = 5;

    [SerializeField]
    private float gridCellSize = 1f;

    private Grid baseGrid;

    // Start is called before the first frame update
    void Start()
    {
        baseGrid = new Grid(gridWidth, gridHeight, gridCellSize);
    }

    void Update()
    {
        // Check for clicks
        HandleLeftClick();
    }

    public void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPositionOnPlane();
            Debug.Log(mouseWorldPosition);
        }
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
}
