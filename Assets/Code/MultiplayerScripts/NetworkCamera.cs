using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCamera : MonoBehaviour
{
    #region VARIABLES
    //LIST OF ALL CAMERA WHERE CAMERA CAN BE GET BY THEIR NAMES
    public List<_CamList> All_Cameras;
    [Space]
    //WIN CAMERA OBJECTS
    public CinemachineFreeLook WinCam;
    public GameObject WinCamera;
    [Space]
    ///TRAGSFORM TO ANIMATE START RACE CAMERA
    public Transform StartPoint;
    public Transform EndPoint;
    [Space]
    //CONFETTI ON GAME WIN
    public GameObject Confetti;

    public GameObject CurruntCam;
    private CinemachineVirtualCamera Cam;
    #endregion

    public static NetworkCamera Instance;

    #region UNITY METHODS
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        NetworkEventManager.e_camera_change += _CameraSetup;
        NetworkEventManager.e_focus_on_player += _FocusOnPlayer;
        NetworkEventManager.e_get_set_go += _GameStarted;
    }

    private void OnDisable()
    {
        NetworkEventManager.e_get_set_go -= _GameStarted;
        NetworkEventManager.e_camera_change -= _CameraSetup;
        NetworkEventManager.e_focus_on_player -= _FocusOnPlayer;
    }
    #endregion

    #region EVENT CALLBACKS
    private void _GameStarted()
    {
        WinCamera.SetActive(false);
        Cam = _GetCamm("0");
        Cam.gameObject.SetActive(false);
        GameObject g = _GetCam("follocam");
        g.SetActive(false);
        g = _GetCam("startracecam");
        g.SetActive(false);
        CurruntCam = _GetCam("startracecam");
    }

    private void _FocusOnPlayer(int _no)
    {
        Cam = _GetCamm("0");
        Cam.gameObject.SetActive(true);
        Cam.transform.DOMove(EndPoint.position, 8f);
    }

    /// <summary>
    /// CALLED BY EVENT AND ACTIVATES CAMERA
    /// </summary>
    /// <param name="_CamState"></param>
    private void _CameraSetup(_CamState _CamState)
    {
        switch (_CamState)
        {
            case _CamState.InitialCam:
                WinCamera.SetActive(false);
                Cam = _GetCamm("0");
                Cam.transform.DOMove(StartPoint.position, 8f);

                Cam = _GetCamm("startcam");
                Cam.gameObject.SetActive(true);
                CurruntCam = Cam.gameObject;
                NetworkEventManager._EventTextLookat(CurruntCam.transform);
                break;
            case _CamState.Start:
                CurruntCam = _GetCam("startracecam");
                CurruntCam.SetActive(true);
                NetworkEventManager._EventTextLookat(CurruntCam.transform);
                _WaitAndDisbaleCam(_GetCam("startcam"));
                break;

            case _CamState.Follow:
                CurruntCam = _GetCam("follocam");
                CurruntCam.SetActive(true);
                NetworkEventManager._EventTextLookat(CurruntCam.transform);
                break;
            case _CamState.DisableFollow:
                Cam = _GetCamm("follocam");
                Cam.Follow = null;
                Cam.LookAt = null;
                break;
        }
    }

    #endregion

    #region CAM UTILITIES
    async void _WaitAndDisbaleCam(GameObject _obj)
    {
        await Utils._Waiter(500);
        _obj.SetActive(false);
    }


    GameObject _GetCam(string _camname)
    {
        return All_Cameras.Find(asd => asd.type == _camname).cam.gameObject;
    }

    CinemachineVirtualCamera _GetCamm(string _camname)
    {
        //Debug.Log(_camname);
        return All_Cameras.Find(asd => asd.type == _camname).cam;
    }
    #endregion

    #region FOLOW CAMERA REGIOION AND WIN SCENE ACTIVATION
    /// <summary>
    /// SETTING UP FOLLOW CAMERA
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="PathNo"></param>
    public void _SetUpCamera(Transform _target, int PathNo)
    {
        Debug.Log(PathNo);
        Cam = _GetCamm("follocam");
        CameraFollower cameraFollower = Cam.GetComponent<CameraFollower>();
        cameraFollower.target_transform = _target;
    }

    public void _ActiveWinScene()
    {
        NetworkEventManager._EventTextLookat(WinCam.transform);

        Cam = _GetCamm("follocam");
        Cam.gameObject.SetActive(false);
        Cam = _GetCamm("startcam");
        Cam.gameObject.SetActive(false);
        WinCamera.gameObject.SetActive(true);
        //FollowCam.gameObject.SetActive(false);
        Confetti.gameObject.SetActive(true);
    }
    #endregion
}

#region CLASS AND ENUMS

public enum _CamState
{
    none,
    Start,
    Follow,
    InitialCam,
    DisableFollow
}

[System.Serializable]
public class _CamList
{
    public string type;
    public CinemachineVirtualCamera cam;
}
#endregion