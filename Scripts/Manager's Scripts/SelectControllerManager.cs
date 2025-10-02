using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//�� ���� ���������� ������ �����
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
            //todo ���������� �� StateMachine SelectionMode
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
            Debug.Log("������ " + newSelected);
            EventManager.Instance.TriggerEvent<MonoBehaviour>("OnSelect", newSelected);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)//(Mouse.current.rightButton.wasPressedThisFrame)
        {
            //todo triger herodata ��������, ����� �� ����� �����������
            Debug.Log("������� �����");
            EventManager.Instance.TriggerEvent("OnDenie");
        }
    }

    //����� �������� �����
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
            
            if (newSelected == null) //���� ������������ ������ � ������ ����� ��������� ���� UI ����� ����
            {
                if (IsPointerOverUI())
                {
                    return;
                }
            }

            if (SelectedHero == newSelected) //���� ��������� ������ ����� ����������� �� ������ ������ ����������� � ������� �� ������
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

    // ���������, ��������� �� ������ ��� UI
    private bool IsPointerOverUI()
    {
        return false;
        //���������
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
