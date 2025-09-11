using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PathNode 
{
    //private Grid<PathNode> grid;
    public int x;
    public int z;
    //todo этажи
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public Cell Cell;

    public PathNode cameFromNode;

    //UnityEngine.Grid<PathNode> grid,
    public PathNode(int x, int z, int y, int gCost, int hCost, int fCost, PathNode cameFromNode)
    {
        this.x = x;
        this.z = z;
        this.y = y;
        this.gCost = gCost;
        this.hCost = hCost;
        this.fCost = fCost;
        this.cameFromNode = cameFromNode;
    }

    public PathNode(Cell item)
    {
        this.x = item.coords.x;
        this.z = item.coords.z;
        this.y = item.coords.y;
        this.Cell = item;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }


}
