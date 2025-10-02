using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class StepAction : IAction
{
    private EnemyObject _enemyObject;
    private Cell _cellToGo;
    public bool IsExecuted { get; set; } = false;
    public float Weight { get; set; } = 10;

    public StepAction(EnemyObject enemyObject, Cell cellToGo)
    {
        _enemyObject = enemyObject;
        _cellToGo = cellToGo;
    }

    public IEnumerator Execute()
    {
        _enemyObject.Stats.ChangeMovementPointsClientRpc(-1);
        yield return new WaitForSeconds(0.5f);
        _enemyObject.ChangeStateRpc(EnemyState.Moving);
        Vector3 targetPosition = _cellToGo.coords.CenterOfCell();

        _enemyObject.LookOn(targetPosition);
        //_enemyObject.transform.LookAt(targetPosition);

        float step = _enemyObject.Stats.Speed * Time.deltaTime;

        CameraController.Instance.FollowTartgetRpc(_enemyObject.GetNetworkObjectReference());

        while (Vector3.Distance(_enemyObject.transform.position, targetPosition) > 0.01f)
        {
            _enemyObject.transform.position = Vector3.MoveTowards(_enemyObject.transform.position, targetPosition, step);
            yield return null; // ∆дем следующий кадр
        }

        BoardManager.Instance.ClearCellServerRpc(_enemyObject.CurrentCell.coords);
        //TODO опции дл€ двух и более клеток
        _enemyObject.CurrentCell = null;
        //_enemyObject.OcupiedCells.Clear();
        //_enemyObject.OcupiedCells.Remove(_enemyObject.CurrentCell);
        BoardManager.Instance.OcupieCellServerRpc(_cellToGo.coords, _enemyObject.GetNetworkObjectReference());

        IsExecuted = true;
        _enemyObject.ChangeStateRpc(EnemyState.Idle);
        CameraController.Instance.SetAutomaticMoveRpc(false);

        _enemyObject.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Move);
        //wait until hero do something
        yield return HandleHeroReaction();
        yield return new WaitForNextFrameUnit();
    }

    private IEnumerator HandleHeroReaction()
    {
        List<FieldHero> heroes = UtilClass.FindObjectsInRange<FieldHero>(_enemyObject.CurrentCell, 1);

        foreach (FieldHero hero in heroes)
        {
            if (hero.HeroData.CurrentState == HeroState.Fainted || hero.HeroData.CurrentState == HeroState.Dead)
            {
                continue;
            }

            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { hero.OwnerClientId }
                }
            };
            var reactCards = hero.HeroData.InventoryData.CachedAllCards
                .OfType<UiReactOnMoveCard>()
                .Where(card => card.IsClickable)
                .ToList();
            if (reactCards.Count > 0)
            {
                hero.ChangeStateRpc(HeroState.Reacting);
                _enemyObject.GetComponent<EnemyController>().PassHeroReactionMessageToMessageClientRpc(hero.GetNetworkObjectReference(),
                clientRpcParams);
            }
            //SendReactOnMoveToClient(hero);
        }

        //Waiting 4 every hero to react;
        yield return new WaitUntil(() => {
            bool result = true;
            foreach (FieldHero hero in heroes) {
                if (hero.HeroData.CurrentState == HeroState.Reacting)
                {
                    result = false;
                }
            }
            return result;
        });
    }

    //private IEnumerator SendReactOnMoveToClient(FieldHero hero)
    //{
    //    Debug.Log($"{hero.name} is owner: {hero.IsOwner}");
    //    //if (!hero.IsOwner) continue;

    //    var heroData = hero.HeroData;
    //    var reactCards = heroData.InventoryData.CachedAllCards
    //        .OfType<UiReactOnMoveCard>()
    //        .Where(card => card.IsClickable)
    //        .ToList(); //  ешируем, чтобы не перебирать коллекцию дважды

    //    if (reactCards.Count > 0)
    //    {
    //        foreach (var card in reactCards)
    //        {
    //            card.SetUpReactCard(this);
    //        }
    //        UiManager.Instance.SetUpThisOnEnemyTurn(hero);
    //        UiManager.Instance.SetUpReactButton(heroData);

    //        heroData.ChangeState(HeroState.Reacting);
    //        CameraController.Instance.MoveCameraToTarget(hero.transform, 1f);

    //        yield return new WaitUntil(() => heroData.CurrentState != HeroState.Reacting);

    //        foreach (var card in reactCards)
    //        {
    //            card.ClearStepAction();
    //        }
    //    }
    //}

}
