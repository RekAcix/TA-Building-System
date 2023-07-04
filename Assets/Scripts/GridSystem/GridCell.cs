using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private Grid<GridCell> grid;
    private int x;
    private int y;
    private int z;
    private Vector3 worldPosition;

    private GameObject worldObject;

    public GridCell(Grid<GridCell> grid, int x, int y, int z, Vector3 worldPosition)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.z = z;
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
