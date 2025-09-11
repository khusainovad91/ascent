using System.Collections;
using UnityEngine;

//todo delete
//Должны были быть действия врага
public interface IAction 
{
    bool IsExecuted { get; set; }
    float Weight { get; set; }
    IEnumerator Execute();
}
