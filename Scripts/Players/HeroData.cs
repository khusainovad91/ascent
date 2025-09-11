using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//dada Херодата
public class HeroData
{
    public HeroState CurrentState = HeroState.Idle;
    public ulong PlayerId;
    public int ClassId;
    public int HeroNumber;

    public string heroName;
    public string heroClass;

    public Color PlayerColor;

    public Sprite HeroImage { get; private set; }
    public NetworkObjectReference FieldHeroReference;
    public bool IsActive;
    public bool IsMakingChoice;

    public BaseHeroSO BaseHeroSO;
    public FieldHero FieldHero { get; private set; }
    public HeroStats Stats { get; private set; }
    //public ConditionHandler ConditionHandler { get; private set; }
    public InventoryData InventoryData { get; private set; }

    public HeroData(int heroNumber, ulong playerId, int classId, BaseHeroSO baseHeroSO, Color playerColor)
    {
        this.HeroNumber = heroNumber;
        this.PlayerId = playerId;
        this.ClassId = classId;
        this.PlayerColor = playerColor;
        this.BaseHeroSO = baseHeroSO;

        this.HeroImage = baseHeroSO.HeroPrefab.HeroSprite;
        this.FieldHero = baseHeroSO.HeroPrefab.GetComponent<FieldHero>();
        this.Stats = baseHeroSO.HeroPrefab.GetComponent<HeroStats>();

        this.IsMakingChoice = false;
        FieldHeroReference = this.FieldHero.NetworkObject;

        //this.ConditionHandler = new ConditionHandler();
        this.InventoryData = new InventoryData(this);

        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent<HeroData>("OnHeroStatsChange", this);
            //EventManager.Instance.Subscribe<HeroData>("OnHeroTurn", SetUpHeroForNewRound);
        }
    }

    //Необходимо, так как при создании HeroData подставляется SO, при спауне объекта надо подставлять ссылку на NetworkObject, а не на SO
    //Иначе при изменении SO меняется префаб => меняются статы у уже заспауненых, жопа
    public void SetUpFieldHero(FieldHero fieldHero)
    {
        this.FieldHero = fieldHero;
        this.Stats = fieldHero.GetComponent<HeroStats>();
        this.FieldHero = fieldHero.GetComponent<FieldHero>();
    }

    public void ChangeState(HeroState state)
    {
        this.FieldHero.ChangeStateRpc(state);
    }

    public void ChangeStateFase2(HeroState state)
    {
        Debug.Log("Поменял состояние героя на: " + state);
        this.CurrentState = state;
        EventManager.Instance.TriggerEvent<HeroData>("OnHeroStateChange", this);

        if (state == HeroState.EndedTurn)
        {
            EndOfTurn(this);
        }
    }

    private void EndOfTurn(HeroData heroData)
    {
        if (this == heroData)
        {
            FieldHero.SetHeroDataIsActiveRpc(false);
            Stats.SetActionsAmountRpc(0);
            Stats.SetMovementPointsRpc(0);
        }
    }

}

public enum HeroState
{
    Idle,
    Moving,
    Attacking,
    RollingDice,
    IsTargeted,
    ConfirmingAttack,
    EndedTurn,
    Reacting,
    Fainted,
    Dead
}