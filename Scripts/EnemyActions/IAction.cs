using System.Collections;
using UnityEngine;

//todo delete
//������ ���� ���� �������� �����
public interface IAction 
{
    bool IsExecuted { get; set; }
    float Weight { get; set; }
    IEnumerator Execute();
}
