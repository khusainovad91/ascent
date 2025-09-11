using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ClassListSO")]
public class ClassListSO : ScriptableObject
{
    [SerializeField] List<ClassSO> list;
}
