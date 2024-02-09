using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// This script will be responsible for enacting the response received by 
/// the Listener attached to the drone.
/// </summary>
public class PictogramBehavior : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Image _backdropImage;
    [SerializeField]
    private Sprite _backdropSprite;

    private Transform _player;

    private void Awake()
    {
        _player = Player.Motion.PlayerMovement.Instance.transform;
    }

    //private void LateUpdate()
    //{
    //    // we'll need a better way to decide how to rotate the drone, e.g. it could look towards wherever it's
    //    transform.LookAt(_player);
    //    Vector3 eulerAngles = transform.rotation.eulerAngles;
    //    transform.rotation = Quaternion.Euler(new Vector3(0, eulerAngles.y, 0));
    //}

    public void SetImageActive(bool active)
    {
        _backdropImage.gameObject.SetActive(active);
        _image.gameObject.SetActive(active);
    }

    /// <summary>
    /// This method is used to change the pictogram image that is attached to the drone
    /// </summary>
    public void ChangePictogramImage(object data)
    {
        Sprite asSprite = data as Sprite;
        if (asSprite != null)
        {
            if (_image.sprite != asSprite) // if condition is for performance (might be pointless)
            {
                _image.sprite = asSprite;
            }
        }
        else
        {
            throw new System.ArgumentException("data must be Sprite: " + data);
        }
    }

    public void SetBackdropImage()
    {
        if (_backdropImage != null)
        {
            _backdropImage.sprite = _backdropSprite;
        }
        else
        {
            throw new System.ArgumentException("Backdrop image was not set as active. ");
        }
    }
}
