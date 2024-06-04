using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCareObjectManager : MonoBehaviour
{
    public PetCareInputManager petInputManager;

    [Space]
    public GameObject eatObjectHolder;
    public Transform eatPoint;

    [Space]
    public GameObject bathObjectHolder;
    public Transform bathPoint;

    [Space]
    public GameObject sleepCanvas;
    public Transform sleepPoint;

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
                PetCareStateManager.instance.SetAvailabeFoodItemOnTable();

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

                sleepCanvas.SetActive(true);
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
}
