using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public abstract class Condition: NetworkBehaviour
{
    public ConditionType Type;
    protected ConditionHandler conditionHandler;
    public Image Image;
    [NonSerialized]
    public FieldObject FieldObject;

    [Rpc(SendTo.Server)]
    public void SetConditionHandlerRpc(NetworkBehaviourReference _conditionHandler)
    {
        SetConditionHandlerClientRpc(_conditionHandler);
    }

    [Rpc(SendTo.Everyone)]
    private void SetConditionHandlerClientRpc(NetworkBehaviourReference _conditionHandler)
    {
        _conditionHandler.TryGet(out conditionHandler);
        FieldObject = conditionHandler.GetComponent<FieldObject>(); 
        //Если не FieldObject, то это ConditionHanlder enemyObject
    }

    public abstract void DeleteThisCondition();
    //Метод, чтобы отписаться от событий если подсписан

    //Dispose должен быть вызван из DeleteThisCondition если Condition подписан на что-то

    protected abstract void PerformHeroCondition(HeroData heroData);
    protected abstract void PerformEnemyCondition(EnemyObject enemyObject);
}
