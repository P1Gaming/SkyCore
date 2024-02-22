using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using static UnityEngine.CullingGroup;

public class DroneCircMov : MonoBehaviour
{
    // Enum to define different states of the drone


    //   private State _currentState = State.Approach; // Initialize drone state to Follow
    [SerializeField]
    private GameObject _player;
    [SerializeField] private GameObject _drone;
    Vector3 _directiontoplayer;
   // public Camera camera;
    public LineRenderer line1;
    // public string "SQ", "SC","I","T", "C","F";
    float _timeStart = 3;
    float _timeStart1 = 8;
    float _timeStart2 = 5;
    float _timeStart3 = 15;
    float _t1 = 4;
    float _t2 = 3;
    float _t3 = 3;
    float _t4 = 5;
    private void Start()
    {

    }

    private void Update()
    {
        line1.SetPosition(0, _drone.transform.position);
        line1.SetPosition(1, _player.transform.position);

        if (_timeStart > 0)
        {
            line1.enabled = true;

            _timeStart -= Time.deltaTime;

            DroneCirclePlayer();
            // line1.enabled = false;
        }


        if (_timeStart1 > 0)
        {

            line1.enabled = (true);
            _timeStart1 -= Time.deltaTime;

            Ap();

        }
        if (_timeStart2 > 0)
        {
            line1.enabled = (false);
            _timeStart2 -= Time.deltaTime;

            IdlePrime();
            // line1.enabled = false;
            Linear();
            //Debug.Log("enebling line");
            //line1.enabled = false;
        }
        if (_timeStart3 > 0 && _timeStart2 <= 0)
        {

            _timeStart3 -= Time.deltaTime;
            line1.enabled = (true);
            IdlePrime();
          //  Debug.Log("hello tv");
            // line1.enabled = false;

            //Debug.Log("enebling line");

        }
        else
        {
            line1.enabled = (false);
        }



    }

    private void IdlePrime()
    {

        _drone.transform.position = _drone.transform.position;
    }
    private void Linear()
    {
        line1.positionCount = 2;

        line1.enabled = (false);
        //line2.enabled = (true);
        line1.startWidth = 1;
        line1.endWidth = 1;
        // _drone.transform.LookAt(_player.transform.position);
        // _player.transform.LookAt(_drone.transform.position);
        line1.SetPosition(0, _drone.transform.position);
        line1.SetPosition(1, _player.transform.position);


        //    PLAY MUSIC below (thank you)
        // ???? src.clip = sfx1;
        //  ?????  src.Play();     
        //  source.PlayOneShot(clip);


        //pause to allow for line to disappear for 4 seconds

        //_currentState = State.Square;
        //StartCoroutine(Sq());
    }

    private void Ap()
    {
        line1.enabled = false;
        _drone.transform.LookAt(_player.transform.position);

        _drone.transform.Translate(Vector3.forward * .03f);
        _drone.transform.position = _drone.transform.position;
        //  _drone.transform.Translate(Vector3.right * 3f);

        //line1.enabled = false;


        // StartCoroutine(linear());
    }
    private void DroneCirclePlayer()
    {

        //yield return new WaitForSeconds(5f);
        _drone.transform.RotateAround(_player.transform.position, Vector3.up, 2f);
        // transform.Translate(Vector3.right * 2*Time.deltaTime)

    }
    private void Sq()
    {
        if (_t1 > 0)
        {
            _t1 -= Time.deltaTime;
            _drone.transform.Translate(Vector3.left * 2f);
        }

        if (_t2 > 0)
        {
            _t2 -= Time.deltaTime;
            _drone.transform.Translate(Vector3.forward * 2f);
        }

        if (_t3 > 0)
        {
            _t3 -= Time.deltaTime;
            _drone.transform.Translate(Vector3.right * 2f);
        }
        if (_t4 > 0)
        {
            _t4 -= Time.deltaTime;

            _drone.transform.Translate(Vector3.back * 2f);
        }
        //  StartCoroutine(IdlePrime());

    }
    private void FollowPlayer()
    {
        _directiontoplayer = (_drone.transform.position - _player.transform.position);
        // Check if the player is moving (you can adjust the threshold as needed)
        if (_directiontoplayer.magnitude > 0.1f)
        {
            // Parent the drone to the player's object
            transform.SetParent(_player.transform);
        }
        else
        {
            // Player has stopped, return to idle

            // Unparent the drone
            transform.SetParent(null);
        }
    }
}
