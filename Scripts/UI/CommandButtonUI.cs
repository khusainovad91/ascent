using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CommandButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RightClickCommand _rcCommand;
    [SerializeField]
    private TextMeshProUGUI commandText;
    private CommandTextGroup _commandTextGroup;

    public void SetCommandAndVlg(RightClickCommand command, CommandTextGroup commandTextGroup) {
        Debug.Log("SetCommandAndVlg: " + command.GetType().Name + " / hash: " + command.GetHashCode());
        _commandTextGroup = commandTextGroup;
        this._rcCommand = command;
        commandText.text = _rcCommand.GetCommandText();
    }

    private void OnMouseExit()
    {
        RemoveTargets();
    }

    private void RemoveTargets()
    {
        switch (_rcCommand)
        {
            case RightClickCommandSelectEnemy _rcSelectEnemy:
                {
                    foreach (var posTarget in _rcSelectEnemy.GetTargets())
                    {
                        posTarget.Outline.enabled = false;
                    }
                    break;
                }
            case RightClickCommandNoSelect _rcNoSelectEnemy:
                break;
            default:
                {
                    throw new NotImplementedException($"Тип {_rcCommand.GetType()} не поддерживается.");
                }
        }
    }

    private void OnDestroy()
    {
        RemoveTargets();
    }

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => {
            StartCoroutine(OnClick());
        });
    }

    private IEnumerator OnClick()
    {
        _commandTextGroup.TurnOff();
        if (_rcCommand.Hero.HeroData.Stats.ActionsAmount < _rcCommand.AmountOfActions()) yield break;

        switch (_rcCommand)
        {
            case RightClickCommandNoSelect rcCommand:
                {
                    CommandManager.Instance.AddCommand(_rcCommand);
                    break;
                }
            case RightClickCommandSelectEnemy _rcSelectEnemy:
                {
                    var targets = _rcSelectEnemy.GetTargets();
                    if (targets != null && targets.Count > 0)
                    {
                        yield return SetEnemyForCommand(targets);

                    }
                    break;
                }
            default:
                {
                    throw new NotImplementedException($"No type of {_rcCommand.GetType()}");
                }
            }

        _commandTextGroup.ClearCommandTextGroup();

        //todo delete
        //if(_rcCommand.IsExecuted)
        //{
        //    _rcCommand.Hero.HeroData.Stats.ChangeActionsAmountRpc(-_rcCommand.AmountOfActions());
        //}
    }

    private IEnumerator SetEnemyForCommand(List<EnemyObject> targets)
    {
        UtilClass.ShowEnemiesInRange(targets);

        yield return UiSelectHandler.Instance.SelectEnemy(targets);

        if (!UiSelectHandler.Instance.Denied)
        {
            CommandManager.Instance.AddCommand(_rcCommand);
        }

        UiSelectHandler.Instance.EndOfMakingChoice<EnemyObject>(targets);
        UiSelectHandler.Instance.RestoreStates();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (_rcCommand)
        {
            case RightClickCommandSelectEnemy _rcSelectEnemy:
                {
                    Debug.Log("Количество целей:" + _rcSelectEnemy.GetTargets().Count);
                    foreach (var possibleTarget in _rcSelectEnemy.GetTargets())
                    {
                        possibleTarget.Outline.enabled = true;
                    }
                    break;
                }
            case RightClickCommandNoSelect _rcNoSelect:
                break;
            default:
                {
                    throw new NotImplementedException($"Тип {_rcCommand.GetType()} не поддерживается.");
                }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free)
        {
            return;
        }
        RemoveTargets();
    }
}
