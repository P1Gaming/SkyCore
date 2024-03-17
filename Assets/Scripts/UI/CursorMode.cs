using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorMode
{
    private static int _numberOfReasonsForFreeCursor = 0;

    // When quit to main menu, need to set _numberOfReasonsForFreeCursor to 0.
    // Quitting to main menu isn't implemented right now.

    public static void Initialize()
    {
        CheckCursorMode();
    }

    public static void AddReasonForFreeCursor()
    {
        ChangeNumberOfReasonsForFreeCursor(true);
    }

    public static void RemoveReasonForFreeCursor()
    {
        ChangeNumberOfReasonsForFreeCursor(false);
    }

    public static void ChangeNumberOfReasonsForFreeCursor(bool incrementDontDecrement)
    {
        int change = incrementDontDecrement ? 1 : -1;
        _numberOfReasonsForFreeCursor += change;
        //Debug.Log("# reasons for free cursor: " + _numberOfReasonsForFreeCursor);
        if (_numberOfReasonsForFreeCursor < 0)
        {
            throw new System.Exception("In CursorMode, _numberOfReasonsForFreeCursor < 0: " + _numberOfReasonsForFreeCursor);
        }
        CheckCursorMode();
    }

    private static void CheckCursorMode()
    {
        bool unlocked = _numberOfReasonsForFreeCursor > 0;
        Cursor.visible = unlocked;
        Cursor.lockState = unlocked ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
