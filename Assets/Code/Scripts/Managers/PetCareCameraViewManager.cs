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

    [Space]
    public Vector3 topViewPos;
    public Quaternion topViewRot;

    [Space]
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
        virtualCam.Follow = null;
        virtualCam.LookAt = null;
        transposer.m_FollowOffset = Vector3.zero;

        if (!isTransitioning && transposer.m_FollowOffset != topViewPos)
        {
            Debug.Log("Move to top");
            StartCoroutine(TransitionToOffset(frontViewPos, topViewPos, transitionTime));
            //StartCoroutine(TransitionToFixedPosition(topViewPos, transitionTime));
        }

        /*GetComponent<CinemachineBrain>().enabled = false;
        //transposer.m_FollowOffset = Vector3.zero;
        transform.DOMove(topViewPos, 1.5f);
        transform.DORotateQuaternion(topViewRot, 1.5f);*/
    }

    public void SetCameraFrontView()
    {
        virtualCam.Follow = player;
        virtualCam.LookAt = player;

        if (!isTransitioning && transposer.m_FollowOffset != frontViewPos)
        {
            StartCoroutine(TransitionToOffset(topViewPos, frontViewPos, transitionTime));
        }

        /*transform.DOMove(player.position + frontViewPos, 1.5f);
        transform.DORotateQuaternion(frontViewRot, 1.5f);*/

        //virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = frontViewPos;
        //StartCoroutine(SetPlayerFollowAndLookAt());
    }

    IEnumerator SetPlayerFollowAndLookAt()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<CinemachineBrain>().enabled = true;

        //StartCoroutine(TransitionToOffset(transform.position, frontViewPos, 0.5f));
        //virtualCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = frontViewPos;
    }


    private IEnumerator TransitionToOffset(Vector3 currentOffset, Vector3 newOffset, float duration)
    {
        isTransitioning = true;
        elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transposer.m_FollowOffset = Vector3.Lerp(currentOffset, newOffset, t);
            yield return null;
        }
        transposer.m_FollowOffset = newOffset;
        isTransitioning = false;
    }

    private IEnumerator TransitionToFixedPosition(Vector3 fixedPosition, float duration)
    {
        isTransitioning = true;
        elapsedTime = 0.0f;
        Vector3 startPosition = virtualCam.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            virtualCam.transform.position = Vector3.Lerp(startPosition, fixedPosition, t);
            yield return null;
        }

        virtualCam.transform.position = fixedPosition;
        isTransitioning = false;
    }
}
