using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "Game/BaseHero")]
public class BaseHeroSO : ScriptableObject
{
    [field:SerializeField] public FieldHero HeroPrefab { get; private set; }
    [field:SerializeField] public List<Item> BaseItems {  get; private set; }
    [field:SerializeField] public List<UiCard> BaseAbilities { get; private set; }
}