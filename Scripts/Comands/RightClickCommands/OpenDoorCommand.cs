using UnityEngine;

public class OpenDoorCommand : RightClickCommandNoSelect
{
    private Door door;

    private bool _isAwaiable;
    public override int AmountOfActions() => 1;
    public override bool IsAwaiable()
    {
        return _isAwaiable;
    }
    public override bool IsExecuted { get; set; } = false;
    public override FieldHero Hero { get; set; } = null;
    public override CommandType Type => CommandType.OpenDoor;

    public override string GetCommandText()
    {
        if (door.NeedKey)
        {
            if (door.KeyToOpen == KeyType.YellowKey)
            {
                return "Желтый ключ <sprite name=\"arrow\"> Открыть дверь"; //TODO Add Yellow sprite to glyphs
            }
        } 
        return "<sprite name=\"arrow\"> Open door";
    }

    public override void SetupCommand(FieldHero chosenHero, FieldObject chosenObject)
    {
        Debug.Log($"Выбранный герой: {chosenHero.CurrentCell} \n Клетка: {chosenObject.CurrentCell}");
        door = chosenObject.GetComponent<Door>();
        Hero = chosenHero;
        bool IsAnoughRange = false;
        var objectCellCoords = chosenObject.CurrentCell.coords;
        var range = 0;

        if (door.XWall)
        {
            //TODO CALLIBRATE COORDS
            IsAnoughRange = UtilClass.RangeBetweenCells(chosenHero.CurrentCell, chosenObject.CurrentCell) <= range
            || UtilClass.RangeBetweenCells(BoardManager.Instance.CellsInBoard[new Vector3Int(objectCellCoords.x - 1, 0, objectCellCoords.z)], chosenHero.CurrentCell) <= range
            || UtilClass.RangeBetweenCells(BoardManager.Instance.CellsInBoard[new Vector3Int(objectCellCoords.x, 0, objectCellCoords.z + 1)], chosenHero.CurrentCell) <= range
            || UtilClass.RangeBetweenCells(BoardManager.Instance.CellsInBoard[new Vector3Int(objectCellCoords.x - 1, 0, objectCellCoords.z + 1)], chosenHero.CurrentCell) <= range;

        }
        else
        {
            IsAnoughRange = UtilClass.RangeBetweenCells(chosenHero.CurrentCell, chosenObject.CurrentCell) <= range
            || UtilClass.RangeBetweenCells(BoardManager.Instance.CellsInBoard[new Vector3Int(objectCellCoords.x, 0, objectCellCoords.z + 1)], chosenHero.CurrentCell) <= range
            || UtilClass.RangeBetweenCells(BoardManager.Instance.CellsInBoard[new Vector3Int(objectCellCoords.x + 1, 0, objectCellCoords.z + 1)], chosenHero.CurrentCell) <= range
            || UtilClass.RangeBetweenCells(BoardManager.Instance.CellsInBoard[new Vector3Int(objectCellCoords.x + 1, 0, objectCellCoords.z)], chosenHero.CurrentCell) <= range;
        }
        if (IsAnoughRange && door.isClosed.Value)
        {
            _isAwaiable = true;
        } else
        {
            _isAwaiable = false;
        }
    }

    public override void Execute()
    {
        door.ChangeWallStateRpc(false);
        Hero.HeroData.Stats.ChangeActionsAmountRpc(-1);
        door.GetComponent<RightClickHandler>().RemoveCommandRpc(CommandType.OpenDoor);
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Free);
        IsExecuted = true;
        EventManager.Instance.TriggerEvent("RecalculateMovement");
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }
}
