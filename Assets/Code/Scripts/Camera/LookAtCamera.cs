using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Fusion.Sockets.NetBitBuffer;

public class LookAtCamera : MonoBehaviour
{

    public Transform target;

    public Vector3 Offset;
    private Vector3 pos;


    private void Start()
    {
        target = NetworkCamera.Instance.CurruntCam.transform;
    }

    private void OnEnable()
    {
        NetworkEventManager.e_text_lookat += _LoookatTargetChange;
    }

    private void OnDisable()
    {
        NetworkEventManager.e_text_lookat -= _LoookatTargetChange;
    }

    private void _LoookatTargetChange(Transform _t)
    {
        target = _t;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            pos=target.position;
            //transform.LookAt(pos + Offset, Vector3.up);
            transform.LookAt(transform.position + target.transform.rotation * Vector3.forward, target.rotation * Vector3.up);
        }
    }
}
