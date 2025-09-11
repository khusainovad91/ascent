using System.Collections.Generic;
using UnityEngine;

public class HeavyDefDice : DiceData
{
    public void Awake()
    {
        base.OnNetworkSpawn();
        sides = new List<DiceSide>();
        SideToRotate = new Dictionary<DiceSide, Vector3>();

        DiceSide side0 = new DiceSide(0, 3);
        sides.Add(side0);
        SideToRotate.Add(side0, new Vector3(-75f, 0, 0));

        DiceSide bottomSide = new DiceSide(1, 4);
        sides.Add(bottomSide);
        SideToRotate.Add(bottomSide, new Vector3(181.507f, -182.195f, 0));

        DiceSide side2 = new DiceSide(2, 2);
        sides.Add(side2);
        SideToRotate.Add(side2, new Vector3(27.735f, -21.2f, -71f));

        DiceSide side3 = new DiceSide(3, 2);
        sides.Add(side3);
        SideToRotate.Add(side3, new Vector3(28, -100, 65));
    }
}
