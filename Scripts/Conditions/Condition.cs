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
        //���� �� FieldObject, �� ��� ConditionHanlder enemyObject
    }

    public abstract void DeleteThisCondition();
    //�����, ����� ���������� �� ������� ���� ���������

    //Dispose ������ ���� ������ �� DeleteThisCondition ���� Condition �������� �� ���-��

    protected abstract void PerformHeroCondition(HeroData heroData);
    protected abstract void PerformEnemyCondition(EnemyObject enemyObject);
}
