using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapManager : MonoBehaviour
{
    public Tilemap tileMap { get; private set; }

    public Dictionary<Vector3Int, Cell> cellsInTileDict { get; private set; }

    public List<TileMapManager> adjacentTiles;

    private void Awake()
    {
        cellsInTileDict = new Dictionary<Vector3Int, Cell>();
        this.tileMap = this.gameObject.GetComponent<Tilemap>();
    }

    public void CreateCellsInTile()
    {
        for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; x++)
        {
            for (int y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; y++)
            {
                Vector3Int localLocation = new Vector3Int(x: x, y: y, z: 0);

                if (tileMap.HasTile(localLocation))
                {
                    Vector3Int globalLocation = Vector3Int.FloorToInt(tileMap.LocalToWorld(new Vector3Int(x: x, y: 0, z: y)));
                    Cell cell = new Cell(globalLocation, tileMap.GetComponent<TileMapManager>());
                    cellsInTileDict.Add(globalLocation, cell);
                }
            }
        }
    }

    public void AddCellsFromTileMapToBoard()
    {
        foreach (var item in cellsInTileDict)
        {
            //Добавляем на глобальную доску
            BoardManager.Instance.CellsInBoard.Add(item.Key, item.Value);
        }
    }

    public Cell GetRandomNonOcupiedCellInTile()
    {
        var nonOcupiedCells = cellsInTileDict.Values.Where(cell => cell.isOcupied == false).ToList();
        return nonOcupiedCells[Random.Range(0, nonOcupiedCells.Count)];
    }

    public List<Cell> GetRandomNotOcupiedCellsInTile(int size)
    {
        switch (size)
        {
            case 2:
                {
                    var nonOccupiedCells = cellsInTileDict.Values
                        .Where(cell => !cell.isOcupied && cell.ThereIsAtLeastOneEmptyWall())
                        .ToList();

                    var pairOfCells = new List<List<Cell>>();

                    foreach (var cell in nonOccupiedCells)
                    {
                        var pairs = cell.PairOfCellsWithoutWall(); 
                        if (pairs != null)
                        {
                            pairOfCells.AddRange(pairs);
                        }
                    }

                    if (pairOfCells.Count == 0)
                        return null;
                    return pairOfCells[UnityEngine.Random.Range(0, pairOfCells.Count)];
                }

            case 4:
                throw new NotImplementedException();
            case 6:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }
    

    //todo при уничтожении пересчитать соседей OnDestroy
}

