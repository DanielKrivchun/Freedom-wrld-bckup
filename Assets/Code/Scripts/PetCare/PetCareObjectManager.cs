using DG.Tweening;
using UnityEngine;

public class PetCareObjectManager : MonoBehaviour
{
    public PetCareStateManager petStateManager;
    public PetCareInputManager petInputManager;

    [Space]
    public PetCareCameraViewManager cameraViewManager;

    [Space]
    public GameObject eatObjectHolder;
    public Transform eatPoint;

    [Space]
    public GameObject bathObjectHolder;
    public Transform bathPoint;
    public BathObject bathObject;

    [Space]
    public GameObject sleepCanvas;
    public Transform sleepPoint;
    public SleepManager sleepManager;

    public void ManagePetCareObjects(PetCareState state)
    {
        //Stop pet from navigating
        petInputManager.SetPetToIdle();

        //Set camera front for Happy
        if (state == PetCareState.Happy)
        {
            
            cameraViewManager.SetCameraFrontView();
        }
        else
        {
            cameraViewManager.SetCameraTopView();
        }

        switch (state)
        {
            case PetCareState.Happy:
                petInputManager.transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 1f);

                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;

            case PetCareState.Eat:
                petInputManager.SetDestinationPointForCareTakingOrTraining(eatPoint.position, false);
                petStateManager.SetAvailabeFoodItemOnTable();

                bathObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);

                break;

            case PetCareState.Clean:
                petInputManager.SetDestinationPointForCareTakingOrTraining(bathPoint.position, false);

                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;

            case PetCareState.Energy:
                petInputManager.SetDestinationPointForCareTakingOrTraining(sleepPoint.position, false);

                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                break;

            default:
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;
        }

        //Start Idle Timer for Pet Care State
        if(state != PetCareState.None)
        {
            petStateManager.StartIdleTimer();
        }
    }

    public void PetReachedSelectedObjectDestination()
    {
        //Set camera front for Eat & Clean state after pet reached to destination point
        if (petStateManager.selectedPetCareState == PetCareState.Eat &&
            petStateManager.selectedPetCareState == PetCareState.Clean)
        {
            cameraViewManager.SetCameraFrontView();
        }

        switch (petStateManager.selectedPetCareState)
        {
            case PetCareState.Eat:
                eatObjectHolder.SetActive(true);
                break;

            case PetCareState.Clean:
                bathObjectHolder.SetActive(true);
                //bathObject.MakePetReadyForBath();
                break;

            case PetCareState.Energy:
                sleepCanvas.SetActive(true);
                break;
        }
    }
}
