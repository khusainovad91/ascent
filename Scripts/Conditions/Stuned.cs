using System.Collections.Generic;
using UnityEngine;

public class Stuned : Condition
{
    public override void OnNetworkSpawn()
    {
        Type = ConditionType.Stuned;
        base.OnNetworkSpawn();
        EventManager.Instance.Subscribe<HeroData>("OnHeroTurn", PerformHeroCondition);
        EventManager.Instance.Subscribe<EnemyObject>("OnCurrentEnemyTurn", PerformEnemyCondition);
    }

    //ћетод убирает 1 очко действи€ в начале хода игроков
    protected override void PerformHeroCondition(HeroData heroData)
    {
        if (FieldObject is FieldHero fieldHero)
        {
            if (fieldHero == heroData.FieldHero)
            {
                fieldHero.HeroData.Stats.ChangeActionsAmountRpc(-1);
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
        EventManager.Instance.Unsubscribe<EnemyObject>("OnCurrentEnemyTurn", PerformEnemyCondition);
    }

    protected override void PerformEnemyCondition(EnemyObject _enemyObject)
    {
        if (FieldObject is EnemyObject enemyObject)
        {
            if (enemyObject == _enemyObject)
            {
                enemyObject.Stats.ChangeActionsAmountRpc(-1);
                DeleteThisCondition();
            }
        }
    }
}
