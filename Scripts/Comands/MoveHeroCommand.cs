using ExtensionMethods;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveHeroCommand : ICommand
{
    private FieldHero fieldHero;
    private List<Cell> path;
    private int currentPathIndex = 0;
    //todo delete
    //private bool isMoving = false;
    //bool IsExecuted { get; set; } = false;
    public bool IsExecuted { get; set; } = false;

    public MoveHeroCommand(FieldHero _fieldHero, List<Cell> _path)
    {
        this.fieldHero = _fieldHero;
        this.path = _path;
    }

    public void Execute()
    {
        //todo delete
        //if (!isMoving || currentPathIndex >= path.Count)
        GameManager.Instance.IsAnythingIsMovingNow = true;

        if (IsExecuted || currentPathIndex >= path.Count)
        {
            return;
        }
        this.fieldHero.HeroData.ChangeState(HeroState.Moving);
        float step = fieldHero.HeroData.Stats.Speed * Time.deltaTime;
        Cell targetCell = path[currentPathIndex];
        Vector3 targetPosition = targetCell.coords.CenterOfCell();

        // Движение персонажа к целевой точке
        fieldHero.LookOn(targetPosition);
        fieldHero.transform.position = Vector3.MoveTowards(fieldHero.transform.position, targetPosition, step);
        CameraController.Instance.FollowTarget(fieldHero.transform);

        // Если достигли текущей цели
        if (fieldHero.transform.position == targetPosition)
        {
            fieldHero.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Move);
            // Обновление текущей ячейки
            BoardManager.Instance.ClearCellServerRpc(fieldHero.CurrentCell.coords);
            fieldHero.CurrentCell = null;
            //fieldHero.OcupiedCells.Clear();
            //fieldHero.OcupiedCells.Remove(fieldHero.CurrentCell);
            BoardManager.Instance.OcupieCellServerRpc(targetCell.coords, fieldHero.GetNetworkObjectReference());
            

            // Уменьшаем очки движения
            fieldHero.HeroData.Stats.ChangeMovementPointsRpc(-1);

            // Переходим к следующей точке пути
            currentPathIndex++;

            //todo что-то может прервать движение
            // Если это была последняя точка
            if (currentPathIndex >= path.Count)
            {
                IsExecuted = true;
                this.fieldHero.HeroData.ChangeState(HeroState.Idle);
                GameManager.Instance.IsAnythingIsMovingNow = false;
            }
        }
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
