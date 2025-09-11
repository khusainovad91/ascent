using System;
using System.Collections.Generic;
using UnityEngine;

public class Rested : Condition
{

    public override void OnNetworkSpawn()
    {
        Type = ConditionType.Rested;
        base.OnNetworkSpawn();
        EventManager.Instance.Subscribe<HeroData>("OnHeroStateChange", PerformHeroCondition);
    }

    //Метод убирает состояние tired с игрока в конце его хода
    protected override void PerformHeroCondition(HeroData heroData)
    {
        if (FieldObject is FieldHero fieldHero)
        {
            if (fieldHero == heroData.FieldHero && fieldHero.HeroData.CurrentState == HeroState.EndedTurn)
            {
                heroData.Stats.ChangeFatigueRpc(-heroData.Stats.fatigue);
                DeleteThisCondition();
            }

        }
    }
    //public override void Dispose()
    //{
    //    EventManager.Instance.Unsubscribe<HeroData>("OnHeroStateChange", PerformCondition);
    //}

    public override void DeleteThisCondition()
    {
        if (!IsServer) return; //?
        this.conditionHandler.RemoveConditionRpc(this.Type);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventManager.Instance.Unsubscribe<HeroData>("OnHeroStateChange", PerformHeroCondition);
    }

    protected override void PerformEnemyCondition(EnemyObject enemyObject)
    {
        throw new NotImplementedException();
    }
}
