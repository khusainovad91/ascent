using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item: ScriptableObject
{
    [field:SerializeField]
    public List<AttackingDice> attackingDicePrefabsNumbers { get; protected set; }
    [field: SerializeField]
    public UiCard UiCard { get; protected set; }
    //todo delete?
    //[SerializeField]    
    //protected bool attackingCard;
    //[SerializeField]
    //protected bool defendingCard;

}
