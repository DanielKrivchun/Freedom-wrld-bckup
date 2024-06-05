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
        if(state == PetCareState.Happy)
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
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;

            case PetCareState.Eat:
                petInputManager.SetDestinationPoint(eatPoint.position);
                petStateManager.SetAvailabeFoodItemOnTable();

                eatObjectHolder.SetActive(true);
                bathObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);

                break;

            case PetCareState.Clean:
                petInputManager.SetDestinationPoint(bathPoint.position);

                bathObjectHolder.SetActive(true);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;

            case PetCareState.Energy:
                petInputManager.SetDestinationPoint(sleepPoint.position);

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
        if(petStateManager.selectedPetCareState != PetCareState.Energy)
        {
            cameraViewManager.SetCameraFrontView();
        }

        switch (petStateManager.selectedPetCareState)
        {
            case PetCareState.Clean:
                bathObject.MakePetReadyForBath();
                break;

            case PetCareState.Energy:
                sleepCanvas.SetActive(true);
                break;
        }
    }
}
