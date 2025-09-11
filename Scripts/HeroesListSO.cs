using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "HeroesList", menuName = "HeroesList")]
public class HeroesListSO : ScriptableObject
{
    //id of hero to heroBaseData
    [field:SerializeField] public List<BaseHeroSO> list { get; private set; }
}
