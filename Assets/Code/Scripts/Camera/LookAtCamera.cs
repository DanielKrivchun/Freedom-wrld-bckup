using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    public Transform target;

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
            transform.LookAt(target);
        }
    }
}
