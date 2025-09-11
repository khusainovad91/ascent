using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandFactory
{
    private static Dictionary<CommandType, Func<RightClickCommand>> _creators = new() {
            { CommandType.Revive, () => new ReviveCommand() },
            { CommandType.Throw, () => new ThrowCommand() },
            { CommandType.OpenDoor, () => new OpenDoorCommand() },
            { CommandType.CloseDoor, () => new CloseDoorCommand() }
    };

    public static RightClickCommand CreateCommand(CommandType commandType)
    {
        if (_creators.TryGetValue(commandType, out var creator)) return creator();

        throw new Exception($"Неизвестная команда: {commandType}");
    }
}

public enum CommandType
{
    Revive,
    Throw,
    OpenDoor,
    CloseDoor
}