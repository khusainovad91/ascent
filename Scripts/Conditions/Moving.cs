using UnityEngine;

//todo delete class
public class Moving : Condition
{
    ////duration ��� isMoving �� �����
    ////�������� ������ ������/����
    //protected override int duration 
    //{   
    //    get => duration; 
    //    set => duration = 0; 
    //}

    ////todo delete ���� ���� �����
    ////public Moving(HeroData heroData)
    ////{
    ////    base.heroData = heroData;
    ////}

    //public override void DeleteThisCondition()
    //{
    //    this.conditionHandler.RemoveConditionRpc(this);
    //    Dispose();
    //}
    public override void DeleteThisCondition()
    {
        throw new System.NotImplementedException();
    }

    protected override void PerformEnemyCondition(EnemyObject enemyObject)
    {
        throw new System.NotImplementedException();
    }

    protected override void PerformHeroCondition(HeroData heroData)
    {
        throw new System.NotImplementedException();
    }
}
