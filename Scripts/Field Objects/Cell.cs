using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Cell
{
    public bool isOcupied = false; 
    //кек
    public FieldObject objectOnTile; //NetworkObject
    //todo delete
    //public TileMapManager tileMap { get; private set; }

    //4 стены
    public bool wallUp;
    //south
    public bool wallDown;
    //west 
    public bool wallLeft;
    //east
    public bool wallRight;

    public Vector3Int coords { get; private set; }

    public List<Cell> neighbours = new List<Cell>();

    public Cell(Vector3Int location, TileMapManager tileMap)
    {
        //this.tileMap = tileMap;
        this.isOcupied = false;
        this.objectOnTile = null;
        this.coords = location;
    }
    public Cell ShallowCopy()
    {
        return (Cell)this.MemberwiseClone();
    }

    //public void ClearCell()
    //{
    //    this.isOcupied = false;
    //    this.objectOnTile = null;
    //}


    //public void OcupieCell(FieldObject gameObject)
    //{
    //    this.isOcupied = true;
    //    this.objectOnTile = gameObject;
    //    gameObject.CurrentCell = this;
    //}

    public override string ToString()
    {
        return "isOcupied: " + isOcupied + "\n"
            + "objectOnTile" + objectOnTile + "\n"
            + "coords" + coords;
    }

    public bool ThereIsAtLeastOneEmptyWall() => wallUp || wallRight || wallDown || wallLeft;

    public List<List<Cell>> PairOfCellsWithoutWall()
    {
        List<List<Cell>> result = new List<List<Cell>>();
        var leftCell = neighbours.FirstOrDefault(cell => cell.coords == new Vector3Int(this.coords.x - 1, 0, this.coords.z));
        var rightCell = neighbours.FirstOrDefault(cell => cell.coords == new Vector3Int(this.coords.x + 1, 0, this.coords.z));
        var upCell = neighbours.FirstOrDefault(cell => cell.coords == new Vector3Int(this.coords.x, 0, this.coords.z + 1));
        var bottomCell = neighbours.FirstOrDefault(cell => cell.coords == new Vector3Int(this.coords.x + 1, 0, this.coords.z - 1));

        if (leftCell != null && !wallLeft)
        {
            result.Add(new List<Cell> { this, leftCell });
        }

        if (rightCell != null && !wallRight)
        {
            result.Add(new List<Cell> { this, rightCell });
        }

        if (upCell != null && !wallUp)
        {
            result.Add(new List<Cell> { this, upCell });
        }

        if (bottomCell != null && !wallDown)
        {
            result.Add(new List<Cell> { this, bottomCell });
        }

        return result;
    }
    //todo in Tilemaps delete
    //private void OnDestroy()
    //{
    //    foreach (var item in neighbours)
    //    {
    //        item.neighbours.Remove(this);
    //    }
    //}
  
}
