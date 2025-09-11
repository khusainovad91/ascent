using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// Гоблин подпездыш (пробует подойти атаковать, а затем убежать)
public class GoblinWorker : SmallEnemy
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Stats.Coward = true;
    }
    
}
