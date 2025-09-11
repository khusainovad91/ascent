using ExtensionMethods;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.UI.Image;

public static class LineOfSight
{
    //todo delete
    //public static bool CheckLos(Cell cell, FieldObject target)
    //{
    //    //ignore.GetComponent<Collider>().enabled = false;
    //    bool result = CheckLos(cell, target);
    //    //ignore.GetComponent<Collider>().enabled = true;
    //    Debug.Log($"Cell: {cell.coords}, target: {target.name}, result: {result}");
    //    return result;
    //}

    public static bool CheckLos(Cell cell, FieldObject target)//, LayerMask obstacleMask)
    {
        var startPosisiton = cell.coords.CenterOfCell() + Constants.HEAD_LEVEL;

        Vector3 targetPosition = target.CurrentCell.coords.CenterOfCell() + Constants.HEAD_LEVEL;

        Vector3 direction = targetPosition - startPosisiton;

        if (Physics.Raycast(startPosisiton, direction.normalized, out RaycastHit hit))// distance)) //, obstacleMask))
        {
            Debug.DrawLine(startPosisiton, hit.point, Color.blue, 5f);
            if (hit.collider.GetComponentInParent<FieldObject>() == target)
            {
                Debug.Log("Cell true: " + cell);
                return true;
            }
        }
        
        Debug.Log("CheckLos failed for: " + target + " " + target.CurrentCell);
        return false;
    }
}

//previous version of LOS
//using ExtensionMethods;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SocialPlatforms;
//using static UnityEngine.UI.Image;

//public static class LineOfSight
//{
//    //public Transform fieldObject;   // Ссылка на героя
//    //public Transform target;  // Ссылка на врага
//    //public LayerMask obstacleMask; // Маска слоев, которые считаются препятствиями

//    //Если луч хотя бы с одного из углов объекта попал в центр другого объекта, то цель доступна для атаки
//    public static bool CheckLos(Cell cell, FieldObject target, FieldObject ignore)
//    {
//        ignore.GetComponent<Collider>().enabled = false;
//        bool result = CheckLos(cell, target);
//        ignore.GetComponent<Collider>().enabled = true;
//        Debug.Log($"Cell: {cell.coords}, target: {target.name}, result: {result}");
//        return result;
//    }

//    public static bool CheckLos(Cell cell, FieldObject target)//, LayerMask obstacleMask)
//    {
//        Vector3[] ObjectAngles = new Vector3[4];
//        ObjectAngles[0] = cell.coords.CenterOfCell() + new Vector3(-0.5f, 1f, -0.5f);
//        ObjectAngles[1] = cell.coords.CenterOfCell() + new Vector3(0.5f, 1f, 0.5f);
//        ObjectAngles[2] = cell.coords.CenterOfCell() + new Vector3(-0.5f, 1f, 0.5f);
//        ObjectAngles[3] = cell.coords.CenterOfCell() + new Vector3(0.5f, 1f, -0.5f);

//        //ObjectAngles[0] = cell.coords.CenterOfCell() + new Vector3(-0.4f, 0.5f, -0.4f);
//        //ObjectAngles[1] = cell.coords.CenterOfCell() + new Vector3(0.4f, 0.5f, 0.4f);
//        //ObjectAngles[2] = cell.coords.CenterOfCell() + new Vector3(-0.4f, 0.5f, 0.4f);
//        //ObjectAngles[3] = cell.coords.CenterOfCell() + new Vector3(0.4f, 0.5f, -0.4f);

//        //ObjectAngles[0] = cell.coords + new Vector3(0, 0.5f, 0);
//        //ObjectAngles[1] = cell.coords + new Vector3(1, 0.5f, 1);
//        //ObjectAngles[2] = cell.coords + new Vector3(1, 0.5f, 0);
//        //ObjectAngles[3] = cell.coords + new Vector3(0, 0.5f, 1);

//        Vector3 targetPosition = target.CurrentCell.coords.CenterOfCell() + Constants.TOP_OF_COLLIDER;

//        foreach (var anglePosition in ObjectAngles)
//        {
//            Vector3 direction = targetPosition - anglePosition;

//            var hits = Physics.RaycastAll(anglePosition, direction.normalized, direction.magnitude);
//            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

//            foreach (var hit in hits)
//            {
//                Debug.DrawLine(anglePosition, hit.point, Color.blue, 5f);

//                var hitObject = hit.collider.GetComponentInParent<FieldObject>();

//                if (hitObject == null)
//                    continue;

//                if (hitObject == target)
//                    return true;

//                // Если попали в что-то, кроме цели — это препятствие
//                return false;
//            }

//            //if (Physics.Raycast(anglePosition, direction.normalized, out RaycastHit hit))// distance)) //, obstacleMask))
//            //{
//            //    Debug.DrawLine(anglePosition, hit.point, Color.blue, 5f);
//            //    if (hit.collider.GetComponentInParent<FieldObject>() == target)
//            //    {
//            //        return true;
//            //    }
//            //}
//        }
//        Debug.Log("CheckLos failed for: " + target);
//        return false;
//    }
//}
