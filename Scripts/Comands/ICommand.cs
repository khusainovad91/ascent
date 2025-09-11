using UnityEngine;

public interface ICommand
{
    public bool IsExecuted { get; set; }
    public void Execute();
    public void Undo();
}
