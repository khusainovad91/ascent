using System;
using System.Collections.Generic;
using UnityEngine;


//FieldObject vars
//public string objectName;
//public bool isSolid;
//public Cell currentCell;

//protected bool isMovable;
//public int speed;

public class SmallEnemy : EnemyObject
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        this.IsSolid = true;
    }
    

}
