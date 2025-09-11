using System.Collections.Generic;
using UnityEngine;

public abstract class RightClickCommand : ICommand
{
    public abstract CommandType Type { get; }
    public abstract FieldHero Hero { get; set; }
    public abstract string GetCommandText();
    /// <summary>
    /// �������������� ������� � ����������� �� UI.
    /// ����� ����� �������� ������ IsAwaiable. ��������, ���� ����� ������� ������, �� ������� �� �������� ��� ����������.
    /// </summary>
    /// <param name="chosenHero">�����, ������� ������ ������, ���������� � ����������� ��������� �� null (����� ����� ���� �� ������)</param>
    /// <param name="chosenObject">������, ��� ������� ����� ���������� �������, �� ���� ������ ������ ���� this</param>
    public abstract void SetupCommand(FieldHero chosenHero, FieldObject chosenObject);
    public abstract bool IsAwaiable();
    public abstract bool IsExecuted { get; set; }
    public abstract void Execute();
    public abstract void Undo();

    public abstract int AmountOfActions();
}
