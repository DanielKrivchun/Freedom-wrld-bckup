using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void OnDisable()
    {
        NetworkEventManager.e_camera_change -= _CameraSetup;
        NetworkEventManager.e_focus_on_player -= _FocusOnPlayer;
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