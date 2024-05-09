using DG.Tweening;
using System;
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
    public Slider happyFillSlider;
    public Slider eatFillSlider;
    public Slider cleanFillSlider;
    public Slider sleepFillSlider;

    [Space]
    public GameObject eatObjectHolder;
    public GameObject bathObjectHolder;

    public bool isReadyForBath;

    Vector3 startPos;

    public int hunger, happiness, cleanliness, energy;
    public int foodTickRate, happinessTickRate, energyTickRate;
    public DateTime lastTimeFeed, lastTimeHappy, lastTimeClean;

    public void Initialize(int hunger, int happiness, int cleanliness, int energy, int foodTickRate, int happinessTickRate, int energyTickRate)
    {
        lastTimeFeed = DateTime.Now;
        lastTimeHappy = DateTime.Now;
        lastTimeClean = DateTime.Now;
        this.hunger = hunger;
        this.happiness = happiness;
        this.energy = energy;
        this.cleanliness = cleanliness;
        this.foodTickRate = foodTickRate;
        this.happinessTickRate = happinessTickRate;
        this.energyTickRate = energyTickRate;
    }

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
            bathObjectHolder.SetActive(false);
        }
        else if(selectedPetCareState == PetCareState.Clean)
        {
            bathObjectHolder.SetActive(true);
            eatObjectHolder.SetActive(false);
        }
        else
        {
            bathObjectHolder.SetActive(false);
            eatObjectHolder.SetActive(false);
        }
    }

    public void SetSelectedStateFillerImage(float value)
    {
        switch (selectedPetCareState)
        {
            case PetCareState.Happy:
                if(happyFillSlider.value < 1f)
                {
                    float totalValue = happyFillSlider.value + value;
                    happyFillSlider.DOValue(totalValue, 0.5f);

                    happiness = (int)(totalValue * 100);
                    lastTimeHappy = DateTime.Now;
                }
                break;

            case PetCareState.Eat:
                if (eatFillSlider.value < 1f)
                {
                    float totalValue = eatFillSlider.value + value;
                    eatFillSlider.DOValue(totalValue, 0.5f);

                    hunger = (int)(totalValue * 100);
                    lastTimeFeed = DateTime.Now;
                }
                break;

            case PetCareState.Clean:
                if (cleanFillSlider.value < 1f)
                {
                    float totalValue = cleanFillSlider.value + value;
                    cleanFillSlider.DOValue(totalValue, 0.5f);

                    cleanliness = (int)(totalValue * 100);
                    lastTimeClean = DateTime.Now;
                }
                break;
        }
    }
}
