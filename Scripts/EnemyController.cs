using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using CodeMonkey;
using System.Data.Common;
using static UnityEngine.GraphicsBuffer;
using NUnit.Framework.Constraints;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(EnemyObject))]
public class EnemyController : NetworkBehaviour
{
    private EnemyObject _enemyObject;
    private EnemyStats Stats;
    //-------------------------------------------------------------------------
    protected bool hasAttacked = false;
    protected bool hasSpendActionOnMove = false;
    public AttackingCell AttackCell;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _enemyObject = GetComponent<EnemyObject>();
        Stats = GetComponent<EnemyStats>();
        EventManager.Instance.Unsubscribe("HeroReacted", FindNewAttackCell);
    }   

    public IEnumerator StartTurn()
    {
        hasAttacked = false;
        hasSpendActionOnMove = false;

        PopUpStatsClientRpc();

        //todo remake on RPC, сейчас выполняется дроп только на сервере
        Stats.DropMovementPointRpc();
        Stats.DropActionsRpc();

        Stats.ChangeMovementPointsClientRpc(Stats.Speed);
        Stats.ChangeActionsAmountClientRpc(Stats.maxActionsAmount);

        CameraController.Instance.MoveCameraToTargetRpc(_enemyObject.GetNetworkObjectReference(), 0.6f);
        yield return new WaitForSeconds(0.6f);

        EventManager.Instance.TriggerEvent<EnemyObject>("OnCurrentEnemyTurn", _enemyObject);

        yield return DoActions();

        PopDownStatsClientRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void PopUpStatsClientRpc()
    {
        Stats.HealthBar.gameObject.SetActive(true);
        Stats.ActionsBar.gameObject.SetActive(true);
        Stats.MpBar.gameObject.SetActive(true);

        UtilClass.LeanPopUp(Stats.HealthBar.gameObject, LeanTweenType.easeOutBounce);
        UtilClass.LeanPopUp(Stats.ActionsBar.gameObject, LeanTweenType.easeOutBounce);
        UtilClass.LeanPopUp(Stats.MpBar.gameObject, LeanTweenType.easeOutBounce);
        UtilClass.LeanPopDown(_enemyObject.RangeFromHero);
    }

    [Rpc(SendTo.Everyone)]
    private void PopDownStatsClientRpc()
    {
        UtilClass.LeanPopDown(Stats.HealthBar.gameObject);
        UtilClass.LeanPopDown(Stats.ActionsBar.gameObject);
        UtilClass.LeanPopDown(Stats.MpBar.gameObject);
    }

    public IEnumerator DoActions()
    {
        FindNewAttackCell();
        if (AttackCell != null)
        {
            Debug.Log("AttackCell: " + AttackCell.cell.coords);
            Debug.Log($"hasAttcked: {hasAttacked}, Stats.MovementPoints: {Stats.MovementPoints}");

            while (!hasAttacked && Stats.MovementPoints > 0) //&& AttackCell != null)
            {
                if (_enemyObject.CurrentCell != AttackCell.cell && Stats.MovementPoints <= 0 && hasSpendActionOnMove) break;

                if (_enemyObject.CurrentCell == AttackCell.cell)
                {
                    if (LineOfSight.CheckLos(_enemyObject.CurrentCell, AttackCell.target)
                      && UtilClass.RangeBetweenCells(_enemyObject.CurrentCell, AttackCell.target.CurrentCell) <= Stats.AttackRange)
                        {
                        Debug.Log("AttackCell.target.CurrentCell: " + AttackCell.target.CurrentCell);
                            AttackAction aa = new AttackAction(_enemyObject, AttackCell.target);
                            yield return aa.Execute();
                            hasAttacked = true;
                            break;
                        } else
                        {
                            FindNewAttackCell();
                        }
                }

                    Debug.Log($"Количество оставшихся очков движенния {Stats.MovementPoints} \n" +
                        $"Дальность атаки {Stats.AttackRange} \n" +
                        $"Дальность до клетки {AttackCell.range}");

                    if (Stats.MovementPoints + Stats.AttackRange <= AttackCell.range)
                    {
                        if (CanSpendActionOnMove())
                        {
                            yield return SpendActionOnMove();
                        }
                    }

                var localAttackCell = AttackCell;

                if (localAttackCell?.pathToCell != null)
                {
                    for (int i = 0; i < localAttackCell.pathToCell.Count && Stats.MovementPoints > 0; i++)
                        {
                            StepAction moveAction = new StepAction(_enemyObject, AttackCell.pathToCell[i].Cell);
                            yield return moveAction.Execute();
                        }
                }

            }
        }
        else
        {
            Debug.Log("aCell is null");
        }

        //todo special action

        if (!hasAttacked)
        {
            yield return StartCoroutine(RunToNearestHero());
        }

        //yield return StartCoroutine(TrySpecialActions());

        //if (hasAttacked)
        //{
        //    yield return StartCoroutine(TryEscape());
        //    yield return StartCoroutine(TryMoveAway());
        //}

        AttackCell = null;
    }

    private void FindNewAttackCell()
    {
        AttackCell = CanSpendActionOnMove()
                        ? FindPositionForAttack(Stats.MovementPoints + Stats.Speed + Stats.AttackRange)
                        : FindPositionForAttack(Stats.MovementPoints + Stats.AttackRange);
    }

    protected virtual IEnumerator TrySpecialActions()
    {
        List<IAction> specialActions = _enemyObject.GetSpecialActions();
        specialActions.Sort((a, b) => b.Weight.CompareTo(a.Weight));

        foreach (var action in specialActions)
        {
            if (Stats.ActionsAmount > 0)
            {
                yield return action.Execute();
            }
            else
            {
                yield break;
            }
        }
    }

    //lasts choice
    private IEnumerator RunToNearestHero()
    {
        //ObjectFinder of = new ObjectFinder(_enemyObject.CurrentCell);
        PathFinder pf = new PathFinder(true);
        List<List<PathNode>> waysToHeroes = new List<List<PathNode>>();

        foreach (var heroData in GameManager.Instance.Heroes)
        {
            if (heroData.CurrentState == HeroState.Fainted) continue;
            var deWay = pf.FindPathToOcupiedCell(_enemyObject.CurrentCell, heroData.FieldHero.CurrentCell);
            if (deWay != null)
            {
                waysToHeroes.Add(deWay);
            }
        }

        waysToHeroes.OrderBy(it => it.Count());

        var pathToHero = waysToHeroes.First();
        Debug.Log("Дальность до блиайшего героя: " + pathToHero.Count);
        if (_enemyObject.CurrentCell == pathToHero.Last().Cell)
        {
            yield break;
        }

        yield return RunByPath(pathToHero);

    }

    //possibleRnage = possibleMp + Range
    private AttackingCell FindPositionForAttack(int possibleRange)
    {

        List<AttackingCell> cells = new List<AttackingCell>();

        Dictionary<FieldHero, List<PathNode>> heroToPath = UtilClass.FindObjectsInRangeWithPath<FieldHero>(_enemyObject.CurrentCell, possibleRange);

        //Убираем героев, у которых хп меньше 0
        foreach (var hero in heroToPath.Keys.Where(h => h.GetComponent<HeroStats>().Hp <= 0).ToList())
        {
            heroToPath.Remove(hero);
        }

        foreach (var hero in heroToPath)
        {
            //добавить текущую клетку к списку
            if (LineOfSight.CheckLos(_enemyObject.CurrentCell, hero.Key)
                && UtilClass.RangeBetweenCells(_enemyObject.CurrentCell, hero.Key.CurrentCell) <= _enemyObject.Stats.AttackRange)
            {
                cells.Add(new AttackingCell(_enemyObject.CurrentCell, hero.Key, UtilClass.RangeBetweenCells(_enemyObject.CurrentCell, hero.Key.CurrentCell), null));
            }

            var emptyCells = UtilClass.FindEmptyCellsInRange(hero.Key.CurrentCell, Stats.AttackRange);

            if (emptyCells.Count == 0)
            {
                Debug.Log($"Нет свободных клеток рядом с героем {hero.Key.name}");
            }

            foreach (var cell in emptyCells)
            {
                PathFinder pf = new PathFinder(_enemyObject.CurrentCell, possibleRange);

                if (!LineOfSight.CheckLos(cell, hero.Key))
                {
                    Debug.Log($"Нет линии видимости от {cell} до {hero.Key.name}");
                    continue;
                }

                if (UtilClass.CalulcateDistance(cell, hero.Key.CurrentCell) > Stats.AttackRange)
                {
                    Debug.Log($"Клетка {cell} слишком далеко от героя {hero.Key.name}");
                    continue;
                }

                int range = hero.Value.Count();
                if (range <= possibleRange)
                {
                    List<PathNode> pathToCell = pf.FindPathMaxLengthUpdateCells(_enemyObject.CurrentCell, cell, possibleRange);

                    if (pathToCell == null)
                    {
                        Debug.Log($"Не найден путь до {cell} от {_enemyObject.name}");
                        continue;
                    }

                    AttackingCell aCell = new AttackingCell(cell, hero.Key, pathToCell.Count, pathToCell);
                    cells.Add(aCell);
                }
                else
                {
                    Debug.Log($"До героя {hero.Key.name} слишком далеко идти: {range} > {possibleRange}");
                }
            }
        }

        if (cells.Count > 1)
        {
            if (Stats.Coward)
            {
                var result = cells.OrderBy(cell => cell.pathToCell == null ? -1 : cell.pathToCell.Count).
                    ThenBy(cell => cell.target.HeroData.Stats.Hp).ToList().First();
                Debug.Log("AttackCell " + result.cell.coords);
                return result;
            }
            else
            {
                var result = cells.OrderBy(cell => cell.target.HeroData.Stats.Hp)
                    .ThenBy(cell => cell.pathToCell == null ? -1 : cell.pathToCell.Count).ToList().First();
                Debug.Log("AttackCell " + result.cell.coords);
                return result;
            }
        }

        Debug.Log("Kek2");
        return null;
    }

    public virtual IEnumerator TryEscape()
    {
        while (Stats.Coward && CanMove())
        {
            ObjectFinder of = (Stats.ActionsAmount > 0)
                ? new ObjectFinder(_enemyObject.CurrentCell, Stats.MovementPoints + Stats.Speed)
                : new ObjectFinder(_enemyObject.CurrentCell, Stats.MovementPoints);

            var path = of.PathToRandomEmptyCellOnMaximumRangeFromCurrent();
            if (path == null || path.Count == 0)
            {
                Debug.Log("No path found.");
                break;
            }

            var targetCell = path.Last().Cell;
            Debug.Log("targetCell " + targetCell);

            yield return RunByPath(path);

            // Подождём кадр, чтобы обновились статы после движения
            yield return null;
        }

        // Выход из корутины, если больше двигаться нельзя или не нужно
        yield break;
    }

    public virtual IEnumerator TryMoveAway()
    {
        yield return null;
    }

    public IEnumerator RunByPath(List<PathNode> pathToCell)
    {
        if (CanSpendActionOnMove())
        {
            yield return SpendActionOnMove();
        }

        for (int i = 1; i < pathToCell.Count && Stats.MovementPoints > 0; i++) {
            if (pathToCell[i].Cell.isOcupied)
            {
                yield break;
            } else
            {
                StepAction move = new StepAction(_enemyObject, pathToCell[i].Cell);
                yield return StartCoroutine(move.Execute());
            }
        }
    }

    //todo delete?
    //protected bool CanAttack(FieldHero target)
    //{
    //    //Debug.Log($"hasAttacked: {hasAttacked}, Stats.ActionsAmount: {Stats.ActionsAmount}");
    //    if (hasAttacked || Stats.ActionsAmount <= 0 || target.GetComponent<Stats>().Hp <= 0) { return false; }
    //    var checkLos = LineOfSight.CheckLos(_enemyObject.CurrentCell, target);
    //    Debug.Log("chechLos " + checkLos);
    //    if (checkLos)
    //    {
    //        Debug.Log("Рейндж атаки: " + Stats.AttackRange);
    //        if (UtilClass.CalulcateDistance(_enemyObject.CurrentCell, target.CurrentCell) <= Stats.AttackRange)
    //        {
    //            Debug.Log("Enemy Object cell " + _enemyObject.CurrentCell);
    //            Debug.Log("Target cell " + target.CurrentCell);
    //            Debug.Log("Distance " + UtilClass.CalulcateDistance(_enemyObject.CurrentCell, target.CurrentCell));
    //            Debug.Log("Attack range " + Stats.AttackRange);
    //            Stats.ChangeActionsAmountRpc(-1);
    //            hasAttacked = true;
    //            return true;
    //        }

    //    }

    //    return false;
    //}


    private bool CanMove()
    {
        return CanSpendActionOnMove() || Stats.MovementPoints > 0;
    }

    private bool CanSpendActionOnMove()
    {
        if (hasSpendActionOnMove || Stats.ActionsAmount <= 0) { return false; }

        return true;
    }

    private IEnumerator SpendActionOnMove()
    {
        hasSpendActionOnMove = true;
        Stats.ChangeActionsAmountClientRpc(-1);
        Stats.ChangeMovementPointsClientRpc(+Stats.Speed);
        yield return new WaitForSeconds(1f);
    }

    [ClientRpc]
    public void PassHeroReactionMessageToMessageClientRpc(NetworkObjectReference fieldHeroNOR, 
        ClientRpcParams rpcParams = default)
    {
        fieldHeroNOR.TryGet(out NetworkObject fieldHeroNO);
        var fieldHero = fieldHeroNO.GetComponent<FieldHero>();
        Debug.Log("Client got hero message" + fieldHero.name);


        var heroData = fieldHero.HeroData;
        var reactCards = heroData.InventoryData.CachedAllCards
            .OfType<UiReactOnMoveCard>()
            .Where(card => card.IsClickable)
            .ToList(); // Кешируем, чтобы не перебирать коллекцию дважды

        Debug.Log("React cards: " + reactCards.Count);

        if (reactCards.Count > 0)
        {
            //todo delete
            //foreach (var card in reactCards)
            //{
            //    card.SetUpReactCard(this);
            //}

            UiManager.Instance.SetUpThisOnEnemyTurn(fieldHero);
            UiManager.Instance.SetUpReactButton(heroData);

            CameraController.Instance.MoveCameraToTarget(fieldHero.transform, 1f);

            //todo delete
            //foreach (var card in reactCards)
            //{
            //    card.ClearStepAction();
            //}
        }
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.Unsubscribe("HeroReacted", FindNewAttackCell);
        base.OnNetworkDespawn();
    }
}
