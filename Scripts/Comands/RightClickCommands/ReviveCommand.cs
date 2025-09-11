using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveCommand : RightClickCommand
{
    public override int AmountOfActions() => 1;

    private bool _isAwaiable;
    public override bool IsExecuted { get; set; } = false;

    public override CommandType Type => CommandType.Revive;

    public override FieldHero Hero { get; set; } = null;

    private FieldHero _heroToRevive;

    public override string GetCommandText()
    {
        return "������� ����� <sprite name=\"arrow\"><sprite name=\"owl\">";
    }
    public override void SetupCommand(FieldHero chosenHero, FieldObject chosenObject)
    {
        if (chosenObject is FieldHero fieldHero)
        {
            if (fieldHero.GetComponent<ConditionHandler>().ContainsCondition<Fainted>() &&
                chosenHero.HeroData.Stats.ActionsAmount > 0 &&
                LineOfSight.CheckLos(chosenHero.CurrentCell, chosenObject) &&
                UtilClass.RangeBetweenCells(chosenHero.CurrentCell, chosenObject.CurrentCell) <= 1
                )
            {
                _isAwaiable = true;
                Hero = chosenHero;
                _heroToRevive = fieldHero;
            }
        }
    }

    public override void Execute()
    {
        //throw dices to test wisdom   
        if (Hero.GetComponent<HeroStats>().ActionsAmount <= 0) return;

        Hero.GetComponent<HeroStats>().ChangeActionsAmountRpc(-1);
        Hero.GetComponent<FateDicePool>().RollAllDicesRpc();
        CalculateSuccessOrNot();
        _heroToRevive.GetComponent<RightClickHandler>().RemoveCommandRpc(CommandType.Revive);
        LeanTween.delayedCall(Constants.DICE_ROLL_TIME + Constants.FATE_DICE_RESSET_TIME, () => { Hero.GetComponent<FateDicePool>().HideDicesRpc(1f); 
        });
        IsExecuted = true;
    }

    private void CalculateSuccessOrNot()
    {
        int result = 0;

        foreach (var diceData in Hero.GetComponent<FateDicePool>().Dices)
        {
            result += diceData.LastRollResult.Value.shields;
        }

        if (result <= Hero.GetComponent<HeroStats>().wisdom)
        {
            Debug.Log("�������� �������!");
            var duration = _heroToRevive.GetComponent<ConditionHandler>().GetConditionByType<Fainted>().Duration;
            _heroToRevive.GetComponent<ConditionHandler>().RemoveConditionByType<Fainted>();
            _heroToRevive.ChangeStateRpc(HeroState.Idle);
            this._isAwaiable = false;

            if (duration < 3)
            {
                _heroToRevive.GetComponent<HeroStats>().ChangeActionsAmountRpc(+2);
                _heroToRevive.GetComponent<HeroStats>().ChangeHealthRpc(Hero.GetComponent<HeroStats>().wisdom - result + 1);
                _heroToRevive.GetComponent<HeroStats>().ChangeMovementPointsRpc(_heroToRevive.GetComponent<HeroStats>().Speed);
            }
        }
    }

    public override bool IsAwaiable()
    {
        return _isAwaiable;
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

}
