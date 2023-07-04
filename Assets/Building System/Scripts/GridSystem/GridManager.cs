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
    private int gridDepth = 5;

    [SerializeField]
    private float gridCellSize = 1f;

    [SerializeField]
    private bool toVisualize = true;

    [SerializeField]
    private Vector3 originPosition = Vector3.zero;

    private Grid<GridCell> baseGrid;

    // Start is called before the first frame update
    void Start()
    {
        baseGrid = new Grid<GridCell>(gridWidth, gridHeight, gridDepth, gridCellSize, originPosition, toVisualize);
        FillWithGridCells(baseGrid);
    }

    public void FillWithGridCells(Grid<GridCell> grid)
    {
        // Fills the grid with data-type, in this case GridCells.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridDepth; z++)
                {
                    grid.gridArray[x, y, z] = new GridCell(grid, x, y, z, grid.GetWorldPosition(x, y, z));
                } 
            }
        }
    }

    public GridCell GetGridCell(Vector3 mousePosition)
    {
        return baseGrid.GetCell(mousePosition);
    }

    /*
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
    */
}
