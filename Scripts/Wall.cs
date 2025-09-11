using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Wall : NetworkBehaviour
{
    List<Vector3Int> wallCoords;
    //Если не x, то z;
    //перпендикулярно X, паралельно Z
    public bool XWall;

    //todo remaker on NetworkSpawn

    private void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Obstacles");
        //Получить координаты стены в Vector3Int
        wallCoords = UtilClass.GetRenderCoordinates(this.gameObject);
        //Определеить стена паралельна оси x или по оси z
        determinateWallRotation();
        //Проставить стены в ячейки
        EventManager.Instance.Subscribe("GameStarting", AddWallToBoard);
    }

    protected void AddWallToBoard()
    {
        ActivateWall(true);
    }

    protected void ActivateWall(bool isActive)
    {
        var boardCells = BoardManager.Instance.CellsInBoard;
        if (XWall)
        {
            for (int i = 0; i < wallCoords.Count - 1; ++i)
            {
                Vector3Int above = new Vector3Int(wallCoords[i].x, wallCoords[i].y, wallCoords[i].z);
                Vector3Int below = new Vector3Int(wallCoords[i].x, wallCoords[i].y, wallCoords[i].z - 1);
                //клетка над стеной
                if (boardCells.ContainsKey(above))
                {
                    boardCells[above].wallDown = isActive;
                }
                //клетка под стеной
                if (boardCells.ContainsKey(below))
                {
                    boardCells[below].wallUp = isActive;
                }
            }
        }
        else if (wallCoords.Count > 1)
        {
            for (int i = 0; i < wallCoords.Count - 1; ++i)
            {
                Vector3Int onLeft = new Vector3Int(wallCoords[i].x - 1, wallCoords[i].y, wallCoords[i].z);
                Vector3Int onRight = new Vector3Int(wallCoords[i].x, wallCoords[i].y, wallCoords[i].z);
                //клетка слева
                if (boardCells.ContainsKey(onLeft))
                {
                    boardCells[onLeft].wallRight = isActive;
                }
                //клетка справа
                if (boardCells.ContainsKey(onRight))
                {
                    boardCells[onRight].wallLeft = isActive;
                }
            }
        }
        else
        {
            Debug.Log("Проверь стену: " + wallCoords);
        }
    }

    private void determinateWallRotation()
    {
        if (wallCoords.Count > 1)
        {
            if (wallCoords[0].z == wallCoords[1].z)
            {
                XWall = true;
            }
        }
        else
        {
            Debug.Log("У стены меньше одной точки" + wallCoords);
        }
    }

    //todo delete
    //private List<Vector3Int> GetWallCoordinates()
    //{
    //    List<Vector3Int> coords = new List<Vector3Int>();
    //    var renderer = gameObject.GetComponent<Renderer>();
    //    int xMin = (int) Mathf.Round(renderer.bounds.min.x);
    //    int zMin = (int) Mathf.Round(renderer.bounds.min.z);
    //    int xMax = (int) Mathf.Round(renderer.bounds.max.x);
    //    int zMax = (int) Mathf.Round(renderer.bounds.max.z);

    //    int y = (int) Mathf.Round(renderer.bounds.min.y);

    //    for (int i = xMin; i <= xMax; ++i)
    //    {
    //        for (int j = zMin; j <= zMax; ++j)
    //        {
    //            coords.Add(new Vector3Int(i, y, j));
    //        }
    //    }
    //    return coords;
    //}

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Instance.Unsubscribe("GameStarting", AddWallToBoard);
    }
}
