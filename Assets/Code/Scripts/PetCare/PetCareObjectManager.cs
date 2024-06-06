using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                petStateManager.StartIdleTimer();
                petInputManager.transform.DORotateQuaternion(Quaternion.Euler(0f, 180f, 0f), 1f);

                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;

            case PetCareState.Eat:
                petInputManager.SetDestinationPointForCareTakingOrTraining(eatPoint.position, false);
                petStateManager.SetAvailabeFoodItemOnTable();
                petStateManager.StartIdleTimer();

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
    }

    public void PetReachedSelectedObjectDestination()
    {
        if (petStateManager.selectedPetCareState != PetCareState.Energy)
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
                bathObject.MakePetReadyForBath();
                break;

            case PetCareState.Energy:
                sleepCanvas.SetActive(true);
                break;
        }
    }
}
