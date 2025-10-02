using ExtensionMethods;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class StepCommand : ICommand
{
    private FieldHero _fieldHero;
    private Cell _targetCell;
    public bool IsExecuted { get; set; } = false;

    public StepCommand(FieldHero fieldHero, Cell targetCell)
    {
        _fieldHero = fieldHero;
        _targetCell = targetCell;
    }

    public void Execute()
    {
        Vector3 targetPosition = _targetCell.coords.CenterOfCell();

        if (IsExecuted)
        {
            return;
        }

        _fieldHero.LookOn(_targetCell.coords);
        LeanTween.move(_fieldHero.gameObject, targetPosition, 0.2f).setOnUpdate((float val) => CameraController.Instance.FollowTarget(_fieldHero.transform));

        //todo нельзя сменить на Moving так как если сменить Reacting, то сразу продолжат ходить враги (StepAction 72)
        // Обновление текущей ячейки
        BoardManager.Instance.ClearCellServerRpc(_fieldHero.CurrentCell.coords);
        _fieldHero.CurrentCell = null;
        //_fieldHero.OcupiedCells.Clear(); //
        BoardManager.Instance.OcupieCellServerRpc(_targetCell.coords, _fieldHero.GetNetworkObjectReference());
        //_fieldHero.CurrentCell.ClearCell();
        //_targetCell.OcupieCell(_fieldHero);

        IsExecuted = true;
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public string GetCommandText()
    {
        //Нет необходимости в реализации, не выводится на UI
        throw new NotImplementedException();
    }
}
