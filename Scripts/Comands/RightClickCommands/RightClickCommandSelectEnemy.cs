using System.Collections.Generic;
using UnityEngine;

public abstract class RightClickCommandSelectEnemy : RightClickCommand
{
    public abstract List<EnemyObject> GetTargets();
}
