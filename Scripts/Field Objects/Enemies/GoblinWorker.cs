using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// ������ ��������� (������� ������� ���������, � ����� �������)
public class GoblinWorker : SmallEnemy
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Stats.Coward = true;
    }
    
}
