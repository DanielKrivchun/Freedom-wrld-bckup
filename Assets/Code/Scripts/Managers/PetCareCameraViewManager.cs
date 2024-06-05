using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCareCameraViewManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;

    [Space]
    public Vector3 topViewPos;
    public Vector3 frontViewPos;

    [Space]
    public float transitionTime;

    private CinemachineTransposer transposer;
    private bool isTransitioning;
    private float elapsedTime = 0f;

    private void Start()
    {
        transposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetCameraTopView();
        }
        else if (Input.GetKeyDown(KeyCode.F)) 
        { 
            SetCameraFrontView(); 
        }
    }


    public void SetCameraTopView()
    {
        if(!isTransitioning && transposer.m_FollowOffset != topViewPos)
        {
            StartCoroutine(TransitionToOffset(frontViewPos, topViewPos, transitionTime));
        }
    }

    public void SetCameraFrontView()
    {
        //transposer.m_FollowOffset = topViewPos;
        if(!isTransitioning && transposer.m_FollowOffset != frontViewPos)
        {
            StartCoroutine(TransitionToOffset(topViewPos, frontViewPos, transitionTime));
        }
    }


    private IEnumerator TransitionToOffset(Vector3 currentOffset, Vector3 newOffset, float duration)
    {
        isTransitioning = true;
        elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(currentOffset, newOffset, t);
            yield return null;
        }

        virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = newOffset;
        isTransitioning = false;
    }
}
