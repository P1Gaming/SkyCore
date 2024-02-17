using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows images of things scanned by the drone.
/// </summary>
public class ShowNewlyScannedThing : MonoBehaviour
{
    [System.Serializable]
    public class JellyTypeToImage
    {
        public Jellies.JellyType jellyType;
        public Sprite sprite;
    }

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private GameEventScriptableObject _scannedItemEvent;
    [SerializeField]
    private GameEventScriptableObject _scannedJellyEvent;
    [SerializeField]
    private JellyTypeToImage[] _jellyImages;

    private const string _showSomethingTrigger = "Show";
    private int _showSomethingTriggerID;

    private bool _showing;
    private Queue<Sprite> _toShow = new();
    private GameEventsAndResponses _gameEventsAndResponses = new();


    private void Awake()
    {
        _showSomethingTriggerID = Animator.StringToHash(_showSomethingTrigger);
        AnimatorStateEnterEvent.GetEnterEvent(_animator).OnEnter += OnAnimatorEntersIdle;

        _gameEventsAndResponses.SetDataResponses(
            (_scannedItemEvent, OnScanItemBeforeCast)
            , (_scannedJellyEvent, OnScanJellyBeforeCast)
            );
    }

    private void OnEnable()
    {
        _gameEventsAndResponses.Register();
    }

    private void OnDisable()
    {
        _gameEventsAndResponses.Unregister();
    }

    private void OnAnimatorEntersIdle()
    {
        _showing = false;
        CheckShowNext();
    }

    private void CheckShowNext()
    {
        if (_showing)
        {
            return;
        }
        if (_toShow.Count == 0)
        {
            return;
        }
        _showing = true;
        _image.sprite = _toShow.Dequeue();
        _animator.SetTrigger(_showSomethingTriggerID);
    }

    private void OnScanItemBeforeCast(object item)
    {
        ItemBase itemCasted = item as ItemBase;
        if (itemCasted == null)
        {
            throw new System.ArgumentException("item must be non-null and an ItemBase");
        }
        EnqueueThingToShow(itemCasted.Icon);
    }

    private void OnScanJellyBeforeCast(object jellyType)
    {
        if (!(jellyType is Jellies.JellyType))
        {
            throw new System.ArgumentException("jellyType must be a Jellies.Parameters.JellyType");
        }
        Jellies.JellyType jellyTypeCasted = (Jellies.JellyType)jellyType;
        EnqueueThingToShow(GetSpriteForJelly(jellyTypeCasted));
    }

    private Sprite GetSpriteForJelly(Jellies.JellyType jellyType)
    {
        for (int i = 0; i < _jellyImages.Length; i++)
        {
            if (_jellyImages[i].jellyType == jellyType)
            {
                return _jellyImages[i].sprite;
            }
        }
        throw new System.Exception($"No image for the jelly type {jellyType} is in _jellyImages.");
    }

    private void EnqueueThingToShow(Sprite sprite)
    {
        _toShow.Enqueue(sprite);
        CheckShowNext();
    }
}
