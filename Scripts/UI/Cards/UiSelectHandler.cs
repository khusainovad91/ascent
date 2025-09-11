using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSelectHandler: Singleton<UiSelectHandler>
{
    public FieldHero SelectedHero { get; private set; }
    public EnemyObject SelectedEnemy { get; private set; }
    public BrickData SelectedBrick { get; private set; }
    public Cell SelectedCell { get; private set; }
    public UiCardAttack SelectedWeaponCard { get; private set; }
    public bool Denied = false;
    public HeroData Hero;

    protected void OnEnable()
    {
        EventManager.Instance.Subscribe<MonoBehaviour>("OnSelect", SetSelected);
        EventManager.Instance.Subscribe("OnDenie", SetDenie);
    }

    protected void OnDisable()
    {
        EventManager.Instance.Unsubscribe<MonoBehaviour>("OnSelect", SetSelected);
        EventManager.Instance.Unsubscribe("OnDenie", SetDenie);
    }


    //-----------------------------------------------------------------------
    //EX ACTIVE CARD CODE
    //-----------------------------------------------------------------------
    private void SetDenie()
    {
        Denied = true;
    }

    private void SetSelected(MonoBehaviour _object)
    {
        if (_object is FieldHero fieldHero)
        {
            SelectedHero = fieldHero;
        }
        if (_object is EnemyObject enemyObject)
        {
            SelectedEnemy = enemyObject;
        }
        if (_object is BrickData brickData)
        {
            SelectedBrick = brickData;
        }
        if (_object is UiCardAttack uiCardAttack)
        {
            SelectedWeaponCard = uiCardAttack;
        }
    }



    public IEnumerator SelectCell(List<Cell> cellsInRange)
    {
        SelectedBrick = null;
        Denied = false;

        SelectControllerManager.Instance.ChangeMode(SelectionMode.Brick);

        var floorNumberPrefab = HeroControllerManager.Instance.FloorNumberPrefab;
        List<BrickData> floorBricks = new List<BrickData>();

        //var floorBricksPapa = new GameObject();
        foreach (var cell in cellsInRange)
        {
            var brick = Instantiate(
                floorNumberPrefab,
                cell.coords.CenterOfCell() + new Vector3(0.0f, Constants.LIL_ABOVE_FLOOR, 0.0f),
                floorNumberPrefab.transform.rotation
            //floorBricksPapa.transform
            ).AddComponent<BrickData>();
            brick.GetComponent<Track>().FaceOut(Hero.FieldHero.gameObject);
            brick.GetComponent<BoxCollider>().enabled = true;

            brick.cell = cell;
            floorBricks.Add(brick);
        }

        if (floorBricks.Count == 0)
        {
            EndOfChoiceWithDestroy(floorBricks);
            RestoreStates();
            yield break;
        }

        while ((SelectedBrick == null || !floorBricks.Contains(SelectedBrick)) && !Denied)
        {
            yield return null;
        }

        if (Denied)
        {
            EndOfChoiceWithDestroy(floorBricks);
            RestoreStates();
            yield break;
        }

        SelectedCell = SelectedBrick.cell;
        Debug.Log("SelectedCell " + SelectedCell);
        EndOfChoiceWithDestroy(floorBricks);
    }

    public IEnumerator SelectHero(List<FieldHero> heroesInRange)
    {
        SelectedHero = null;
        Denied = false;

        Hero.IsMakingChoice = true;
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Hero);

        Debug.Log("Герои в округе: " + heroesInRange.Count);

        while ((SelectedHero == null || !heroesInRange.Contains(SelectedHero)) && !Denied)
        {
            yield return null; // ждем следующего кадра
        }

        if (Denied)
        {
            EndOfMakingChoice(heroesInRange);
            RestoreStates();
            yield break; // если отказ, выходим из корутины
        }
    }

    public IEnumerator SelectWeapon()
    {
        SelectedWeaponCard = null;
        Denied = false;

        Hero.IsMakingChoice = true;
        SelectControllerManager.Instance.ChangeMode(SelectionMode.AttackCard);

        while (SelectedWeaponCard == null && !Denied)
        {
            yield return null;
        }

        if (Denied)
        {
            SelectWeapon();
        }

        Hero.IsMakingChoice = false;

        HeroAttackCommand hac = new HeroAttackCommand(Hero.FieldHero, SelectedWeaponCard.WeaponItem, SelectedEnemy, UtilClass.CalulcateDistance(Hero.FieldHero.CurrentCell, SelectedEnemy.CurrentCell));
        yield return hac.SetUp();

        EndOfMakingChoice();
    }

    public IEnumerator SelectEnemy(List<EnemyObject> enemiesInRange)
    {

        UtilClass.ShowEnemiesInRange(enemiesInRange);

        SelectControllerManager.Instance.ChangeMode(SelectionMode.Enemy);
        SelectedEnemy = null;
        Denied = false;

        if (enemiesInRange.IsNullOrEmpty())
        {
            Debug.Log("Враги для выбора отсутствуют");
            EndOfMakingChoice(enemiesInRange);
            RestoreStates();
            yield break;
        }

        while ((SelectedEnemy == null || !enemiesInRange.Contains(SelectedEnemy)) && !Denied)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Denied = true;
                break;
            }
            Debug.Log("Ждем следующего кадра");
            yield return null; // ждем следующего кадра
        }

        if (Denied)
        {
            Debug.Log("Выбор был отменен");
            EndOfMakingChoice(enemiesInRange);
            RestoreStates();
            yield break; // если отказ, выходим из корутины
        }

        Debug.Log("SelectedEnemy  " + SelectedEnemy);
        SelectedEnemy.SetIsTargetedRpc(true);
        EndOfMakingChoice(enemiesInRange);
    }

    /// <summary>
    /// Searching and outline objects
    /// </summary>
    /// <typeparam name="T">Must be children of FieldObject</typeparam>
    /// <param name="targetCell">Cell, to search around</param>
    /// <param name="range">Search range</param>
    /// <param name="exception">Exception, if need to excpet FieldObject</param>
    /// <returns></returns>
    public List<T> OutlineObjectsInRange<T>(Cell targetCell, int range, T exception) where T : FieldObject
    {
        var objectsInRange = UtilClass.FindObjectsInRange<T>(targetCell, range);
        objectsInRange.Remove(exception);
        foreach (var fieldHero in objectsInRange)
        {
            fieldHero.Outline.OutlineColor = Color.white;
            fieldHero.Outline.enabled = true;
        }

        if (objectsInRange.Count == 0)
        {
            return null;
        }

        return objectsInRange;
    }

    public List<T> OutlineObjectsInRange<T>(Cell targetCell, int range) where T : FieldObject
    {
        var objectsInRange = UtilClass.FindObjectsInRange<T>(targetCell, range);
        foreach (var fieldHero in objectsInRange)
        {
            fieldHero.Outline.OutlineColor = Color.white;
            fieldHero.Outline.enabled = true;
        }

        if (objectsInRange.Count == 0)
        {
            return null;
        }

        return objectsInRange;
    }

    public void EndOfMakingChoice<T>(List<T> objects) where T : FieldObject
    {
        if (!objects.IsNullOrEmpty())
        {
            foreach (var item in objects)
            {
                item.Outline.OutlineColor = item.defaultColor;
                item.Outline.enabled = false;
            }
        }

        //По идее переключение isMakingChoice тут тоже должно быть
        EndOfMakingChoice();
    }

    public void EndOfMakingChoice()
    {
        Hero.IsMakingChoice = false;
        if (SelectedEnemy != null)
        {
            SelectedEnemy.SetIsTargetedRpc(false);
        }
    }

    public void EndOfChoiceWithDestroy<T>(List<T> goToDestroy) where T : MonoBehaviour
    {
        if (goToDestroy != null)
        {
            foreach (var item in goToDestroy)
            {
                Destroy(item.gameObject);
            }
        }

        EndOfMakingChoice();
    }

    public void RestoreStates()
    {
        if (Hero.CurrentState != HeroState.Reacting)
        {
            SelectControllerManager.Instance.ChangeMode(SelectionMode.Free);
            Hero.ChangeState(HeroState.Idle);
        }
    }

    public void ClearSelected()
    {
        SelectedBrick = null;
        SelectedEnemy = null;
        SelectedHero = null;
        SelectedCell = null;
    }
}
