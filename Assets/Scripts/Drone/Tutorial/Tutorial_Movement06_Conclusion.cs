using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;
using Player.Motion;
using Player.View;


public class Tutorial_Movement06_Conclusion : MonoBehaviour
{
    [Header("Tutorial Options")]
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;
    [SerializeField]
    private DroneScanning _droneScanning;

    [Header("Tutorial Options")]
    [Tooltip("This is how long (in seconds) the planet drone image is displayed.")]
    private float _planetImageDuration = 3f;
    [Tooltip("This is how long (in seconds) the analyzing drone image is displayed.")]
    private float _analyzingImageDuration = 3f;

    [Header("Drone Area Scan Settings")]
    [Tooltip("This is the size of the area scan's arc (in degrees).")]
    [SerializeField, Min(0)]
    private int _AreaScanArcSize = 360;
    [Tooltip("This is how long (in seconds) that the drone's area scan will take.")]
    [SerializeField, Min(0f)]
    private float _AreaScanDuration = 10f;
    [Tooltip("This is the radius of the circle the drone's laser will trace on the ground as it does the area scan.")]
    [SerializeField, Min(1f)]
    private float _AreaScanRadius = 10f;
    [Tooltip("This is the y-position at which the bottom of the laser will be as the drone does the area scan.")]
    [SerializeField]
    private float _BottomOfLaserAltitude = 0f;
    [Tooltip("This sets how long (in seconds) the drone's scan success visual is displayed for.")]
    [SerializeField, Min(0)]
    private float _areaScanCompleteDelay = 1.5f;


    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_Planet;
    [SerializeField]
    private Sprite _sprite_ScanningArea;
    [SerializeField]
    private Sprite _sprite_Success;
    [SerializeField]
    private Sprite _sprite_Analyzing;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;
    [SerializeField]
    private FSMParameter _finishedMovementTutorial;

    [Header("Tutorial State Machine Events - Stage 04 - Mobility Test")]
    [SerializeField]
    private GameEventScriptableObject _Enter;
    [SerializeField]
    private GameEventScriptableObject _Update;
    [SerializeField]
    private GameEventScriptableObject _Exit;



    private FiniteStateMachineInstance _stateMachineInstance;

    private bool _droneAreaScanComplete;
    private bool _conclusionFinished;

    private GameEventResponses _gameEventResponses = new();




    private void Awake()
    {
        // Get the PlayerInputComponent.
        _stateMachineInstance = _drone.GetActionStateMachineInstance();

        _gameEventResponses.SetResponses(
            // Player controls events
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial)
            );

    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();


    private void EnterTutorial()
    {
       
        _droneAreaScanComplete = false;
        _conclusionFinished = false;

        CameraSystem.SwitchToTutorialCamera();

        StartCoroutine(DoTutorialConclusion());
    }

    private void UpdateTutorial()
    {
        if (_droneAreaScanComplete && _conclusionFinished)
        {
            // Trigger the tutorial to end.
            _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);
            _stateMachineInstance.SetBool(_finishedMovementTutorial, true);
        }        
    }

    private void ExitTutorial()
    {
        // Switch back to the normal player cam.
        CameraSystem.SwitchToFirstPersonCamera();

        // Re-enable all the player controls.
        FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs--;
        PlayerMovement.Instance.NumberOfReasonsToIgnoreJumpInputs--;
        PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs--;

        Inventory.Instance.NumberOfReasonsToIgnoreInputs--;
        InteractionUI.Instance.NumberOfReasonsToBeInactive--;
        PlayerInteraction.Instance.NumberOfReasonsToIgnoreInputs--;

        if (!PauseManagement.IsPaused && (FirstPersonView.Instance.IgnoreInputs 
            || PlayerMovement.Instance.IgnoreJumpInputs || PlayerMovement.Instance.IgnoreWASDInputs
            || Inventory.Instance.IgnoreInputs || !InteractionUI.Instance.gameObject.activeSelf
            || PlayerInteraction.Instance.IgnoreInputs))
        {
            throw new System.Exception("At this point, player should be able to move camera, jump, use WASD, use inventory (may change once" +
                " we add inventory tutorial), and interact with things. info: "
                + "FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs: " + FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs 
                + ", PlayerMovement.Instance.NumberOfReasonsToIgnoreJumpInputs: " + PlayerMovement.Instance.NumberOfReasonsToIgnoreJumpInputs 
                + ", PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs: " + PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs 
                + ", Inventory.Instance.NumberOfReasonsToIgnoreInputs: " + Inventory.Instance.NumberOfReasonsToIgnoreInputs
                + ", InteractionUI.Instance.NumberOfReasonsToBeInactive: " + InteractionUI.Instance.NumberOfReasonsToBeInactive
                + ", InteractionUI.Instance.gameObject.activeSelf: " + InteractionUI.Instance.gameObject.activeSelf
                + ", PlayerInteraction.Instance.NumberOfReasonsToIgnoreInputs: " + PlayerInteraction.Instance.NumberOfReasonsToIgnoreInputs

                );
        }
    }

    private IEnumerator DoTutorialConclusion()
    {
        _pictogramBehaviour.ChangePictogramImage(_sprite_Planet);
        yield return new WaitForSeconds(_planetImageDuration);


        yield return StartCoroutine(DoAreaScan());


        // Show the analyzing image.
        _pictogramBehaviour.ChangePictogramImage(_sprite_Analyzing);
        yield return new WaitForSeconds(_analyzingImageDuration);

        // Disable the drone's image and flag that the conclusion has finished.
        _pictogramBehaviour.SetImageActive(false);
        _conclusionFinished = true;
    }

    private IEnumerator DoAreaScan()
    {
        _pictogramBehaviour.ChangePictogramImage(_sprite_ScanningArea);

        Vector3 dronePosition = _drone.transform.position;
        Quaternion droneRotation = _drone.transform.rotation;

        Vector3 initialRotation = droneRotation.eulerAngles;
        float elapsedTime = 0f;


        _droneScanning.ShowScanningVisual(CalculateLaserEndPoint(0f, dronePosition) + dronePosition);

        // This loop makes the drone do the area scan.
        while (elapsedTime <= _AreaScanDuration)
        {
            elapsedTime += Time.deltaTime;

            float arcScannedSoFar = (elapsedTime / _AreaScanDuration) * _AreaScanArcSize;

            float newAngle = initialRotation.y + arcScannedSoFar;
            _droneScanning.ShowScanningVisual(CalculateLaserEndPoint(newAngle, dronePosition) + dronePosition); // We add the drone position so the circle the end of the laser follows is shifted to be centered on the drone.

            Vector3 rotation = droneRotation.eulerAngles;
            rotation.y = newAngle ;
            droneRotation.eulerAngles = rotation;
            _drone.transform.rotation = droneRotation;

            yield return null;
        }

        _pictogramBehaviour.ChangePictogramImage(_sprite_Success);


        // Show the scanning success visual.
        _droneScanning.HideScanningVisual();
        _droneScanning.ShowScanningSuccessVisual(CalculateLaserEndPoint(initialRotation.y + _AreaScanArcSize, dronePosition) + dronePosition);
        yield return new WaitForSeconds(_areaScanCompleteDelay);

        // Hide the scanning success visual.
        _droneScanning.HideScanningSuccessVisual();

        // Flag that the area scan has completed.
        _droneAreaScanComplete = true;
    }

    private Vector3 CalculateLaserEndPoint(float angle, Vector3 dronePosition)
    {
        // Convert angle to radians.
        angle *= Mathf.Deg2Rad;

        // Calculate the laser's current position on the scan arc.
        Vector4 endPoint = new Vector3(_AreaScanRadius * Mathf.Sin(angle),
                                       _BottomOfLaserAltitude - dronePosition.y, // We subtract the drone's altitude because the laser position is relative to the drone's position since it is a child object. This converts the y coordinate back to global world space.
                                       _AreaScanRadius * Mathf.Cos(angle));

        return endPoint;
    }

}
