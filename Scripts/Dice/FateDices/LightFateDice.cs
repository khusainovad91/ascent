using System.Collections.Generic;
using UnityEngine;

public class LightFateDice : DiceData
{
    public void Awake()
    {
        sides = new List<DiceSide>();
        SideToRotate = new Dictionary<DiceSide, Vector3>();

        DiceSide side0 = new DiceSide(0, 2);
        sides.Add(side0);
        SideToRotate.Add(side0, new Vector3(90, -90, 0));

        DiceSide side1 = new DiceSide(1, 1);
        sides.Add(side1);
        SideToRotate.Add(side1, new Vector3(180, 90, 0));

        DiceSide side2 = new DiceSide(2, 1);
        sides.Add(side2);
        SideToRotate.Add(side2, new Vector3(0, 0, 90));

        DiceSide side3 = new DiceSide(3, 1  );
        sides.Add(side3);
        SideToRotate.Add(side3, new Vector3(0, 0, 0));

        DiceSide side4 = new DiceSide(4, 0);
        sides.Add(side4);
        SideToRotate.Add(side4, new Vector3(-90, -90, 180));

        DiceSide side5 = new DiceSide(5, 3);
        sides.Add(side5);
        SideToRotate.Add(side5, new Vector3(0, -180, -90));
    }
}
