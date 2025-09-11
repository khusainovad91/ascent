using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectFinder 
{
    private Cell _starterCell;
    private Dictionary<Vector3Int, Cell> _cellsInRange;
    private int _range;
    public ObjectFinder(Cell starterCell, int range)
    {
        this._starterCell = starterCell;
        _range = range;
        _cellsInRange = UtilClass.GetCellsInRange(starterCell, range);
    }

    public ObjectFinder(Cell starterCell)
    {
        this._starterCell = starterCell;
    }

    //Метод ищет среди cellsInRange до которых можно дотянуться с дальностью Range 
    //линия прямая не включает стены, обрывы и LoS
    public Dictionary<T, int> SearchObjectsInRage<T>() where T : FieldObject
    {
        Dictionary<T, int> result = new Dictionary<T, int>();
        Dictionary<Vector3Int, Cell> searchedObjects = _cellsInRange.Where(item => item.Value.objectOnTile?.GetComponent<T>()).ToDictionary(i => i.Key, i => i.Value);

        foreach (Cell cell in searchedObjects.Values)
        {
            Debug.Log("cell: " + cell);
            result.Add(cell.objectOnTile.GetComponent<T>(), (int)(Vector3.Distance(cell.coords, _starterCell.coords)));
            Debug.Log("Distance: " + (int)(Vector3.Distance(cell.coords, _starterCell.coords)));
        }

        return result;
    }

    public List<Cell> FindEmptyCellsInRange()
    {
        return _cellsInRange.Values.Where(item => item.objectOnTile == null).ToList();
    }

    public Tuple<List<PathNode>, FieldObject> PathToNearestObject<T>() where T : FieldObject
    {
        Dictionary<Vector3Int, Cell> searchedObjects = BoardManager.Instance.CellsInBoard.Where(item => item.Value.objectOnTile?.GetComponent<T>()).ToDictionary(i => i.Key, i => i.Value);
        PathFinder pathFinder = new PathFinder(false);
        List<PathNode> result = null;

        int min = int.MaxValue;
        FieldObject fieldObject = null;

        foreach (var item in searchedObjects)
        {
            Debug.Log("item.value " + item.Value);
        }

        foreach (var item in searchedObjects)
        {
            var path = pathFinder.FindPathToOcupiedCell(_starterCell, item.Value);
            if (path.Count < min)
            {
                min = path.Count;
                result = path;
                fieldObject = item.Value.objectOnTile;
            }
        }

        return new Tuple<List<PathNode>, FieldObject>(result, fieldObject);
    }

    // создавать только с CellsInRange
    public List<PathNode> PathToRandomEmptyCellOnMaximumRangeFromCurrent()
    {
        List<Cell> searchedObjects = _cellsInRange.Where(item => item.Value.objectOnTile == null).ToDictionary(i => i.Key, i => i.Value).Values.ToList(); //пустые в округе
        searchedObjects.Sort((a, b) =>
        {
            int distA = (a.coords - _starterCell.coords).sqrMagnitude;
            int distB = (b.coords - _starterCell.coords).sqrMagnitude;

            return distB.CompareTo(distA);
        });

        PathFinder pathFinder = new PathFinder(_starterCell, _range);
        foreach (var cell in searchedObjects)
        {
            List<PathNode> result = pathFinder.FindPathMaxLengthUpdateCells(_starterCell, cell, _range);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
