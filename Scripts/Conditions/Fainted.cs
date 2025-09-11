using System;
using UnityEngine;

public class Fainted : Condition
{
    public int Duration = 3;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Type = ConditionType.Fainted;
        EventManager.Instance.Subscribe("OnRoundStart", Tick);
    }

    private void Tick()
    {
        Duration -= 1;
        Debug.Log($"�� ������ ��������: {Duration}");
        if (Duration == 0)
        {
            var fieldHero = this.FieldObject as FieldHero;
            PerformHeroCondition(fieldHero.HeroData);
        }
    }

    public override void DeleteThisCondition()
    {
        //����� ����� rebibe me
        this.conditionHandler.RemoveConditionRpc(this.Type);
    }

    protected override void PerformEnemyCondition(EnemyObject enemyObject)
    {
        throw new System.NotImplementedException();
    }

    protected override void PerformHeroCondition(HeroData heroData)
    {
        heroData.CurrentState = HeroState.Dead;
        //����� ��������� � ����, ������ ��� ������� �������� � ������� ����������
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventManager.Instance.Unsubscribe("OnRoundStart", Tick);
    }
}
