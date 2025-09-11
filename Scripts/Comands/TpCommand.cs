using ExtensionMethods;
using System;
using UnityEngine;

public class TpCommand : ICommand
{
    private FieldObject _fieldObject;
    private Cell _targetCell;

    public bool IsExecuted { get; set; }

    public TpCommand(FieldObject fieldObject, Cell targetCell)
    {
        _fieldObject = fieldObject;
        _targetCell = targetCell;
    }

    public void Execute()
    {
        BoardManager.Instance.ClearCellServerRpc(_fieldObject.CurrentCell.coords);
        //_fieldObject.CurrentCell.ClearCell();
        _fieldObject.transform.position = _targetCell.coords.CenterOfCell();
        BoardManager.Instance.OcupieCellServerRpc(_targetCell.coords, _fieldObject.GetNetworkObjectReference());
        //_targetCell.OcupieCell(_fieldObject);
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
