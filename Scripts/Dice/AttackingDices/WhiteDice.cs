using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhiteDice : DiceData
{
    public void Awake()
    {
        sides = new List<DiceSide>();
        SideToRotate = new Dictionary<DiceSide, Vector3>();

        DiceSide miss = new DiceSide(0, true);
        sides.Add(miss);
        SideToRotate.Add(miss, new Vector3(118, -39, -38));

        DiceSide side1 = new DiceSide(1, 1, 3, 1, 0);
        sides.Add(side1);
        SideToRotate.Add(side1, new Vector3(-58, -107, -30));

        DiceSide side2 = new DiceSide(2, 1, 4, 1, 0);
        sides.Add(side2);
        SideToRotate.Add(side2, new Vector3(-150, 40, -122));

        DiceSide side3 = new DiceSide(3, 2, 3, 1, 0);
        sides.Add(side3);
        SideToRotate.Add(side3, new Vector3(0, -15, 182));

        DiceSide side4 = new DiceSide(4, 2, 4, 0, 0);
        sides.Add(side4);
        SideToRotate.Add(side4, new Vector3(32, -32, -120));

        DiceSide side5 = new DiceSide(5, 2, 5, 1, 0);
        sides.Add(side5);
        SideToRotate.Add(side5, new Vector3(0, -160, 0));

        DiceSide side6 = new DiceSide(6, 1, 2, 0, 1);
        sides.Add(side6);
        SideToRotate.Add(side6, new Vector3(-121, 5, -35));

        DiceSide side7 = new DiceSide(7, 1, 3, 0, 1);
        sides.Add(side7);
        SideToRotate.Add(side7, new Vector3(-35, -214, -125));

        DiceSide side8 = new DiceSide(8, 2, 2, 0, 1);
        sides.Add(side8);
        SideToRotate.Add(side8, new Vector3(0, -200, -64));

        DiceSide side9 = new DiceSide(9, 2, 3, 0, 0);
        sides.Add(side9);
        SideToRotate.Add(side9, new Vector3(60, 0, -30));

        DiceSide side10 = new DiceSide(10, 2, 4, 0, 1);
        sides.Add(side10);
        SideToRotate.Add(side10, new Vector3(0, 17, 117));

        DiceSide side11 = new DiceSide(11, 3, 6, 1, 1);
        sides.Add(side11);
        SideToRotate.Add(side11, new Vector3(32, 39, 59));
    }
    
}
