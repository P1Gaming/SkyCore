using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows an image for the drone's communication.
/// </summary>
public class PictogramBehavior : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Image _backdropImage;
    [SerializeField]
    private Sprite _backdropSprite;

    private void Awake()
    {
        _backdropImage.sprite = _backdropSprite;
    }

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

    public void ChangePictogramImage(Sprite data)
    {
        if (data != null)
        {
            _image.sprite = data;
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
