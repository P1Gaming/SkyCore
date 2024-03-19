using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Cinemachine;


/// <summary>
/// This class generatets a cube of blocks with the specified side length.
/// </summary>
public class BlockProfilingTest : MonoBehaviour
{
    [HeaderAttribute("Cube Generation")]
    [SerializeField]
    [Tooltip("The block prefab used to spawn each block in the generated cube.")]
    private GameObject _blockPrefab;

    [SerializeField]
    [Tooltip("The length of the sides of the generated cube.")]   
    private uint _sideLengthOfGeneratedCube = 10;


    [Header("Camera")]
    [Tooltip("The cinemachine virtual camera that is controlling the main camera.")]
    [SerializeField]
    private CinemachineVirtualCamera _blockVCam;

    [Tooltip("How long it takes (in seconds) for the camera to revolve around the generated cube once.")]
    [SerializeField]
    private float _camOrbitTime = 10f;



    private float _camOrbitSpeed;

    private float _halfSideLengthOfGeneratedCube;

    private CinemachineOrbitalTransposer _orbitalTransposer;



    // Start is called before the first frame update
    void Awake()
    {
        if (_blockPrefab == null)
            throw new Exception("The block prefab has not been set in the inspector!");

        if (_blockVCam == null)
            throw new Exception("The block birtual camera has not been set in the inspector!");


        _orbitalTransposer = _blockVCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();


        SetCubeSize(_sideLengthOfGeneratedCube);        
        GenerateBlocks();
    }

    float elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime >= 10 && transform.childCount < 0)
            DestroyAllCubes();

        if (_orbitalTransposer != null)
            _orbitalTransposer.m_XAxis.Value += Time.deltaTime * _camOrbitSpeed;
    }

    // Generate a block with the specified side length.
    public void GenerateBlocks()
    {
        // Destroy any cubes that may still exist from previous generations.
        DestroyAllCubes();


        for (int y = 0; y < _sideLengthOfGeneratedCube; y++)
        {
            for (int x = 0; x < _sideLengthOfGeneratedCube; x++)
            {
                for (int z = 0; z < _sideLengthOfGeneratedCube; z++)
                {
                    Instantiate(_blockPrefab, 
                                new Vector3(x - _halfSideLengthOfGeneratedCube,y - _halfSideLengthOfGeneratedCube, z - _halfSideLengthOfGeneratedCube), // We subtract half the side length to shift all the blocks so the cube we are generating will be centered on the origin.
                                Quaternion.identity, 
                                transform);
                } // end for z

            } // end for x
             
        } // end for y

    }

    private void DestroyAllCubes()
    {
        for (int i = transform.childCount - 1; i>= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child);
        }
    }

    private void SetCubeSize(uint size)
    {
        _sideLengthOfGeneratedCube = size;

        _halfSideLengthOfGeneratedCube = _sideLengthOfGeneratedCube / 2;


        // The part in ()s is just calculating the circumference of the camera's circular path.
        _camOrbitSpeed = (2f * Mathf.PI * _sideLengthOfGeneratedCube) / _camOrbitTime;

        // Modify the camera offset so it is always offset up by half the height of the cube, and offset the correct distance on the Z-axis.
        // The Z-Axis is defining how far away the camera is from the center of the cube (its lookat target).
        _orbitalTransposer.m_FollowOffset = new Vector3(0, 
                                                        _sideLengthOfGeneratedCube * 0.75f,
                                                        _sideLengthOfGeneratedCube * 2f);
    }

}
