using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Class")]
public class ClassSO : ScriptableObject
{
    [SerializeField] string className;
    [SerializeField] List<Item> baseItems;
    [SerializeField] List<UiCard> baseAbilities;
}
