using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCareStateManager : MonoBehaviour
{
    public GameEventState petCareEvent;

    public PetCareState selectedPetCareState;

    [Space]
    public GameObject player;

    [Space]
    public Button happyBtn;
    public Button eatBtn;
    public Button cleanBtn;
    public Button sleepBtn;

    [Space]
    public Image happyFillImg;
    public Image eatFillImg;
    public Image cleanFillImg;
    public Image sleepFillImg;

    [Space]
    public GameObject eatObjectHolder;
    public GameObject toiletObjectHolder;

    public bool isReadyForToilet;

    Vector3 startPos;

    private void Start()
    {
        startPos = player.transform.position;
        happyBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Happy));
        eatBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Eat));
        cleanBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Clean));
        sleepBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Sleep));
    }

    public void OnClickOfPetCareStateBtn(PetCareState selectedState)
    {
        petCareEvent.Raise(selectedState);
    }

    public void CheckForSelectedPetCareState(PetCareState state)
    {
        selectedPetCareState = state;
        player.transform.position = startPos;

        if (selectedPetCareState == PetCareState.Eat)
        {
            eatObjectHolder.SetActive(true);
        }
        else if(selectedPetCareState == PetCareState.Clean)
        {
            toiletObjectHolder.SetActive(true);
            eatObjectHolder.SetActive(false);
        }
        else
        {
            toiletObjectHolder.SetActive(false);
            eatObjectHolder.SetActive(false);
        }
    }

    public void SetSelectedStateFillerImage(float value)
    {
        switch (selectedPetCareState)
        {
            case PetCareState.Happy:
                if(happyFillImg.fillAmount < 1f)
                {
                    float totalValue = happyFillImg.fillAmount + value;
                    happyFillImg.DOFillAmount(totalValue, 0.5f);
                }
                break;

            case PetCareState.Eat:
                if (eatFillImg.fillAmount < 1f)
                {
                    float totalValue = eatFillImg.fillAmount + value;
                    eatFillImg.DOFillAmount(totalValue, 0.5f);
                }
                break;

            case PetCareState.Clean:
                if (cleanFillImg.fillAmount < 1f)
                {
                    float totalValue = cleanFillImg.fillAmount + value;
                    cleanFillImg.DOFillAmount(totalValue, 0.5f);
                }
                break;
        }
    }
}
