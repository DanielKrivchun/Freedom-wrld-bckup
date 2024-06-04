using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCareObjectManager : MonoBehaviour
{
    public PetCareStateManager petStateManager;
    public PetCareInputManager petInputManager;

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
        switch (petStateManager.selectedPetCareState)
        {
            case PetCareState.Clean:
                bathObject.MakePetReadyForBath();
                break;

            case PetCareState.Energy:
                sleepCanvas.SetActive(true);
                break;

            default:
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepCanvas.SetActive(false);
                break;
        }
    }
}
