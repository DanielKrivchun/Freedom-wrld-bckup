using DG.Tweening;
using UnityEngine;

public class PetCareCameraViewManager : MonoBehaviour
{
    [Space]
    public Transform player;

    [Space]
    public Vector3 topViewPos;
    public Vector3 topViewRot;

    [Space]
    public Vector3 frontViewPos;
    public Vector3 frontViewRot;

    [Space]
    public Vector3 runningTrainingViewPos;
    public Vector3 runningTrainingViewRot;

    [Space]
    public Vector3 swimmingTrainingViewPos;
    public Vector3 swimmingTrainingViewRot;

    [Space]
    public Vector3 climbingTrainingViewPos;
    public Vector3 climbingTrainingViewRot;

    [Space]
    public float transitionTime;


    private void Update()
    {
        //For testing
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetCameraTopView();
        }
        else if (Input.GetKeyDown(KeyCode.F)) 
        { 
            SetCameraFrontView(); 
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SetCameraRunningTrainingView();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetCameraSwimmingTrainingView();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            SetCameraClimbingTrainingView();
        }
    }

    //Top view
    public void SetCameraTopView()
    {
        transform.DOMove(topViewPos, transitionTime);
        transform.DORotate(topViewRot, transitionTime);
    }

    //Front view
    public void SetCameraFrontView()
    {
        transform.DOMove(player.position + frontViewPos, transitionTime);
        transform.DORotate(frontViewRot, transitionTime);
    }

    //Running training view
    public void SetCameraRunningTrainingView()
    {
        transform.DOMove(runningTrainingViewPos, transitionTime);
        transform.DORotate(runningTrainingViewRot, transitionTime);
    }

    //Running training view
    public void SetCameraSwimmingTrainingView()
    {
        transform.DOMove(swimmingTrainingViewPos, transitionTime);
        transform.DORotate(swimmingTrainingViewRot, transitionTime);
    }

    //Running training view
    public void SetCameraClimbingTrainingView()
    {
        transform.DOMove(climbingTrainingViewPos, transitionTime);
        transform.DORotate(climbingTrainingViewRot, transitionTime);
    }
}
