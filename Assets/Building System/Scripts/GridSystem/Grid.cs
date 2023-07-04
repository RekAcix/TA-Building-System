using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridCell>
{
    private int width;
    private int height;
    private int depth;
    private float cellSize;
    private Vector3 originPosition;

    public TGridCell[,,] gridArray;
    private bool toVisualize;

    public Grid(int width, int height, int depth, float cellSize, Vector3 originPosition, bool toVisualize)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.toVisualize = toVisualize;

        gridArray = new TGridCell[width, height, depth];

        if (toVisualize) VisualizeGrid();
    }

    private void VisualizeGrid()
    {
        // Creates the grid in the game world.

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    // Loops through every (x, y) grid element
                    Vector3 cellWorldPosition = GetWorldPosition(x, y, z) + new Vector3(cellSize, 0, cellSize) * 0.5f;

                    // Draw the Grid
                    // Vertical
                    Debug.DrawLine(GetWorldPosition(x, y, z), GetWorldPosition(x, y + 1, z), Color.white, 100f);
                    // Horizontal
                    Debug.DrawLine(GetWorldPosition(x, y, z), GetWorldPosition(x + 1, y, z), Color.white, 100f);
                }  
            }
        }

        // Close the grid
        Debug.DrawLine(GetWorldPosition(0, height, 0), GetWorldPosition(width, height, 0), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0, 0), GetWorldPosition(width, height, 0), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int x, int y, int z)
    {
        // Coordinate system adjusted for the convention
        return new Vector3(x, z, y) * cellSize + originPosition;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        //Debug.Log(x.ToString() + "," + y.ToString() + "," + z.ToString());
    }

    public void SetCell(int x, int y, int z, TGridCell value)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth)
        {
            // Assign value or object here.
        }
    }

    public void SetCell(Vector3 worldPosition, TGridCell value)
    {
        int x, y, z;
        GetXY(worldPosition, out x, out y, out z);

        SetCell(x, y, z, value);
    }

    public TGridCell GetCell(int x, int y, int z)
    {
        // Based off of grid coordinates
        if (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth)
        {
            // return value or object here.
            return gridArray[x, y, z];
        } else
        {
            return default(TGridCell);
        }
    }

    public TGridCell GetCell(Vector3 worldPosition)
    {
        // Based off of World position
        int x, y, z;
        GetXY(worldPosition, out x, out y, out z);
        return GetCell(x, y, z);
    }

    public float GetCellSize()
    {
        return cellSize;
    }






}
