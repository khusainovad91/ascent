using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using TMPro;

public static class UtilClass
{
    public static List<Vector3Int> GetRenderCoordinates(GameObject gameObject)
    {
        List<Vector3Int> coords = new List<Vector3Int>();
        var renderer = gameObject.GetComponent<Renderer>();
        int xMin = (int)Mathf.Round(renderer.bounds.min.x);
        int zMin = (int)Mathf.Round(renderer.bounds.min.z);
        int xMax = (int)Mathf.Round(renderer.bounds.max.x);
        int zMax = (int)Mathf.Round(renderer.bounds.max.z);

        int y = (int)Mathf.Round(renderer.bounds.min.y);

        for (int i = xMin; i <= xMax; ++i)
        {
            for (int j = zMin; j <= zMax; ++j)
            {
                coords.Add(new Vector3Int(i, y, j));
            }
        }
        return coords;
    }


    public static List<Tuple<int, int>> FindPossibleNeighbourCoord(Cell cell)
    {
        List<Tuple<int, int>> possibleNeighbourCoords = new List<Tuple<int, int>>();
        //cases
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x - 1, cell.coords.z - 1));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x - 1, cell.coords.z));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x - 1, cell.coords.z + 1));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x, cell.coords.z - 1));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x, cell.coords.z + 1));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x + 1, cell.coords.z - 1));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x + 1, cell.coords.z));
        possibleNeighbourCoords.Add(new Tuple<int, int>(cell.coords.x + 1, cell.coords.z + 1));
        return possibleNeighbourCoords;
    }

    public static int RangeBetweenCells(Cell cellOne, Cell cellTwo) =>
        Mathf.Max(Mathf.Abs(cellTwo.coords.z - cellOne.coords.z), Mathf.Abs(cellTwo.coords.x - cellOne.coords.x));
   

    // Возвращает словарь ячеек с дальностью _range
    public static Dictionary<Vector3Int, Cell> GetCellsInRange(Cell starterPoint, float _range)
    {
        return BoardManager.Instance.CellsInBoard.
                Where(item => Vector3.Distance(item.Key, starterPoint.coords) <= Mathf.Sqrt(2 * _range * _range)). //достаем дальние углы по диагонали sqrt(a^2 + b^2) 
                Where(item => Mathf.Abs(Mathf.Abs(item.Key.x) - Mathf.Abs(starterPoint.coords.x)) <= _range && Mathf.Abs(Mathf.Abs(item.Key.z) - Mathf.Abs(starterPoint.coords.z)) <= _range). //маскируем лишние квадраты
                ToDictionary(i => i.Key, i => i.Value);
    }


    public static Color DarkenColor(Color color, float amount = 0.2f)
    {

        amount = Mathf.Clamp01(amount); // Ограничиваем от 0 до 1
        return new Color(color.r * (1 - amount), color.g * (1 - amount), color.b * (1 - amount), color.a);
    }


    //--------------------------------------------------------------------
    // Enemy
    //--------------------------------------------------------------------


    //--------------------------------------------------------------------
    // LeanTween
    //--------------------------------------------------------------------

    public static void LeanPopUp(GameObject objectToActivate, LeanTweenType type)
    {
        objectToActivate.transform.localScale = Vector3.zero;  // Устанавливаем начальный размер в 0
        LeanTween.scale(objectToActivate, Vector3.one, 1f).setEase(type);
        // Также можно плавно изменять прозрачность с помощью альфа-канала
        LeanTween.alpha(objectToActivate.GetComponent<RectTransform>(), 1f, 1f).setEase(LeanTweenType.easeOutSine);
    }

    public static void LeanPopDown(GameObject objectToDeactivate, float time)
    {
        LeanTween.scale(objectToDeactivate, Vector3.zero, time).setEase(LeanTweenType.easeInBack);

        // Также можно плавно уменьшать прозрачность с помощью альфа-канала
        LeanTween.alpha(objectToDeactivate.GetComponent<RectTransform>(), 0f, 1f).setEase(LeanTweenType.easeInSine);
    }

    public static void LeanPopDown(GameObject objectToDeactivate)
    {
        // Плавно уменьшаем масштаб объекта от текущего размера до 0
        LeanTween.scale(objectToDeactivate, Vector3.zero, 1f).setEase(LeanTweenType.easeInBack);

        // Также можно плавно уменьшать прозрачность с помощью альфа-канала
        LeanTween.alpha(objectToDeactivate.GetComponent<RectTransform>(), 0f, 1f).setEase(LeanTweenType.easeInSine);
    }

    public static IEnumerator PlayClickAnimation(GameObject obj,float scaleFactor = 0.9f, float duration = 0.2f)
    {
        Debug.Log("Click animation");
        //Vector3 originalScale = obj.transform.localScale;
        Vector3 originalScale = new Vector3(1,1,1);
        Vector3 targetScale = originalScale * scaleFactor;

        LeanTween.sequence()
            .append(LeanTween.scale(obj, targetScale, duration / 2).setEase(LeanTweenType.easeOutQuad))
            .append(LeanTween.scale(obj, originalScale, duration / 2).setEase(LeanTweenType.easeOutQuad));
        yield return new WaitUntil(() => !obj.LeanIsTweening());
    }

    public static void ChooseMeAnimation(GameObject _gameObject)
    {
        LeanTween.scale(_gameObject, new Vector3(1.2f, 1.2f, 1.2f), 1)
            .setLoopPingPong()
            .setEase(LeanTweenType.easeOutBack);
    }

    public static void StopAnimation(GameObject _gameObject)
    {
        LeanTween.scale(_gameObject, Constants.VECTOR_1, 1).setOnComplete(() => LeanTween.cancel(_gameObject));
    }

    public static void SetNewParentForAllChilds(UnityEngine.Transform parent, UnityEngine.Transform newParent)
    {
        while (parent.childCount > 0)
        {
            parent.transform.GetChild(parent.childCount - 1).transform.SetParent(newParent);
        }
    }
    /// <summary>
    /// Delete All Childrens
    /// </summary>
    /// <param name="parent"></param>
    public static void DestroyChilds(UnityEngine.Transform parent)
    {
        List<UnityEngine.Transform> childs = new List<UnityEngine.Transform>();
        foreach (UnityEngine.Transform child in parent)
        {
            childs.Add(child);
        }

        foreach (var child in childs)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static IEnumerator BurnCard(RectTransform panel)
    {
        panel.pivot = new Vector2(panel.pivot.x, 0); // Устанавливаем Pivot снизу

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>(); // Добавляем CanvasGroup, если его нет

        float duration = 0.5f;
        bool isAnimationComplete = false;

        // Анимация изменения высоты
        LeanTween.value(panel.gameObject, panel.sizeDelta.y, 0, duration)
            .setOnUpdate((float newHeight) =>
            {
                panel.sizeDelta = new Vector2(panel.sizeDelta.x, newHeight);
            })
            .setEase(LeanTweenType.easeInOutQuad);

        // Анимация исчезновения (прозрачность)
        LeanTween.alphaCanvas(canvasGroup, 0, duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => isAnimationComplete = true);

        // Ждём окончания анимации
        yield return new WaitUntil(() => isAnimationComplete);
    }

    //----------------------------------------
    // SELECTS
    //----------------------------------------

    /// <summary>
    /// Метод ищет объекты вокруг с ограниченной дальностью
    /// Метод учитывает стены, но не LOS
    /// </summary>
    /// <typeparam name="T">Объекты для поиска, должны быть FieldObject</typeparam>
    /// <param name="currentCell">Текущая позиция</param>
    /// <param name="range">Максимальная дистанция</param>
    /// <returns></returns>
    public static List<T> FindObjectsInRange<T> (Cell currentCell, int range) where T : FieldObject
    {
        List<T> result = new List<T>();
        ObjectFinder of = new ObjectFinder(currentCell, range);
        var objectsAround = of.SearchObjectsInRage<T>();

        PathFinder pf = new PathFinder(currentCell, range);
        foreach (var item in objectsAround)
        {
            Debug.Log("Объекты вокруг: " + item);
            var path = pf.FindPathToOcupiedCellMaxLength(currentCell, item.Key.CurrentCell, range);

            if (path != null && path.Count + 1 <= range) //+1 потому что сам герой не считается
            {
                Debug.Log("Дистанция до них: " + path.Count + 1);
                Debug.Log("Максимальная дистанция: " + range);
                result.Add(item.Key);
            }
        }

        return result;
    }

    public static Dictionary<T, List<PathNode>> FindObjectsInRangeWithPath<T>(Cell currentCell, int range) where T : FieldObject
    {
        Dictionary<T, List<PathNode>> result = new Dictionary<T, List<PathNode>>();
        ObjectFinder of = new ObjectFinder(currentCell, range);
        var objectsAround = of.SearchObjectsInRage<T>();

        PathFinder pf = new PathFinder(currentCell, range);
        foreach (var item in objectsAround)
        {
            Debug.Log("Объекты вокруг: " + item);
            var path = pf.FindPathToOcupiedCellMaxLength(currentCell, item.Key.CurrentCell, range);
            if (path != null && path.Count + 1 <= range) //+1 потому что сам герой не считается
            {
                Debug.Log("Дистанция до них: " + path.Count + 1);
                Debug.Log("Максимальная дистанция: " + range);
                result.Add(item.Key, path);
            }
        }

        return result;
    }


    //С Pathfinder
    public static List<Cell> FindEmptyCellsInRange(Cell currentCell, int range)
    {
        List<Cell> result = new List<Cell>();
        ObjectFinder of = new ObjectFinder(currentCell, range);
        var emptyCellsAround = of.FindEmptyCellsInRange();

        PathFinder pf = new PathFinder(currentCell, range);
        foreach (var cell in emptyCellsAround)
        {
            var path = pf.FindPathMaxLengthUpdateCells(currentCell, cell, range);
            if (path != null && path.Count <= range)
            {
                result.Add(cell);
            }
        }
        return result;
    }


    //-----------------------------
    //RANGE
    //-----------------------------

    public static int CalulcateDistance(Cell cell1, Cell cell2)
    {
        int dist = (int)(Vector3.Distance(cell1.coords, cell2.coords));
        Debug.Log("Distance: " + dist);
        return dist;
    }

    public static Dictionary<EnemyObject, int> EnemiesInRange(FieldHero fieldHero, int range)
    {
        var objectFinder = new ObjectFinder(fieldHero.CurrentCell, range);
        return objectFinder.SearchObjectsInRage<EnemyObject>();
    }

    public static void ShowEnemiesInRange(Dictionary<EnemyObject, int> enemiesInRange)
    {
        foreach (var enemy in enemiesInRange)
        {
            Debug.Log("enemy: " + enemy);
            enemy.Key.Outline.OutlineColor = Color.white;
            enemy.Key.Outline.enabled = true;
            //enemy.Key.RangeFromHero.GetComponentInChildren<TMP_Text>().text = enemy.Value.ToString();
            //UtilClass.LeanPopUp(enemy.Key.RangeFromHero, LeanTweenType.easeOutBounce);
            
            //enemy.Key.RangeFromHero.SetActive(true);
            //todo isTargeted true
        }
    }

    public static void ShowEnemiesInRange(List<EnemyObject> enemiesInRange)
    {
        foreach (var enemy in enemiesInRange)
        {
            Debug.Log("enemy: " + enemy);
            enemy.Outline.OutlineColor = Color.white;
            enemy.Outline.enabled = true;
            //todo isTargeted true
        }
    }
}
