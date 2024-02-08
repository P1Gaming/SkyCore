using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Responsible for the debug console. Commands can be added via the command list. 
/// Essentially will be used for debuging cheats/data.
/// </summary>
public class DebugController : MonoBehaviour
{
    private bool _displayConsole = false;
    private bool _showHelp = false;

    private string _input = "";
    private List<string> _output = new List<string>();

    public static DebugCommand<string> DebugLog;

    public static List<object> commandList;

    private string _previousCommand = "";

    private List<string> _previousCommands = new List<string>();
    private int _position = 0;

    public static DebugController Instance { get; private set; }

    private PlayerActions _pActions;
    // Stores the current scroll position
    private Vector2 _scroll;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            UnityEngine.Debug.LogError("TOO MANY DEBUG CONTROLLERS!");
        }

        _pActions = new PlayerActions();
        _pActions.Enable();

        DebugCommand help = new DebugCommand("help", "Shows Commands", "help", () =>
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.commandFormat} - {command.commandDescription}";

                NewOutput(label);
            }
        });

        DebugCommand clear = new DebugCommand("clear", "Clears the console of all logs.", "clear", () =>
        {
            _output.Clear();
            NewOutput("TYPE '" + help.commandId + "' TO SEE A LIST OF COMMANDS!");
        });

        commandList = new List<object>
        {
            help,
            clear
        };

        NewOutput("TYPE '" + help.commandId + "' TO SEE A LIST OF COMMANDS!");
    }

    private void OnGUI()
    {
        if (!_displayConsole) { return; }

        float y = 0;
        float historyLength = 200;
        float lineHeight = 20;
        float inputFieldHeight = 10;
        float verticalSpacing = 30;
        float padding = 5;


        GUI.Box(new Rect(0, y, Screen.width, historyLength), "");

        Rect viewport = new Rect(0, 0, Screen.width - verticalSpacing, lineHeight * this._output.Count);

        _scroll = GUI.BeginScrollView(
            new Rect(0, y + padding, Screen.width, historyLength - inputFieldHeight),
            _scroll,
            viewport,
            false,
            true);

        for (int i = 0; i < _output.Count; i++)
        {
            Rect labelRect = new Rect(5, lineHeight * i, viewport.width - historyLength, lineHeight);

            GUI.Label(labelRect, _output[i]);
        }

        GUI.EndScrollView();

        y += historyLength;

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = Color.black;
        _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);
        
    }

    /// <summary>
    /// Execute to switch between showing and hiding the console 
    /// </summary>
    /// <param name="input"></param>
    public void DebugToggle(InputAction.CallbackContext context)
    {
        _displayConsole = !_displayConsole;
        this._input = "";

        if (_displayConsole)
        {
            UnlockMouse();
        }
        else
        {
            LockMouse();
        }
    }

    /// <summary>
    /// To be called when return is pressed and if the console is open.
    /// </summary>
    /// <param name="context"></param>
    public void Return(InputAction.CallbackContext context)
    {
        float lineHeight = 20;

        if (_displayConsole && context.performed)
        {
            HandleInput();
            if (this._input != "") { _previousCommands.Add(this._input); }
            this._position = _previousCommands.Count;
            this._input = "";
            _scroll = new Vector2(0, lineHeight * this._output.Count);
        }
    }

    public void Up(InputAction.CallbackContext context)
    {
        if (_displayConsole && _position != 0 && context.performed)
        {
            _position--;
            this._input = _previousCommands[_position];
        }
    }


    public void Down(InputAction.CallbackContext context)
    {
        if (_displayConsole && _position != _previousCommands.Count && context.performed)
        {
            _position++;
            this._input = (_position != _previousCommands.Count) ? _previousCommands[_position] : string.Empty;
        }
    }

    /// <summary>
    /// Lock the mouse to the screen.
    /// </summary>
    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Unlock the mouse from the screen 
    /// </summary>
    private void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Print new output to console 
    /// </summary>
    /// <param name="output">What to print to console.</param>
    private void NewOutput(string output)
    {
        this._output.Add(output);
    }

    /// <summary>
    /// Execute any commands that may be in the input field
    /// </summary>
    private void HandleInput()
    {
        string[] inputValues = _input.Split(' ');

        bool commandFound = false;

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (_input.Contains(commandBase.commandId))
            {
                if ((commandList[i] as DebugCommand) != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if ((commandList[i] as DebugCommand<string>) != null)
                {
                    string stringBuild = "";

                    for (int p = 1; p < inputValues.Length; p++)
                    {
                        stringBuild += inputValues[p];
                    }

                    if (stringBuild == "")
                    {
                        NewOutput((commandList[i] as DebugCommand<string>).commandFormat);
                    }
                    else
                    {
                        (commandList[i] as DebugCommand<string>).Invoke(stringBuild);
                    }
                }
                else if ((commandList[i] as DebugCommand<int>) != null)
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(inputValues[1]));
                }
                else if ((commandList[i] as DebugCommand<float>) != null)
                {
                    (commandList[i] as DebugCommand<float>).Invoke(float.Parse(inputValues[1]));
                }
                commandFound = true;
            }
        }

        if (!commandFound)
        {
            _output.Add("COMMAND: '" + inputValues[0] + "' DOES NOT EXIST");
        }
    }
}

