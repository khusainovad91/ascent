using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData: IDisposable
{
    //������ �� ��������� ���������
    public HeroData HeroData;
    //������
    private int _currentBusyHands = 0;
    private int _otherItemAmount = 0;
    //����� �����
    public Dictionary<Item, UiCard> EquipedCards { get; private set; }
    public List<UiCard> Abilities{ get; private set; }
    public List<UiCard> CachedAllCards { get; private set; }
    public List<UiCard> BurnedCards { get; private set; }   
    //������ ������ � �����
    //todo
    //������ ��������� � �����
    List<InventoryItem> inventoryDeck; 
    //public DeffendingDicePool DeffendingDicePool { get; private set; }

    public InventoryData(HeroData heroData)
    {
        this.HeroData = heroData;
        EquipedCards = new Dictionary<Item, UiCard>();
        Abilities = new List<UiCard>();
        CachedAllCards = new List<UiCard>();
        BurnedCards = new List<UiCard>();
        //attackingDicePool = new AttackingDicePool();
        EventManager.Instance?.Subscribe("HeroesSpawned", OnHeroesSpawned);
    }


    public void TryAddItem(Item item)
    {
        switch (item)
        {
            case HandItem handItem:
                if (_currentBusyHands + handItem.handsAmount > HeroData.Stats.AmountOfHands)
                {
                    Debug.Log($"�� ������� ����� � �����, itemHands: {handItem.handsAmount}, currentAmountOfHands: {HeroData.Stats.AmountOfHands} ");
                    //todo �������������
                    break;
                }
                _currentBusyHands += handItem.handsAmount;
                AddItemAndCard(handItem);
                break;
            case ArmourItem armourItem:
                if (EquipedCards.ContainsKey(armourItem))
                {
                    Debug.Log("��� ���� �����: " + armourItem);
                    break;
                }
                AddItemAndCard(armourItem);
                break;

            case OtherItem otherItem:
                if (_otherItemAmount + otherItem.amountOfSpace > HeroData.Stats.AmountOfSpace)
                {
                    Debug.Log("�� ������� �����");
                    break;
                }
                AddItemAndCard(otherItem);
                break;
            default:
                Debug.Log("����������� �������!");
                break;
        }
    }

    private void AddItemAndCard(Item item)
    {
        var uiCard = GameObject.Instantiate(item.UiCard);
        uiCard.setUpHeroData(HeroData);
        CachedAllCards.Add(uiCard);
        EquipedCards.Add(item, uiCard);

        if (item is WeaponItem weaponItem)
        {
            Debug.Log(this.HeroData.FieldHero);
            Debug.Log(this.HeroData.FieldHero.GetComponent<AttackingDicePool>());
            this.HeroData.FieldHero.GetComponent<AttackingDicePool>().TryAddDicesByWeaponCard(weaponItem);
        }
    }

    public void AddAbility(UiCard card)
    {
        var uiCard = GameObject.Instantiate(card);
        uiCard.setUpHeroData(HeroData);
        Abilities.Add(uiCard);
        CachedAllCards.Add(uiCard);
    }

    public void RemoveItem(Item item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAbility(UiCard card)
    {
        Abilities.Remove(card);
        CachedAllCards.Remove(card);
    }

    private void OnHeroesSpawned()
    {
        //Debug.Log("������� ����� ������: " + this.HeroData.Stats.baseDefDicePrefab);

        foreach (var ability in HeroData.BaseHeroSO.BaseAbilities)
        {
            HeroData.InventoryData.AddAbility(ability);
        }
        foreach (var item in HeroData.BaseHeroSO.BaseItems)
        {
            HeroData.InventoryData.TryAddItem(item);
        }

        //todo add
        //DeffendingDicePool = new DeffendingDicePool(HeroData.FieldHero, this.HeroData.Stats.baseDefDicePrefab);
    }

    public void Dispose()
    {
        EventManager.Instance?.Unsubscribe("HeroesSpawned", OnHeroesSpawned);
    }

}
