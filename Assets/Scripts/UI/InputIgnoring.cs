using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputIgnoring 
{
    public static void AddReasonToIgnoreInputs()
    {
        ChangeNumberOfReasonsToIgnoreInputs(true);
    }

    public static void RemoveReasonToIgnoreInputs()
    {
        ChangeNumberOfReasonsToIgnoreInputs(false);
    }

    public static void ChangeNumberOfReasonsToIgnoreInputs(bool incrementDontDecrement)
    {
        int change = incrementDontDecrement ? 1 : -1;
        InteractionUI.Instance.NumberOfReasonsToBeInactive += change;
        Player.View.FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs += change;
        Player.Motion.PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs += change;
        Player.Motion.PlayerMovement.Instance.NumberOfReasonsToIgnoreJumpInputs += change;
        PlayerInteraction.Instance.NumberOfReasonsToIgnoreInputs += change;
    }
}
