using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetCareStateManager : MonoBehaviour
{
    public PetCareData petCareData;
    public bool isDataFromJSON;

    [Space]
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
    public Slider hungerFillSlider;
    public Slider cleanFillSlider;
    public Slider energyFillSlider;

    [Space]
    public GameObject eatObjectHolder;
    public GameObject bathObjectHolder;

    public bool isReadyForBath;

    Vector3 startPos;

    [Space]
    [Header("Pet Care Data")]
    public int hunger, happiness, cleanliness, energy;
    public int foodTickRate, happinessTickRate, energyTickRate;
    public DateTime lastTimeFeed, lastTimeHappy, lastTimeClean;



    private void Start()
    {
        startPos = player.transform.position;
        happyBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Happy));
        eatBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Eat));
        cleanBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Clean));
        sleepBtn.onClick.AddListener(() => OnClickOfPetCareStateBtn(PetCareState.Sleep));

        if (!isDataFromJSON)
        {
            GetAndSetAllPetCareData();
        }
        else
        {
            GetAndSetAllPetCareDataJSON();
        }
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
        else if (selectedPetCareState == PetCareState.Clean)
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

    public void SetSelectedStateFillerImage(int value)
    {
        switch (selectedPetCareState)
        {
            case PetCareState.Happy:
                if (happyFillSlider.value < 100f)
                {
                    happiness = (int)happyFillSlider.value + value;
                    happyFillSlider.DOValue(happiness, 0.5f);

                    lastTimeHappy = DateTime.Now;
                }
                break;

            case PetCareState.Eat:
                if (hungerFillSlider.value < 100f)
                {
                    hunger = (int)hungerFillSlider.value + value;
                    hungerFillSlider.DOValue(hunger, 0.5f);

                    lastTimeFeed = DateTime.Now;
                }
                break;

            case PetCareState.Clean:
                if (cleanFillSlider.value < 100f)
                {
                    cleanliness = (int)cleanFillSlider.value + value;
                    cleanFillSlider.DOValue(cleanliness, 0.5f);

                    lastTimeClean = DateTime.Now;
                }
                break;
        }
    }

    public void StoreAllPetCareData()
    {
        if (!isDataFromJSON)
        {
            petCareData.SetPetCareData(lastTimeHappy, lastTimeFeed, lastTimeClean, happiness, hunger, cleanliness, energy);
        }
        else
        {
            Pet pet = new Pet(lastTimeHappy, lastTimeFeed, lastTimeClean, happiness, hunger, cleanliness, energy);
            DatabaseManager.instance.SavePet(pet);
        }  
    }


    //Scriptable Object Data
    public void GetAndSetAllPetCareData()
    {
        lastTimeHappy = petCareData.lastTimeHappy;
        // Calculate the difference between the two times
        TimeSpan timeDifference = DateTime.Now - lastTimeHappy;

        // Print the difference in the format: days:hours:minutes:seconds
        Debug.Log("Happy Time Difference: " + timeDifference.Days + " days, " + timeDifference.Hours + " hours, " + timeDifference.Minutes + " minutes, " + timeDifference.Seconds + " seconds");

        lastTimeFeed = petCareData.lastTimeFeed;
        Debug.Log("Feed Time Span - " + (DateTime.Now - lastTimeFeed).TotalHours);

        lastTimeClean = petCareData.lastTimeClean;
        Debug.Log("Clean Time Span - " + (DateTime.Now - lastTimeClean).TotalHours);

        happiness = petCareData.happiness;
        happyFillSlider.DOValue(happiness, 0.5f);

        hunger = petCareData.hunger;
        hungerFillSlider.DOValue(hunger, 0.5f);

        cleanliness = petCareData.cleanliness;
        cleanFillSlider.DOValue(cleanliness, 0.5f);

        energy = petCareData.energy;
        energyFillSlider.DOValue(energy, 0.5f);
    }


    //Local Data (JSON)

    public void GetAndSetAllPetCareDataJSON()
    {
        Pet pet = DatabaseManager.instance.LoadPet();

        lastTimeHappy = pet.lastTimeHappy;
        // Calculate the difference between the two times
        TimeSpan timeDifference = DateTime.Now - lastTimeHappy;

        // Print the difference in the format: days:hours:minutes:seconds
        Debug.Log("Happy Time Difference: " + timeDifference.Days + " days, " + timeDifference.Hours + " hours, " + timeDifference.Minutes + " minutes, " + timeDifference.Seconds + " seconds");

        lastTimeFeed = pet.lastTimeFeed;
        Debug.Log("Feed Time Span - " + (DateTime.Now - lastTimeFeed).TotalHours);

        lastTimeClean = pet.lastTimeClean;
        Debug.Log("Clean Time Span - " + (DateTime.Now - lastTimeClean).TotalHours);

        happiness = pet.happiness;
        happyFillSlider.DOValue(happiness, 0.5f);

        hunger = pet.hunger;
        hungerFillSlider.DOValue(hunger, 0.5f);

        cleanliness = pet.cleanliness;
        cleanFillSlider.DOValue(cleanliness, 0.5f);

        energy = pet.energy;
        energyFillSlider.DOValue(energy, 0.5f);
    }
}
