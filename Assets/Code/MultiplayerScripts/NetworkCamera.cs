using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCamera : MonoBehaviour
{
    public CinemachineVirtualCamera MyCam;


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

    public void _SetUpCamera(Transform _target)
    {
        MyCam.Follow = _target;
        MyCam.LookAt = _target;
    }
}
