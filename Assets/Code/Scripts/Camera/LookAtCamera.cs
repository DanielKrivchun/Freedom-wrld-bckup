using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    public Transform m_cam;

    private void Start()
    {
        m_cam = NetworkCamera.Instance.MyCam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_cam != null)
        {
            transform.LookAt(m_cam);
        }
    }
}
