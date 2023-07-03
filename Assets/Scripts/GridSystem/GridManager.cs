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

    [SerializeField]
    private Vector3 originPosition = Vector3.zero;

    private Grid<GridCell> baseGrid;

    // Creates the basic Grid Cell datatype
    public class GridCell
    {
        private Grid<GridCell> grid;
        private int x;
        private int y;

        private GameObject worldObject;

        public GridCell(Grid<GridCell> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        baseGrid = new Grid<GridCell>(gridWidth, gridHeight, gridCellSize, originPosition);
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
            // baseGrid.GetCell(mouseWorldPosition)
        }
    }

    public GridCell GetGridCell(Vector3 mousePosition)
    {
        return baseGrid.GetCell(GetMouseWorldPositionOnPlane());
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
