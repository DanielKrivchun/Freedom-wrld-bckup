using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Beamable.Api.Notification.PubNubOp;

public class NetworkCamera : MonoBehaviour
{
    public CinemachineBrain Brain;
    [Space]
    public List<Transform> InitialTransforms;
    [Space]
    public List<_CamList> All_Cameras;
    [Space]
    public CinemachineFreeLook WinCam;
    public GameObject WinCamera;
    [Space]
    public List<_CamCofigs> CamConfigs;
    [Space]
    public GameObject Confetti;

    public GameObject CurruntCam;
    private CinemachineVirtualCamera cam;

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

    private void _GameStarted()
    {
        WinCamera.SetActive(false);
        cam = _GetCamm("0");
        cam.gameObject.SetActive(false);
        cam = _GetCamm("1");
        cam.gameObject.SetActive(false);
        cam = _GetCamm("2");
        cam.gameObject.SetActive(false);
        cam = _GetCamm("3");
        cam.gameObject.SetActive(false);
        cam = _GetCamm("4");
        cam.gameObject.SetActive(false);
        GameObject g = _GetCam("follocam");
        g.SetActive(false);
        g = _GetCam("startracecam");
        g.SetActive(false);
        CurruntCam = _GetCam("startcam");
    }

    private void _FocusOnPlayer(int _no)
    {
        Debug.Log(_no);
        cam = _GetCamm(_no.ToString());
        cam.gameObject.SetActive(true);
        cam.Follow = InitialTransforms[_no];
        cam.LookAt = InitialTransforms[_no];
    }

    private void _CameraSetup(_CamState _CamState)
    {
        switch (_CamState)
        {
            case _CamState.InitialCam:
                cam = _GetCamm("startcam");
                cam.gameObject.SetActive(true);
                NetworkEventManager._EventTextLookat(null);
                break;
            case _CamState.Start:
                CurruntCam = _GetCam("startracecam");
                CurruntCam.SetActive(true);
                NetworkEventManager._EventTextLookat(CurruntCam.transform);
                break;

            case _CamState.Follow:
                CurruntCam = _GetCam("follocam");
                CurruntCam.SetActive(true);
                NetworkEventManager._EventTextLookat(CurruntCam.transform);
                break;
        }
    }

    GameObject _GetCam(string _camname)
    {
        return All_Cameras.Find(asd => asd.type == _camname).cam.gameObject;
    }

    CinemachineVirtualCamera _GetCamm(string _camname)
    {
        return All_Cameras.Find(asd => asd.type == _camname).cam;
    }

    public void _SetUpCamera(Transform _target)
    {
        cam = _GetCamm("follocam");
        cam.Follow = _target;
        cam.LookAt = _target;
    }

    public void _ActiveWinScene()
    {
        NetworkEventManager._EventTextLookat(WinCam.transform);

        cam = _GetCamm("follocam");
        cam.gameObject.SetActive(false);
        cam = _GetCamm("startcam");
        cam.gameObject.SetActive(false);
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
    InitialCam
}

[System.Serializable]
public class _CamCofigs
{
    public GameObject Target;
    [Header("Boddy Variables")]
    public string BodyType;
    public Vector2 Offset;

    public string AimType;

}

[System.Serializable]
public class _CamList
{
    public string type;
    public CinemachineVirtualCamera cam;
}