using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttackCommand : ICommand
{
    public FieldHero FieldHero;
    private WeaponItem _weaponItem;
    public EnemyObject TargetEnemy;
    public HashSet<SunAndSkull> usedAbilities;

    private bool _attackButtonPressed;
    private int _rangeToTarget;

    private AttackingDicePool _attackingDicePool;
    private DeffendingDicePool _deffendingDicePool;

    public int suns;
    public int skulls;
    public int dmg;
    public int shields;
    public int range;

    public int bonusRange;
    public int bonusDmg;

    public bool IsExecuted { get; set; } = false;

    public HeroAttackCommand(FieldHero fieldHero, WeaponItem weaponItem, EnemyObject target, int rangeToTarget)
    {
        FieldHero = fieldHero;
        _weaponItem = weaponItem;
        TargetEnemy = target;
        _rangeToTarget = rangeToTarget;
        usedAbilities = new HashSet<SunAndSkull>();
    }

    public IEnumerator SetUp()
    {
        suns = 0;
        skulls = 0;
        dmg = 0;
        shields = 0;

        TargetEnemy.SetIsTargetedRpc(true);
        TargetEnemy.Outline.OutlineColor = FieldHero.defaultColor;
        TargetEnemy.LookOn(FieldHero.gameObject.transform.position);
        FieldHero.LookOn(TargetEnemy.gameObject.transform.position);

        //������������� ������
        _deffendingDicePool = TargetEnemy.GetComponent<DeffendingDicePool>();
        //_deffendingDicePool = TargetEnemy.DeffendingDicePool;
        _deffendingDicePool.RollAllDicesRpc();

        //��������� ������
        _attackingDicePool = FieldHero.GetComponent<AttackingDicePool>();
        //_attackingDicePool = new AttackDicePool(_weaponItem, FieldHero);
        _attackingDicePool.RollAllDicesRpc();
        _attackButtonPressed = false;

        LeanTween.delayedCall(Constants.DICE_ROLL_TIME, () => CalculateRangeAnsSunsSkulls());

        Debug.Log("suns " + this.suns);
        //todo � ������������ ���� �������� ������ ������ ����� ������, �� ����� �����
        //���� ������� ���, ����� ��� ���������� ������ �� ����� ����� � ����� �����������, ���� ��� ���������� �������

        while (!_attackButtonPressed)
        {
            yield return null;
        }

        yield break;
    }

    public void CalculateRangeAnsSunsSkulls()
    {
        suns = 0;
        skulls = 0;
        range = 0;
        foreach (var diceData in _attackingDicePool.Dices)
        {
            if (diceData.LastRollResult.Value.miss)
            {
                suns = 0;
                skulls = 0;
                range = 0;
                break;
            }

            suns += diceData.LastRollResult.Value.suns;
            skulls += diceData.LastRollResult.Value.skulls;
            Debug.Log("Added to range" + diceData.LastRollResult.Value.range);
            range += diceData.LastRollResult.Value.range;
        }
        EventManager.Instance.TriggerEvent<HeroAttackCommand>("ChooseSunSkull", this);
    }

    public void CalculateDmg()
    {
        dmg = 0;
        foreach (var diceData in _attackingDicePool.Dices)
        {
            if (diceData.LastRollResult.Value.miss)
            {
                dmg = 0;
                break;
            }
            Debug.Log("added " + diceData.LastRollResult.Value.hits);
            dmg += diceData.LastRollResult.Value.hits;
        }
    }

    public void CalculateDefence()
    {
        shields = 0;
        foreach (var diceData in _deffendingDicePool.Dices)
        {
            shields += diceData.LastRollResult.Value.shields;
        }
    }

    //��������� ��������� �� ������ �����
    public void Execute()
    {
        _attackButtonPressed = true;
        EventManager.Instance.TriggerEvent<HeroAttackCommand>("HacExecuted", this);
        //deal final dmg
        CalculateDmg();
        CalculateDefence();

        if (!_weaponItem.isMelee)
        {
            RangeAttackTerm();
        }

        _attackingDicePool.HideDicesRpc(1f);
        _deffendingDicePool.HideDicesRpc(1f);
        TargetEnemy.SetIsTargetedRpc(false);

        if (dmg + bonusDmg - shields > 0)
        {
            Debug.Log("�������� �����: " + (dmg + bonusDmg - shields));
            TargetEnemy.Stats.ChangeHealthRpc(dmg + bonusDmg - shields);
        }

        Debug.Log("���������� �����: " + dmg);
        Debug.Log("���������� ��������� �����" + bonusDmg);
        Debug.Log("���������� �����: " + (shields));

        IsExecuted = true;
        FieldHero.HeroData.Stats.ChangeActionsAmountRpc(-1);
        FieldHero.ChangeStateRpc(HeroState.Idle);
    }

    private void RangeAttackTerm()
    {
        if (range + bonusRange >= _rangeToTarget) { return; }
        Debug.Log($"�� ������� ���������, range: {range} \n rangeToTarget: {_rangeToTarget}");
        dmg = 0;
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }

    public string GetCommandText()
    {
        //��� ������������� � ����������, �� ��������� �� UI
        throw new NotImplementedException();
    }
}
