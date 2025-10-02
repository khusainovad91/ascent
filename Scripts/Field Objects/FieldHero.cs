using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HeroStats))]
[RequireComponent(typeof(AttackingDicePool))]
[RequireComponent(typeof(DeffendingDicePool))]
[RequireComponent(typeof(FateDicePool))]
[RequireComponent(typeof(PersonSoundHandler))]
public class FieldHero : FieldObject
{
    [field: SerializeField] public Sprite HeroSprite { get; private set; }
    [SerializeField] public Animator Animator;

    public HeroData HeroData { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isMovable = true;
        base.OnNetworkSpawn();

        StartCoroutine(SpawnDicePoolsDelayed());
        Debug.Log("Имя героя: " + this.name + " Владелец: " + this.OwnerClientId);

        EventManager.Instance.Subscribe("OnRoundStart", SetUpHeroForNewRound);
    }

    private IEnumerator SpawnDicePoolsDelayed()
    {
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<DeffendingDicePool>().AddBaseDices();
        this.GetComponent<FateDicePool>().AddFateDices();
    }

    public void SetupHeroData(HeroData _heroData)
    {
        this.defaultColor = _heroData.PlayerColor;
        this.HeroData = _heroData;
        Debug.Log("MaxHP HeroData at FieldHero: " + HeroData.Stats.MaxHP);
    }

    [Rpc(SendTo.Server)]
    internal void ChangeStateRpc(HeroState state)
    {
        ChangeStateRpcClientRpc(state);
    }

    [Rpc(SendTo.Everyone)]
    private void ChangeStateRpcClientRpc(HeroState state)
    {
        HandleAnimations(state);
        HeroData.ChangeStateFase2(state);
    }

    private void SetUpHeroForNewRound()
    {
        if (!IsServer) return;
        if (this.GetComponent<ConditionHandler>().ContainsCondition<Fainted>()) return;
        HeroData.Stats.ChangeActionsAmountRpc(HeroData.Stats.maxActionsAmount);
        HeroData.Stats.ChangeMovementPointsRpc(HeroData.Stats.Speed);
        ChangeStateRpc(HeroState.Idle);
        SetHeroDataIsActiveRpc(true);
    }

    [Rpc(SendTo.Server)]
    public void SetHeroDataIsActiveRpc(bool IsActive)
    {
        SetHeroDataIsActiveClientRpc(IsActive);
    }

    [Rpc(SendTo.Everyone)]
    public void SetHeroDataIsActiveClientRpc(bool IsActive)
    {
        HeroData.IsActive = true;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EventManager.Instance.Unsubscribe("OnRoundStart", SetUpHeroForNewRound);
    }

    private void HandleAnimations(HeroState state)
    {
        if (Animator == null) return;
        switch (state)
        {
            case HeroState.Idle:
                Animator.SetBool("Run", false);
                break;
            case HeroState.Moving:
                Animator.SetBool("Run", true);
                break;
            case HeroState.Attacking:
                break;
            case HeroState.Fainted:
                Animator.SetBool("Dead", true);
                break;
            case HeroState.Reacting:
                Animator.SetTrigger("React");
                break;
            default:
                break;
        }
    }

}
