using System.Collections.Generic;
using UnityEngine;

public class Tired : Condition
{
    public override void OnNetworkSpawn()
    {
        Type = ConditionType.Tired;
        base.OnNetworkSpawn();
        EventManager.Instance.Subscribe<HeroData>("OnHeroStateChange", PerformHeroCondition);
    }

    //public Tired(HeroData heroData)
    //{
    //    base.heroData = heroData;
    //EventManager.Instance.Subscribe<HeroData>("OnHeroStateChange", PerformCondition);
    //}

    //Метод убирает состояние tired с игрока в конце его хода
    protected override void PerformHeroCondition(HeroData heroData)
    {
        if (FieldObject is FieldHero fieldHero)
        {
            if (fieldHero == heroData.FieldHero && fieldHero.HeroData.CurrentState == HeroState.EndedTurn)
            {
                DeleteThisCondition();
            }
        }
    }

    public override void DeleteThisCondition()
    {
        this.conditionHandler.RemoveConditionRpc(Type);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventManager.Instance.Unsubscribe<HeroData>("OnHeroStateChange", PerformHeroCondition);
    }

    protected override void PerformEnemyCondition(EnemyObject enemyObject)
    {
        throw new System.NotImplementedException();
    }
}
