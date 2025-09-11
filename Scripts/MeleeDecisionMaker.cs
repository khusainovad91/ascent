using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

//todo delete
public class MeleeDecisionMaker
{
    Dictionary<Type, int> _avaiableCommands;
    EnemyObject _enemyObject;
    

    public MeleeDecisionMaker(Dictionary<Type, int> avaiableCommands, EnemyObject enemyObject)
    {
        _avaiableCommands = avaiableCommands;
        _enemyObject = enemyObject;
    }

}
