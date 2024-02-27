using UnityEngine;
using Player.View;
using Player.Motion;
using TMPro;
using UnityEngine.UI;
using Jellies;

public class IslandHeartInteractBase : Interactable
{
    [Header("Timing")]
    [SerializeField]
    private float _reactivateTime = 1.25f;

    [Header("Controlled Objects")]
    [SerializeField, Tooltip("Virtual Island Heart camera to focus for interaction.")]
    private GameObject _islandHeartVCamera;

    [Header("Feed-Island Heart interaction UI")]
    [SerializeField, Tooltip("If the Island Heart is leveling up, disable the feedButton")]
    private Button _feedButton;

    private IslandHeartLeveling heart;

    private GameObject _interactionUI;
    private GameObject _inventoryUI;
    private FirstPersonView _firstPersonView;
    private PlayerInteraction _playerInteraction;


    private bool _interacting;
    public bool Interacting
    {
        get => _interacting;
        private set
        {
            if (value != _interacting)
            {
                _interacting = value;

                // Only 1 can interact at a time, so we can determine whether
                // any are interacting like this. Need the static one, but also
                // need to know whether this specific one is interacting.
                AnyInteracting = value;
            }
        }
    }
    public static bool AnyInteracting { get; private set; }

    private void Awake()
    {
        heart = gameObject.GetComponent<IslandHeartLeveling>();

        // Temporary single player reference. These might not work later, e.g. if the inventoryUI
        // starts inactivate b/c FindWithTag wont work.
        _interactionUI = InteractionUI.Instance.gameObject;
        _inventoryUI = GameObject.FindWithTag("InventoryUI");
        _firstPersonView = FindObjectOfType<FirstPersonView>();
        _playerInteraction = _firstPersonView?.GetComponent<PlayerInteraction>();
    }

    private void Update()
    {
        if (Interacting)
        {
            OnInteractionStay();
        }
    }

    /// <summary>
    /// During the interaction duration (when _interacting == true)
    /// </summary>
    private void OnInteractionStay()
    {
        _feedButton.interactable = heart.IsMaxLevel();    // Disable Feed-button when Island Heart is max level
    }

    private void SetInteract(bool interact)
    {
        Interacting = interact;

        _islandHeartVCamera.SetActive(interact);

        Cursor.visible = interact;
        Cursor.lockState = interact ? CursorLockMode.Confined : CursorLockMode.Locked;

        if (interact)
        {
            SetUIActive(false);
        }
        else
        {
            Invoke(nameof(ReactivateUI), _reactivateTime);
        }
    }

    /// <summary>
    /// Hold variables to reactivate when Invoked, preventing visual glitches.
    /// <summary>
    private void ReactivateUI()
    {
        SetUIActive(true);
    }

    private void SetUIActive(bool active)
    {
        _interactionUI.SetActive(active);
        _firstPersonView.enabled = active;
        PlayerMovement.Instance.SetInteract(!active);
    }

    /// <summary> 
    /// Deactivate wandering and parts of player, in preparation for activating interaction variables. Prevents the
    /// Player from moving and sliding around while in interaction since a sudden disabling of movement causes constant
    /// application of velocity.
    /// <summary>
    public void InteractStart()
    {
        SetInteract(true);
    }

    /// <summary> 
    /// Reactivate wandering and parts of player, in preparation for returning to normal gameplay.
    /// <summary>
    public void InteractStop()
    {
        SetInteract(false);
    }
}
