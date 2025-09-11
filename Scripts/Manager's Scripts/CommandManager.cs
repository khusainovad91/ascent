using ExtensionMethods;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class CommandManager : NetworkBehaviour
{
    public static CommandManager Instance;
    private Queue<ICommand> commandQueue = new Queue<ICommand>();
    //private Stack<ICommand> commandHistory = new Stack<ICommand>();
    private ICommand currentCommand;

    public void AddCommand(ICommand command)
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame callerFrame = stackTrace.GetFrame(1); // 0 — текущий, 1 — вызывающий
        var method = callerFrame.GetMethod();
        UnityEngine.Debug.Log("Команда " + command);
        UnityEngine.Debug.Log("Statcktrace " + stackTrace.ToString());
        commandQueue.Enqueue(command);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    void Update()
    {
        if (currentCommand != null)
        {
            // Обновление текущей команды
            // if (currentCommand is MoveHeroCommand moveCommand)
            if (!currentCommand.IsExecuted)
            {
                currentCommand.Execute();
            }

            // Проверка завершения команды
            if (currentCommand.IsExecuted)
            {
                currentCommand = null;
            }
        }
        else if (commandQueue.Count > 0)
        {
            // Берем следующую команду из очереди
            currentCommand = commandQueue.Dequeue();
        }
    }

    //todo а может и не todo
    //public void UndoLastCommand()
    //{
    //    if (commandHistory.Count > 0)
    //    {
    //        var command = commandHistory.Pop();
    //        command.Undo();
    //    }
    //    else
    //    {
    //        Debug.Log("No commands to undo.");
    //    }
    //}


    //-------------------------------------------------------------------
    //COMMAND RPC LIST
    //-------------------------------------------------------------------

    [Rpc(SendTo.Server)]
    public void EnqeueueMoveHeroCommandRpc(NetworkObjectReference heroRef, Coords[] coords)
    {
        heroRef.TryGet(out NetworkObject no);
        var hero = no.GetComponent<FieldHero>();
        //var path = vector3Ints.ToCellList();
        var path = coords.ToVector3Int().ToCellList();
        var moveCommand = new MoveHeroCommand(hero, path);
        AddCommand(moveCommand);
    }

}
