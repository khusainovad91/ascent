using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FieldHero))]
public class HeroStats : Stats
{
    //todo закрыть
    //Сила для воинов
    public int strength;
    //Интелект для магов
    public int intelect;
    //Ловкость для ловкачей :()
    public int agility;
    //Мудрость для лекарей
    public int wisdom;

    //Усталость есть только у героев
    public int maxFatigue;
    //Усталость есть только у героев
    public int fatigue;
    //количество рук
    [SerializeField]
    public int AmountOfHands = 2;
    //количество места на себе
    [SerializeField]
    public int AmountOfSpace = 3;

    //public List<GameObject> baseDefDicePrefab;

    //------------------------------------------------------
    // HEALTH
    //------------------------------------------------------

    [Rpc(SendTo.Server)]
    public void ChangeHealthRpc(int _hp)
    {
        this.Hp += _hp;

        if (_hp < 0 && this.Hp <= 0)
        {
            this.Hp = 0;
            this.GetComponent<ConditionHandler>().RemoveAllConditionsRpc();
            this.GetComponent<ConditionHandler>().AddConditionRpc(ConditionType.Fainted);
            this.GetComponent<RightClickHandler>().AddNewCommandRpc(CommandType.Revive);
            this.GetComponent<FieldHero>().HeroData.ChangeState(HeroState.Fainted);
        }

        else if (this.Hp > MaxHP)
        {
            this.Hp = MaxHP;
        }

        ChangeHealthClientRpc(this.Hp);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeHealthClientRpc(int _newHp)
    {
        this.Hp = _newHp;
        EventManager.Instance.TriggerEvent<HeroData>("OnHpChanged", this.gameObject.GetComponent<FieldHero>().HeroData);
    }

    //------------------------------------------------------
    // FATIGUE
    //------------------------------------------------------

    [Rpc(SendTo.Server)]
    public void ChangeFatigueRpc(int _fatigue)
    {
        ChangeFatigueClientRpc(_fatigue);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeFatigueClientRpc(int _fatigue)
    {
        fatigue += _fatigue;

        if (fatigue > maxFatigue)
        {
            ChangeHealthRpc(-(fatigue - maxFatigue));
            fatigue = maxFatigue;
        }
        else if (fatigue < 0)
        {
            fatigue = 0;
        }
        EventManager.Instance.TriggerEvent<HeroData>("OnFatigueChanged", this.gameObject.GetComponent<FieldHero>().HeroData);
    }


    //------------------------------------------------------
    // MOVEMENT POINTS
    //------------------------------------------------------

    [Rpc(SendTo.Server)]
    public void ChangeMovementPointsRpc(int mp)
    {

        ChangeMovementPointsClientRpc(mp);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeMovementPointsClientRpc(int mp)
    {
        //todo check in water is there possibility to move
        // if (mp < 1)
        MovementPoints += mp;

        if (MovementPoints < 0)
        {
            Debug.Log("Trying to make Movement points < 0, wtf");
            MovementPoints = 0;
        }
        EventManager.Instance.TriggerEvent<HeroData>("OnMpChanged", this.gameObject.GetComponent<FieldHero>().HeroData);
    }

    //------------------------------------------------------
    // ACTIONS
    //------------------------------------------------------

    [Rpc(SendTo.Server)]
    public void ChangeActionsAmountRpc(int actionsAmount)
    {
        ChangeActionsAmountClientRpc(actionsAmount);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeActionsAmountClientRpc(int actionsAmount)
    {
        if (actionsAmount + ActionsAmount < 0)
        {
            throw new System.Exception("actionAmount < 0");
        }
        Debug.Log("actionsAmount: " + actionsAmount);
        ActionsAmount += actionsAmount;
        EventManager.Instance.TriggerEvent<HeroData>("OnActionsAmountChange", this.gameObject.GetComponent<FieldHero>().HeroData);
    }

    [Rpc(SendTo.Server)]
    public void SetActionsAmountRpc(int actionsAmount)
    {
        SetActionsAmountClientRpc(actionsAmount);
    }

    [Rpc(SendTo.Everyone)]
    private void SetActionsAmountClientRpc(int actionsAmount)
    {
        this.ActionsAmount = actionsAmount;
    }

    [Rpc(SendTo.Server)]
    public void SetMovementPointsRpc(int movementPoints)
    {
        SetMovementPointsClientRpc(movementPoints);
    }

    [Rpc(SendTo.Everyone)]
    private void SetMovementPointsClientRpc(int movementPoints)
    {
        this.MovementPoints = movementPoints;
    }
}
