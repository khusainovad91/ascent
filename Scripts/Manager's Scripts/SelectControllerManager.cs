using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//По сути контроллер левого клика
public class SelectControllerManager : PersistentSingleton<SelectControllerManager>
{
    [SerializeField]
    private Camera _currentCamera;
    [NonSerialized]
    public FieldHero SelectedHero;
    private bool selectEnabled = true;
    public SelectionMode currentMode { get; private set; }

    // Update is called once per frame

    private void Start()
    {
        currentMode = SelectionMode.Free;
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroData>("OnHeroStateChange", HandleStateChange);
    }

    private void OnDisable()
    {
        EventManager.Instance?.Unsubscribe<HeroData>("OnHeroStateChange", HandleStateChange);
    }

    private void HandleStateChange(HeroData heroData)
    {
        switch (heroData.CurrentState)
        {
            //todo переделать на StateMachine SelectionMode
            case HeroState.RollingDice:
                selectEnabled = false;
                break;
            case HeroState.Idle:
            case HeroState.Attacking:
                selectEnabled = true;
                break;
        }
    }

    void Update()
    {
        Debug.Log("currentMode" + currentMode);
        switch (currentMode)
        {
            case SelectionMode.Free:
                FreeMode();
                break;
            case SelectionMode.Hero:
                SelectObjectMode<FieldHero>();
                break;
            case SelectionMode.Enemy:
                SelectObjectMode<EnemyObject>();
                break;
            case SelectionMode.Dice:
                SelectObjectMode<DiceData>();
                break;
            case SelectionMode.Brick:
                SelectObjectMode<BrickData>();
                break;
        }
    }

    private void SelectObjectMode<T>() where T : MonoBehaviour
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverUI())
            {
                return;
            }

            var newSelected = HandleObjectSelection<T>();
            Debug.Log("выбрал " + newSelected);
            EventManager.Instance.TriggerEvent<MonoBehaviour>("OnSelect", newSelected);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)//(Mouse.current.rightButton.wasPressedThisFrame)
        {
            //todo triger herodata наверное, чтобы не увсех отключалось
            Debug.Log("Отменил выбор");
            EventManager.Instance.TriggerEvent("OnDenie");
        }
    }

    //можно выбирать героя
    private void FreeMode()
    {
        if (!selectEnabled || GameManager.Instance.StateOfGame.Value != GameState.HeroTurn)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && !GameManager.Instance.IsAnythingIsMovingNow)
        {
            if (IsPointerOverUI())
            {
                return;
            }

            var newSelected = HandleObjectSelection<FieldObject>();
            
            if (newSelected == null) //если пользователь тыкнул в пустое место отключаем весь UI кроме топа
            {
                if (IsPointerOverUI())
                {
                    return;
                }
            }

            if (SelectedHero == newSelected) //если выбранный объект равен предыдущему то больше ничего происходить в апдейте не должно
            {
                return;
            }

            else if (newSelected.CompareTag("Hero"))
            {
                EventManager.Instance.TriggerEvent("event_UnselectHero");
                EventManager.Instance.TriggerEvent("event_SelectHero", newSelected.GetComponent<FieldHero>());
            }

            //todo 4 NPC && other objects

        }
    }

    private T HandleObjectSelection<T>() where T: MonoBehaviour
    {
        var ray = _currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponentInParent<T>();
        }

        return null;
    }

    // Проверяет, находится ли курсор над UI
    private bool IsPointerOverUI()
    {
        return false;
        //Продумать
        //return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public void ChangeMode(SelectionMode newMode)
    {
        Debug.Log("CurrentSelectionMode: " + currentMode);
       // if (currentMode == newMode) return;
        currentMode = newMode;
        HeroControllerManager.Instance.ClearUiOnFloor();
    }
}

public enum SelectionMode
{
    Free,
    Hero,
    Enemy,
    Dice,
    AttackCard,
    Brick,
    Npc,
    Object,
    RightClickCommand
}
