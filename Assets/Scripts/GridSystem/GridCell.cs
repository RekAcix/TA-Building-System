using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private Grid<GridCell> grid;
    private int x;
    private int y;
    private Vector3 worldPosition;

    private GameObject worldObject;

    public GridCell(Grid<GridCell> grid, int x, int y, Vector3 worldPosition)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.worldPosition = worldPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return worldPosition;
    }

    public Grid<GridCell> GetGrid()
    {
        return grid;
    }
}
