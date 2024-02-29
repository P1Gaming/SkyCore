using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscMenuToggle : MonoBehaviour
{
    [SerializeField]
    private GameObject _toggler;



    public void SetActive(bool active)
    {
        _toggler.SetActive(active);
    }
}
