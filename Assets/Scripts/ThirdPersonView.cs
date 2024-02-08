using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonView : MonoBehaviour
{
    // Private fields
    private Vector2 LookDirection = new Vector2(0f, 0f);
    private float xAngle;
    private float yAngle;

    // Inspector Fields
    [SerializeField]
    Transform CamTarget;    // Cinemachine camera target.
    [SerializeField]
    float CamSensitive;     // Camera movement sensitivity.
    [SerializeField]
    float AngleXMin, AngleXMax; // Minimum and maximum x angles.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onEnable()
    {
        // RegisterEventHandlers(input);
    }

    private void RegisterEventHandlers()
    {
        // Input: PlayerInput input
    }

    private void onDisable()
    {
        // UnrgisterEventHandlers(input);
    }

    private void UnrgisterEventHandlers()
    {
        // Input: PlayerInput input
    }

    private void OnLook()
    {
        // Input: InputAction.CallbackContext context
    }

    private void LateUpdate()
    {
        TurnCamera();
    }

    private float ConfineAngle(float angle)
    {
        return ((angle + 540) % 360) - 180;
    }

    private void TurnCamera()
    {
        // Update current x and y angles (z = 0), -180 <> 180
        // Apply angles to CamTarget
        xAngle = ConfineAngle(xAngle);
        xAngle = Mathf.Clamp(xAngle, AngleXMin, AngleXMax);
    }
}
