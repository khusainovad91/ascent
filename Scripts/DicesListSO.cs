using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DicesListSO : ScriptableObject
{
    public List<DiceData> dicePrefabs;
}
