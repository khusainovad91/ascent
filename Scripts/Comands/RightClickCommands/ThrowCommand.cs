using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowCommand : RightClickCommandSelectEnemy
{
    private bool _isAwaiable;
    public override bool IsAwaiable()
    {
        return _isAwaiable;
    }
    public override bool IsExecuted { get; set; } = false;

    public override FieldHero Hero { get; set; } = null;

    public override CommandType Type => CommandType.Throw;

    public override int AmountOfActions() => 1;

    private FieldObject _thisObject;
    private List<EnemyObject> _enemies;

    public override string GetCommandText()
    {
        return "Throw at enemy <sprite name=\"arrow\"><sprite name=\"strength\">, \n max range = <sprite name=\"strength\">";
    }

    public override void SetupCommand(FieldHero chosenHero, FieldObject chosenObject)
    {
        if (UtilClass.RangeBetweenCells(chosenHero.CurrentCell, chosenObject.CurrentCell) <= 1)
        {
            _isAwaiable = true;
            Hero = chosenHero;
            UiSelectHandler.Instance.Hero = chosenHero.HeroData;
            _thisObject = chosenObject;
        }
    }

    public override void Execute()
    {
        var selectedEnemy = UiSelectHandler.Instance.SelectedEnemy;
        if (selectedEnemy == null) return;
        selectedEnemy.LookOn(Hero.gameObject.transform.position);
        Hero.LookOn(selectedEnemy.gameObject.transform.position);
        selectedEnemy.Targeted();

        Hero.GetComponent<FateDicePool>().RollAllDicesRpc();

        _thisObject.GetComponent<RightClickHandler>().RemoveCommandRpc(CommandType.Throw);
        LeanTween.delayedCall(Constants.DICE_ROLL_TIME + Constants.FATE_DICE_RESSET_TIME, () => {
            Hero.GetComponent<FateDicePool>().HideDicesRpc(1f);
            if (CalculateSuccess())
            {
                //урон равный силе с эффектом стана, враг не кидает кубики защиты (ќ“ —“”Ћј Ќ≈Ћ№«я «јў»“»“№—я ’ј-’ј)
                Debug.Log("¬ыбранный враг: " + selectedEnemy);
                selectedEnemy.Stats.ChangeHealthRpc(Hero.HeroData.Stats.strength);
                if (selectedEnemy != null)
                {
                    selectedEnemy.GetComponent<ConditionHandler>().AddConditionRpc(ConditionType.Stuned);
                    selectedEnemy.NotTargeted();
                    UiSelectHandler.Instance.ClearSelected();
                }
            }
            UiSelectHandler.Instance.EndOfMakingChoice<EnemyObject>(_enemies);
            UiSelectHandler.Instance.RestoreStates();
        });
        IsExecuted = true;
        Hero.HeroData.Stats.ChangeActionsAmountRpc(-AmountOfActions());
        _thisObject.GetNetworkObject().Despawn();
    }

    public override List<EnemyObject> GetTargets()
    {
        _enemies = UtilClass.EnemiesInRange(Hero, Hero.HeroData.Stats.strength).Keys
            .Where(e => !e.isTargeted.Value && LineOfSight.CheckLos(Hero.CurrentCell, e)).ToList();

        return _enemies;
    }

    private bool CalculateSuccess()
    {
        int result = 0;

        foreach (var diceData in Hero.GetComponent<FateDicePool>().Dices)
        {
            result += diceData.LastRollResult.Value.shields;
        }

        if (result <= Hero.GetComponent<HeroStats>().strength)
        {
            return true;
        }

        return false;
    } 

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

}
