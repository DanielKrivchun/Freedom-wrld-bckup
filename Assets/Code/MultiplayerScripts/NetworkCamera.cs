using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCamera : MonoBehaviour
{
    public CinemachineBrain Brain;
    [Space]
    public CinemachineFreeLook WinCam;
    public GameObject WinCamera;
    [Space]
    public CinemachineVirtualCamera StartCam;
    public CinemachineVirtualCamera FollowCam;
    [Space]
    public GameObject Confetti;

    private GameObject CurruntCam;

    public static NetworkCamera Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }

        CurruntCam = FollowCam.gameObject;
    }

    private void OnEnable()
    {
        NetworkEventManager.e_camera_cnage += _CameraSetup;
    }

    private void OnDisable()
    {
        NetworkEventManager.e_camera_cnage -= _CameraSetup;

    }

    private void _CameraSetup(_CamState _CamState)
    {
        switch (_CamState)
        {
            case _CamState.Start:
                StartCam.gameObject.SetActive(true);
                break;

            case _CamState.Follow:
                FollowCam.gameObject.SetActive(true);
                break;
        }
    }



    public void _SetUpCamera(Transform _target)
    {
        FollowCam.Follow = _target;
        FollowCam.LookAt = _target;
    }

    public void _ActiveWinScene()
    {
        WinCamera.gameObject.SetActive(true);
        //FollowCam.gameObject.SetActive(false);
        Confetti.gameObject.SetActive(true);
    }
}

public enum _CamState
{
    none,
    Start,
    Follow,
}
