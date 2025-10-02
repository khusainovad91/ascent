using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static List<Vector3Int> ToVector3List(this List<PathNode> pathNodes)
        {
            return pathNodes.Select(node => node.Cell.coords).ToList();
        }

        public static List<Cell> ToCellList(this List<Vector3Int> vector3Ints)
        {
            return vector3Ints.Select(node => BoardManager.Instance.CellsInBoard[node]).ToList();
        }

        public static Coords[] ToCoords(this List<Vector3Int> vector3Ints)
        {
            return vector3Ints.Select(node => Coords.Vector3IntToCoords(node)).ToArray();
        }

        public static List<Vector3Int> ToVector3Int(this Coords[] coords)
        {
            return coords.Select(node => node.ToVector3Int()).ToList(); 
        }

        public static void MoveUp(this UnityEngine.Transform child)
        {
            UnityEngine.Transform parent = child.parent;
            if (parent == null) return; // Если объект на верхнем уровне, выходим

            int index = child.GetSiblingIndex();
            if (index > 0) // Проверяем, что он не первый
            {
                child.SetSiblingIndex(index - 1);
            }
        }

        public static void MoveDown(this UnityEngine.Transform child)
        {
            UnityEngine.Transform parent = child.parent;
            if (parent == null) return; // Если объект на верхнем уровне, выходим

            int index = child.GetSiblingIndex();
            if (index > 0) // Проверяем, что он не первый
            {
                child.SetSiblingIndex(index + 1);
            }
        }


        public static Vector3 CellToWorld(this Vector3Int cellIndex, Grid grid)
        {
            return grid.GetCellCenterWorld(cellIndex);
        }

        public static Vector3 CenterOfCell(this Vector3Int cellIndex)
        {
            return new Vector3(cellIndex.x + 0.5f, 0, cellIndex.z + 0.5f);
        }
        public static Vector3Int GetVector3IntPosition(this FieldObject fieldObject)
        {
            int xInt = (int)Mathf.Round(fieldObject.transform.position.x - 0.5f);
            int yInt = (int)fieldObject.transform.position.y;
            int zInt = (int)Mathf.Round(fieldObject.transform.position.z - 0.5f);
            return new Vector3Int(xInt, 0, zInt);
        }

        public static Cell PlaceObjectInACell(this FieldObject fieldObject)
        {
            int xInt = (int)Mathf.Round(fieldObject.transform.position.x - 0.5f);
            int yInt = (int)fieldObject.transform.position.y;
            int zInt = (int)Mathf.Round(fieldObject.transform.position.z - 0.5f);

            Vector3Int coords = new Vector3Int(xInt, 0, zInt);
            return fieldObject.PlaceObjectOnABoardByCoordinates(coords);
        }

        public static Cell PlaceObjectOnABoardByCoordinates(this FieldObject fieldObject, Vector3Int coords)
        {
            //fieldObject.transform.position = new Vector3(xInt + 0.5f, yInt + 0.5f, zInt + 0.5f);
            if (BoardManager.Instance.CellsInBoard.TryGetValue(coords, out var cell))
            {
                if (fieldObject.IsSolid)
                {
                    BoardManager.Instance.OcupieCellServerRpc(cell.coords, fieldObject.GetNetworkObjectReference());
                    //cell.isOcupied = true;
                    //cell.objectOnTile = fieldObject;
                } else
                {
                    fieldObject.SetCellRpc();
                }

                return cell;
            }
            return null;
        }

        public static void PlaceObjectInACell(this Cell cell, FieldObject fieldObject)
        {
            float xFloat = cell.coords.x + 0.5f;
            float yFloat = fieldObject.transform.position.y;
            float zFloat = cell.coords.z + 0.5f;

            fieldObject.transform.position = new Vector3(xFloat, yFloat, zFloat);

            if (fieldObject.IsSolid)
            {
                BoardManager.Instance.OcupieCellServerRpc(cell.coords, fieldObject.GetNetworkObjectReference());
            }
            else
            {
                fieldObject.SetCellRpc();
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]); // Swap
            }
        }


        public static IEnumerator DisableLayoutAfterFrame(this LayoutGroup lg)
        {
            yield return new WaitForNextFrameUnit(); // Ждем 1 кадр
            lg.enabled = false;
        }


        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }
    }
}

