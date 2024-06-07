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
    public float transitionTime;


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
        transform.DOMove(topViewPos, transitionTime);
        transform.DORotate(topViewRot, transitionTime);
    }

    public void SetCameraFrontView()
    {
        transform.DOMove(player.position + frontViewPos, transitionTime);
        transform.DORotate(frontViewRot, transitionTime);
    }
}
