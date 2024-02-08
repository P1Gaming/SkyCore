using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this when creating new commands for the DebugController class. 
/// </summary>
public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;

    public string commandId { get { return _commandId; } }
    public string commandDescription { get { return _commandDescription; } }
    public string commandFormat { get { return _commandFormat; } }

    public DebugCommandBase(string id, string description, string format)
    {
        _commandId = id;
        _commandDescription = description;
        _commandFormat = format;
    }
}

/// <summary>
/// Use this when creating basic new commands for the DebugController class. EX: Respawn
/// </summary>
public class DebugCommand : DebugCommandBase
{
    private Action _command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this._command = command;
    }

    public void Invoke()
    {
        _command.Invoke();
    }
}

/// <summary>
/// Use this when creating new commands for the DebugController class that expect a value to be passed to them. EX: give berry 20 
/// </summary>
public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> _command;

    public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
    {
        this._command = command;
    }

    public void Invoke(T value)
    {
        _command.Invoke(value);
    }
}
