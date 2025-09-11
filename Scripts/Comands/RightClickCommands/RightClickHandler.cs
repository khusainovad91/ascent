using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickHandler : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private List<CommandType> defaultCommandTypes;
    public List<RightClickCommand> possibleCommands;
    [SerializeField]
    CommandTextGroup commandTextGroup;
    FieldObject fieldObject;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        possibleCommands = new List<RightClickCommand>();

        Debug.Log("RCH TEST" + this.gameObject.name);
        fieldObject = GetComponent<FieldObject>();
        var staticObj = GetComponent<FieldStaticObject>();
        Debug.Log($"FieldObject: {fieldObject}");
        Debug.Log($"FieldStaticObject: {staticObj}");
    }

    [Rpc(SendTo.Server)]
    public void AddNewCommandRpc(CommandType commandType)
    {
        //Debug.Log("AddNewCommand called: " + command.GetType().Name + " / hash: " + command.GetHashCode());
        AddNewCommandClientRpc(commandType);
    }

    [Rpc(SendTo.Everyone)]
    private void AddNewCommandClientRpc(CommandType commandType)
    {
        var newCommand = CommandFactory.CreateCommand(commandType);
        if (!possibleCommands.Contains(newCommand))
        {
            possibleCommands.Add(newCommand);
        }
    }

    [Rpc(SendTo.Server)]
    public void RemoveCommandRpc(CommandType commandType)
    {
        RemoveCommandClientRpc(commandType);
    }

    [Rpc(SendTo.Everyone)]
    private void RemoveCommandClientRpc(CommandType commandType) {
        var commandToRemove = possibleCommands.FirstOrDefault(command => command.Type == commandType);
        if (commandToRemove != null && possibleCommands.Contains(commandToRemove))
        {
            possibleCommands.Remove(commandToRemove);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
        {
        if (HeroControllerManager.Instance.FieldHero == null) return;

        if (eventData.button != PointerEventData.InputButton.Right &&
                HeroControllerManager.Instance.FieldHero.HeroData.CurrentState != HeroState.Idle &&
                this.GetComponent<FieldObject>().isTargeted.Value == false &&
                HeroControllerManager.Instance.FieldHero.IsOwner
            ) return;

        //this.GetComponent<FieldObject>().SetIsTargetedRpc(true);
        //Высчитываем кого можно добавить на UI
        HashSet<RightClickCommand> avaiableCommands = new HashSet<RightClickCommand>();
                    
        Debug.Log("Possible commands: " + possibleCommands.Count);

        foreach (var commandType in defaultCommandTypes)
        {
            var newCommand = CommandFactory.CreateCommand(commandType);
            newCommand.SetupCommand(HeroControllerManager.Instance.FieldHero, fieldObject);
            if (newCommand.IsAwaiable())
            {
                avaiableCommands.Add(newCommand);
            }
            Debug.Log("Добавил стандартную команду:" + newCommand);
        }

        foreach (var possibleCommand in possibleCommands)
        {
            Debug.Log(possibleCommand.ToString());
            Debug.Log("Selected Hero:" + HeroControllerManager.Instance.FieldHero);
            possibleCommand.SetupCommand(HeroControllerManager.Instance.FieldHero, fieldObject);
            int i = 0;
            if (possibleCommand.IsAwaiable())
            {
                i++;
                Debug.Log("Total possible commands:" + i);
                avaiableCommands.Add(possibleCommand);
            }
        }

        //Добавляем на UI
        //&& (possibleCommands.Count > 0 || avaiableCommands.Count > 0)
        if (commandTextGroup != null ) {
            commandTextGroup.TurnOn();
            commandTextGroup.ClearCommandTextGroup();
            commandTextGroup.SetUpCommandTextGroup(avaiableCommands);
            SelectControllerManager.Instance.ChangeMode(SelectionMode.RightClickCommand);
            StartCoroutine(WaitForEscape());
        }
  
    }

    private IEnumerator WaitForEscape()
    {
        while (true) { 
            if (Input.GetKeyUp(KeyCode.Escape)) { 
                if (commandTextGroup != null)
                {
                    commandTextGroup.TurnOff();
                }

                SelectControllerManager.Instance.ChangeMode(SelectionMode.Free);
                this.GetComponent<FieldObject>().Outline.enabled = false;
                if (HeroControllerManager.Instance.FieldHero != null)
                {
                    HeroControllerManager.Instance.FieldHero.ChangeStateRpc(HeroState.Idle);
                }
                yield break;
            }

            if (SelectControllerManager.Instance.currentMode != SelectionMode.RightClickCommand)
            {
                this.GetComponent<FieldObject>().Outline.enabled = false;
                yield break;
            }

            yield return null;
        }
    }
}
