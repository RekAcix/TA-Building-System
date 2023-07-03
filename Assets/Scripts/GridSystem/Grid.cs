using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;

    private int[,] gridArray;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Creates the grid in the game world.

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                // Loops through every (x, y) grid element
                Vector3 cellWorldPosition = GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f;

                // Draw the Grid
                // Vertical
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                // Horizontal
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        // Close the grid
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        // Coordinate system adjusted for the convention
        return new Vector3(x, 0, y) * cellSize;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.z / cellSize);
    }

    public void SetCell(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            // Assign value or object here.
        }
    }

    public void SetCell(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);

        SetCell(x, y, value);
    }






}
