using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCareObjectManager : MonoBehaviour
{
    [Space]
    public GameObject eatObjectHolder;
    public GameObject bathObjectHolder;
    public GameObject sleepBtn;

    public void ManagePetCareObjects(PetCareState state)
    {
        switch (state)
        {
            case PetCareState.Happy:
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepBtn.SetActive(false);
                break;

            case PetCareState.Eat:
                PetCareStateManager.instance.SetAvailabeFoodItemOnTable();

                eatObjectHolder.SetActive(true);
                bathObjectHolder.SetActive(false);
                sleepBtn.SetActive(false);
                break;

            case PetCareState.Clean:
                bathObjectHolder.SetActive(true);
                eatObjectHolder.SetActive(false);
                sleepBtn.SetActive(false);
                break;

            case PetCareState.Energy:
                sleepBtn.SetActive(true);
                sleepBtn.GetComponent<Button>().interactable = true;
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                break;

            default:
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepBtn.SetActive(false);
                break;
        }
    }
}
