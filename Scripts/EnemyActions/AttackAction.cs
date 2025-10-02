using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class AttackAction : IAction
{
    public bool IsExecuted { get; set; } = false;
    public float Weight { get; set; } = 100;

    private readonly EnemyObject _enemyObject;
    public FieldHero TargetHero { get; }
    public int TotalDamage;
    public int Range;
    public List<Condition> Conditions { get; } = new List<Condition>();
    public int Suns;
    public int Skulls;
    public int Shields;

    public AttackAction(EnemyObject enemyObject, FieldHero hero)
    {
        _enemyObject = enemyObject;
        TargetHero = hero;
    }

    public IEnumerator Execute()
    {
        _enemyObject.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Grunt);
        LeanTween.delayedCall(0.4f, () => {
            _enemyObject.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Attack);
            TargetHero.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Die);
            var impact = "Impact " + Random.Range(0, 1);
            Debug.Log(impact);
            TargetHero.Animator.SetTrigger(impact);
        });



        _enemyObject.LookOn(TargetHero.CurrentCell.coords.CenterOfCell());
        TargetHero.LookOn(_enemyObject.CurrentCell.coords.CenterOfCell());
        UiManager.Instance.SetUpThisOnEnemyTurn(TargetHero);
        _enemyObject.ChangeStateRpc(EnemyState.Attacking);
        CameraController.Instance.MoveCameraToTarget(_enemyObject.transform, 1f);
        RollDices();

        yield return new WaitForSeconds(Constants.DICE_ROLL_TIME);
        yield return ApplySkullSunAbilities();

        yield return HandleHeroReaction();

        ApplyDamage();
        yield return new WaitForSeconds(2f);

        HideUIElements();
        _enemyObject.ChangeStateRpc(EnemyState.Idle);
        UiManager.Instance.TurnOffBottomPanel();
    }

    private void RollDices()
    {
        _enemyObject.GetComponent<AttackingDicePool>().RollAllDicesRpc();
        //List<DiceSide> attackResult = _enemyObject.GetComponent<AttackingDicePool>().GetRollResults();
        //List<DiceSide> attackResult = _enemyObject._attackingDicePool.RollAllDices().GetRollResults();
        TargetHero.GetComponent<DeffendingDicePool>().RollAllDicesRpc();
        //List<DiceSide> defendResult = TargetHero.GetComponent<DeffendingDicePool>().GetRollResults();
        //var defendResult = TargetHero.HeroData.InventoryData.DeffendingDicePool.RollAllDices().GetRollResults();

        foreach (var diceData in _enemyObject.GetComponent<AttackingDicePool>().Dices)
        {
            if (diceData.LastRollResult.Value.miss)
            {
                Suns = 0;
                Skulls = 0;
                Range = 0;
                break;
            }

            Suns += diceData.LastRollResult.Value.suns;
            Skulls += diceData.LastRollResult.Value.skulls;
            Range += diceData.LastRollResult.Value.range;
            TotalDamage += diceData.LastRollResult.Value.hits;
        }

        //TotalDamage = attackResult.Sum(dice => dice.hits);
        //Suns = attackResult.Sum(dice => dice.suns);
        //Skulls = attackResult.Sum(dice => dice.skulls);

        foreach (var diceData in TargetHero.GetComponent<DeffendingDicePool>().Dices)
        {
            Shields += diceData.LastRollResult.Value.shields;
        }


        //if (attackResult.Any(dice => dice.miss))
        //{
        //    TotalDamage = Suns = Skulls = 0;
        //}

        //Shields = defendResult.Sum(dice => dice.shields);
    }

    private IEnumerator ApplySkullSunAbilities()
    {
        for (int i = 0; i < _enemyObject.skullSunAbilities.Count && Suns > 0 && Skulls > 0; i++)
        {
            var ability = _enemyObject.skullSunAbilities[i];
            if (ability.Use(this))
            {
                UtilClass.LeanPopUp(_enemyObject.hlzSunAndSkullTexts.transform.GetChild(i).gameObject, LeanTweenType.easeOutBounce);
                yield return new WaitForSeconds(ability.AnimationTime);
            }
        }
    }

    private IEnumerator HandleHeroReaction()
    {
        var heroData = TargetHero.HeroData;
        var reactCards = heroData.InventoryData.CachedAllCards
            .OfType<UiReactOnAttackCard>()
            .Where(card => card.IsClickable)
            .ToList(); // Кешируем, чтобы не перебирать коллекцию дважды

        if (reactCards.Count > 0)
        {
            foreach (var card in reactCards)
            {
                card.SetUpReactCard(this);
            }

            UiManager.Instance.SetUpReactButton(heroData);
            heroData.ChangeState(HeroState.Reacting);

            CameraController.Instance.MoveCameraToTarget(TargetHero.transform, 1f);

            yield return new WaitUntil(() => heroData.CurrentState != HeroState.Reacting);

            foreach (var card in reactCards)
            {
                card.ClearAa();
            }
        }
    }

    private void ApplyDamage()
    {
        int damageTaken = TotalDamage - Shields;
        if (damageTaken > 0)
        {
            TargetHero.HeroData.Stats.ChangeHealthRpc(-damageTaken);
        }
    }

    private void HideUIElements()
    {
        foreach (Transform child in _enemyObject.hlzSunAndSkullTexts.transform)
        {
            UtilClass.LeanPopDown(child.gameObject);
        }

        _enemyObject.GetComponent<AttackingDicePool>().HideDicesRpc(1f);
        TargetHero.GetComponent<DeffendingDicePool>().HideDicesRpc(1f);
        //_enemyObject._attackingDicePool.HideDices(1f);
        //TargetHero.HeroData.InventoryData.DeffendingDicePool.HideDices(1f);
    }
}
