using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


    //public Dictionary<Vector3Int, Cell> cellsInRange { get; private set; }
    public Dictionary<Vector3Int, Cell> cellsToChooseFrom { get; private set; }
    private List<PathNode> openList;
    private List<PathNode> closeList;
    private Dictionary<Vector3Int, PathNode> pathNodesInRange;
    Cell starterPoint;
    float range;

    public PathFinder(Cell _starterPoint, int _range)
    {
        starterPoint = _starterPoint;
        range = _range + 1; //starter cell

        var cellsInRange = UtilClass.GetCellsInRange(starterPoint, range).
            ToDictionary(i => i.Key, i => i.Value);

        cellsToChooseFrom = cellsInRange.
        Where(item => item.Value.isOcupied == false).
        ToDictionary(i => i.Key, i => i.Value);
    }

    public PathFinder(bool withoutOcupiedCells)
    {
        if (withoutOcupiedCells)
        {
            cellsToChooseFrom = BoardManager.Instance.CellsInBoard.Where(item => item.Value.isOcupied == false).
            ToDictionary(i => i.Key, i => i.Value);
        } else
        {
            cellsToChooseFrom = BoardManager.Instance.CellsInBoard;
        }

    }

    public bool IsCellInPath(Cell cell, List<PathNode> pathNode)
    {
        var match = pathNode.FirstOrDefault(item => item.Cell == cell);
        if (match == null)
        {
            return false;
        }

        return true;
    }

    public List<PathNode> FindPathMaxLengthUpdateCells(Cell starterPoint, Cell endPoint, int maxPathLength)
    {
        UpdateNotOcupiedCellsInRange(starterPoint);
        var path = FindPath(starterPoint, endPoint);
        if (path != null && path.Count <= maxPathLength + 1)
        {
            path.RemoveAt(0); //обрезаем сзади, потому что клетка на которой стоим
            return path;
        }
        return null;
    }

    public List<PathNode> FindPathToOcupiedCellMaxLength(Cell starterPoint, Cell endPoint, int maxPathLength)
    {
        var cell = endPoint.ShallowCopy();
        cell.isOcupied = false;
        cellsToChooseFrom[endPoint.coords] = cell;

        var result = FindPath(starterPoint, cell);

        if (result != null && result.Count <= maxPathLength + 1) // +1 cause Starter cell counts too
        {
            result.RemoveAt(result.Count - 1); //обрезаем спереди, потому что нельз€ наступить на геро€
            result.RemoveAt(0); //обрезаем сзади, потому что клетка на которой стоим
            Debug.Log("range pf " + result.Count);
            Debug.Log("endPoint " + endPoint);
            return result;
        }
        return null;
    }

    public List<PathNode> FindPathToOcupiedCell(Cell starterPoint, Cell endPoint)
    {
        var cell = endPoint.ShallowCopy();
        cell.isOcupied = false;
        cellsToChooseFrom[endPoint.coords] = cell;

        var result = FindPath(starterPoint, cell);
        result.RemoveAt(result.Count - 1);
        return result;
    }

    public List<PathNode> FindPath(Cell starterPoint, Cell endPoint)
    {
        //todo подсветить cells in range дл€ игрока
        pathNodesInRange = new Dictionary<Vector3Int, PathNode>();
        foreach (var item in cellsToChooseFrom)
        {
            PathNode pathNode = new PathNode(item.Value);
            pathNode.gCost = int.MaxValue;
            pathNode.CalculateFCost();
            pathNode.cameFromNode = null;
            pathNodesInRange.Add(item.Key, pathNode);
        }

        //todo этажность
        PathNode startNode = new PathNode(starterPoint);
        PathNode endNode = new PathNode(endPoint);
        openList = new List<PathNode>() { startNode };
        closeList = new List<PathNode>();


        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);

        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode.Cell == endNode.Cell)
            {
                return CalculatePath(currentNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closeList.Contains(neighbourNode)) continue;

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        //Out of nodes on the openlist
        return null;
    }

    public void UpdateNotOcupiedCellsInRange(Cell starterPoint)
    {
        cellsToChooseFrom = UtilClass.GetCellsInRange(starterPoint, range).
        Where(item => item.Value.isOcupied == false).
        ToDictionary(i => i.Key, i => i.Value);
    }

    // ак достать соседа
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbours = new List<PathNode>();

        foreach(var item2 in currentNode.Cell.neighbours)
        {
            if (pathNodesInRange.ContainsKey(item2.coords))
            {
                PathNode searchedPathNode;
                if (pathNodesInRange.TryGetValue(item2.coords, out searchedPathNode))
                {
                    if (checkWalls(currentNode, searchedPathNode))
                    {
                        neighbours.Add(searchedPathNode);
                    }
                }
            }
        }
        return neighbours;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();

        return path;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(Mathf.Abs(a.x) - Mathf.Abs(b.x));
        int yDistance = Mathf.Abs(Mathf.Abs(a.z) - Mathf.Abs(b.z));
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    //todo delete
    public void DebugThis()
    {
        Debug.Log("Cells amount at all: " + BoardManager.Instance.CellsInBoard.Count());
        Debug.Log(" оличество в рейндже: " + cellsToChooseFrom.Count());
        foreach (var cell in cellsToChooseFrom)
        {
            Debug.Log("Cells в рейндже: " + cell.Key);
        }
    }

    //‘ункци€ провер€ет нет ли стены между текующей клеткой ноды и соседом
    private bool checkWalls(PathNode currentNode, PathNode neighbourNode)
    {
        int curX = currentNode.Cell.coords.x;
        int curZ = currentNode.Cell.coords.z;
        int neighX = neighbourNode.Cell.coords.x;
        int neighZ = neighbourNode.Cell.coords.z;

        int diffX = neighX - curX;
        int diffZ = neighZ - curZ;

        //corners
        if (diffX == -1 && diffZ == 1)
        {
            // _|
            if (neighbourNode.Cell.wallDown && neighbourNode.Cell.wallRight)
            {
                return false;
            }
            // |~
            if (currentNode.Cell.wallUp && currentNode.Cell.wallLeft)
            {
                return false;
            }
            // | 
            // |
            if (currentNode.Cell.wallLeft && neighbourNode.Cell.wallRight)
            {
                return false;
            }
            //--
            if (neighbourNode.Cell.wallDown && currentNode.Cell.wallUp)
            {
                return false;
            }

        }
        else if (diffX == -1 && diffZ == -1)
        {
            // ~|
            if (neighbourNode.Cell.wallUp && neighbourNode.Cell.wallRight)
            {
                return false;
            }
            // |_
            if (currentNode.Cell.wallDown && currentNode.Cell.wallLeft)
            {
                return false;
            }
            // | 
            // |
            if (currentNode.Cell.wallLeft && neighbourNode.Cell.wallRight)
            {
                return false;
            }
            //--
            if (currentNode.Cell.wallDown && neighbourNode.Cell.wallUp)
            {
                return false;
            }
        }
        else if (diffX == 1 && diffZ == 1)
        {
            // |_
            if (neighbourNode.Cell.wallLeft && neighbourNode.Cell.wallDown)
            {
                return false;
            }
            // ~|
            if (currentNode.Cell.wallUp && currentNode.Cell.wallRight)
            {
                return false;
            }
            // | 
            // |
            if (currentNode.Cell.wallRight && neighbourNode.Cell.wallLeft)
            {
                return false;
            }
            // --
            if (currentNode.Cell.wallUp && neighbourNode.Cell.wallDown)
            {
                return false;
            }
        }
        else if (diffX == 1 && diffZ == -1)
        {
            // |~
            if (neighbourNode.Cell.wallUp && neighbourNode.Cell.wallLeft)
            {
                return false;
            }
            // _| 
            if (currentNode.Cell.wallDown && currentNode.Cell.wallRight)
            {
                return false;
            }
            // | 
            // |
            if (currentNode.Cell.wallRight && neighbourNode.Cell.wallLeft)
            {
                return false;
            }
            // --
            if (currentNode.Cell.wallDown && neighbourNode.Cell.wallUp)
            {
                return false;
            }
        }

        //left, up, right and down cases
        if (diffX == 0)
        {
            if (diffZ == -1 && currentNode.Cell.wallDown)
            {
                return false;
            }
            if (diffZ == 1 && currentNode.Cell.wallUp)
            {
                return false;
            }
        }
        if (diffZ == 0)
        {
            if (diffX == -1 && currentNode.Cell.wallLeft)
            {
                return false;
            }
            if (diffX == 1 && currentNode.Cell.wallRight)
            {
                return false;
            }
        }

        return true;
    }



}
