using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetCareCameraViewManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;

    [Space]
    public Transform player;
    public Transform map;

    [Space]
    public Vector3 topViewPos;
    public Quaternion topViewRot;
    public Vector3 frontViewPos;
    public Quaternion frontViewRot;

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
        /*virtualCam.Follow = map;
        virtualCam.LookAt = map;

        if (!isTransitioning && transposer.m_FollowOffset != topViewPos)
        {
            StartCoroutine(TransitionToOffset(frontViewPos, topViewPos, transitionTime));
        }*/

        GetComponent<CinemachineBrain>().enabled = false;
        transposer.m_FollowOffset = Vector3.zero;
        transform.DOMove(topViewPos, 1.5f);
        transform.DORotateQuaternion(topViewRot, 1.5f);
    }

    public void SetCameraFrontView()
    {
        GetComponent<CinemachineBrain>().enabled = true;

        if (!isTransitioning && transposer.m_FollowOffset != frontViewPos)
        {
            StartCoroutine(TransitionToOffset(transform.position, frontViewPos, transitionTime));
        }

        /*transform.DOMove(frontViewPos, 1.5f);
        transform.DORotateQuaternion(frontViewRot, 1.5f);
        StartCoroutine(SetPlayerFollowAndLookAt());*/
    }

    IEnumerator SetPlayerFollowAndLookAt()
    {
        yield return new WaitForSeconds(1.5f);
        GetComponent<CinemachineBrain>().enabled = true;
        virtualCam.Follow = player;
        virtualCam.LookAt = player;
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
