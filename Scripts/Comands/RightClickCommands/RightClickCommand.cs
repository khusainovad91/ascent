using System.Collections.Generic;
using UnityEngine;

public abstract class RightClickCommand : ICommand
{
    public abstract CommandType Type { get; }
    public abstract FieldHero Hero { get; set; }
    public abstract string GetCommandText();
    /// <summary>
    /// Подготавливает команду к отображению на UI.
    /// Здесь будет решаться судьба IsAwaiable. Например, если герой слишком далеко, то команда не доступна для выполнения.
    /// </summary>
    /// <param name="chosenHero">Герой, который сейчас выбран, необходимо в реализациях проверять на null (герой может быть не выбран)</param>
    /// <param name="chosenObject">Объект, над которым будет выполнятся команда, по идее всегда должен быть this</param>
    public abstract void SetupCommand(FieldHero chosenHero, FieldObject chosenObject);
    public abstract bool IsAwaiable();
    public abstract bool IsExecuted { get; set; }
    public abstract void Execute();
    public abstract void Undo();

    public abstract int AmountOfActions();
}
