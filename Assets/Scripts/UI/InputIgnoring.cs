using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputIgnoring 
{
    public static void ChangeNumberOfReasonsToIgnoreInputsForMovementAndInteractionThings(bool incrementDontDecrement)
    {
        ChangeNumberOfReasonsToIgnoreInputsForMovementThings(incrementDontDecrement);
        ChangeNumberOfReasonsToIgnoreInputsForInteractionThings(incrementDontDecrement);
    }

    public static void ChangeNumberOfReasonsToIgnoreInputsForMovementThings(bool incrementDontDecrement)
    {
        int change = incrementDontDecrement ? 1 : -1;
        Player.View.FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs += change;
        Player.Motion.PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs += change;
        Player.Motion.PlayerMovement.Instance.NumberOfReasonsToIgnoreJumpInputs += change;
    }

    public static void ChangeNumberOfReasonsToIgnoreInputsForInteractionThings(bool incrementDontDecrement)
    {
        int change = incrementDontDecrement ? 1 : -1;
        InteractionUI.Instance.NumberOfReasonsToBeInactive += change;
        PlayerInteraction.Instance.NumberOfReasonsToIgnoreInputs += change;
    }
}
