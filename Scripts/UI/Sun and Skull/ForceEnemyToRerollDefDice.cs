using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForceEnemyToRerollDefDice : Sun
{
    DiceData _SelectedDice;
    bool _denied = false;

    private new void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.Subscribe<MonoBehaviour>("OnSelect", SetSelectedDice);
        EventManager.Instance.Subscribe("OnDenie", SetDenie);
    }

    private new void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.Unsubscribe<MonoBehaviour>("OnSelect", SetSelectedDice);
        EventManager.Instance.Unsubscribe("OnDenie", SetDenie);
    }
    //todo rightclick отмену сделай
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_hac == null)
        {
            return;
        }

        if (_hac.suns - _sunCost < 0) return;

        if (_hac.usedAbilities.Contains(this))
        {
            return;
        }

        base.OnPointerClick(eventData);
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Dice);

        _hac.suns -= _sunCost;
        StartCoroutine(ChooseDiceToReroll());
    }

    private void SetSelectedDice(MonoBehaviour clicked)
    {
        if (clicked is DiceData dice)
        {
            _SelectedDice = dice;
        }
    }

    private void SetDenie()
    {
        _denied = true;
    }

    private IEnumerator ChooseDiceToReroll()
    {
        _heroData.IsMakingChoice = true;
        _SelectedDice = null;
        _denied = false;

        var enemiesDices = _hac.TargetEnemy.GetComponent<DeffendingDicePool>().GetDices();

        foreach (var dice in enemiesDices)
        {
            Debug.Log(dice.name);
            dice.Outline.OutlineColor = Color.white;
            dice.Outline.enabled = true;
        }

        while ((_SelectedDice == null || !enemiesDices.Contains(_SelectedDice)) && !_denied)
        {
            yield return null;
        }

        if (_denied)
        {
            //Return spended points
            _hac.suns += _sunCost;
            base.HighlightThis();
            InTheEnd(enemiesDices);
            yield break;
        }

        _hac.usedAbilities.Add(this);
        _heroData.IsMakingChoice = false;
        _SelectedDice.RollDiceRpc();
        InTheEnd(enemiesDices);
    }

    private void InTheEnd(List<DiceData> enemiesDices)
    {
        foreach (var dice in enemiesDices)
        {
            dice.SetDefaultColor();
            dice.Outline.enabled = false;
        }

        _heroData.IsMakingChoice = false;
        _heroData.ChangeState(HeroState.Attacking);
    }

}
