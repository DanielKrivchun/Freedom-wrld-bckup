using DG.Tweening;
using System;
using UnityEngine;

public class PetCareStateManager : MonoBehaviour
{
    public PetCareData petCareData;
    public bool isDataFromJSON;

    [Space]
    public PetCareState selectedPetCareState;
    public PetCareUIManager petCareUIManager;

    [Space]
    public GameObject player;

    [Space]
    public GameObject eatObjectHolder;
    public GameObject bathObjectHolder;

    public bool isReadyForBath;

    Vector3 startPos;

    [Space]
    [Header("Pet Care Data")]
    public int happiness;
    public int hunger;
    public int cleanliness;
    public int energy;

    [Space]
    public int happinessTickRate;
    public int feedTickRate;
    public int cleanTickRate;
    public int energyTickRate;

    public DateTime lastTimeFeed, lastTimeHappy, lastTimeClean;

    public TimingManager timingManager;


    private void Start()
    {
        startPos = player.transform.position;

        if (!isDataFromJSON)
        {
            GetAndSetAllPetCareData();
        }
        else
        {
            GetAndSetAllPetCareDataJSON();
        }
    }

    public void CheckForSelectedPetCareState(PetCareState state)
    {
        selectedPetCareState = state;
        player.transform.position = startPos;

        if (selectedPetCareState == PetCareState.Feed)
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

    public void SetSelectedStateDataFiller(int value)
    {
        switch (selectedPetCareState)
        {
            case PetCareState.Happy:
                happiness += value;
                if (happiness > 100)
                {
                    happiness = 100;  
                }

                petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);
                lastTimeHappy = DateTime.Now;
                break;

            case PetCareState.Feed:
                hunger += value;
                if (hunger > 100)
                {
                    hunger = 100;
                }

                petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);
                lastTimeFeed = DateTime.Now;
                break;

            case PetCareState.Clean:
                cleanliness += value;
                if (cleanliness > 100)
                {
                    cleanliness = 100;   
                }

                petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);
                lastTimeClean = DateTime.Now;
                break;
        }
    }

    public void CheckForPetCareTimer(PetCareState state)
    {
        switch (state)
        {
            case PetCareState.Happy:
                happiness -= happinessTickRate;
                if (happiness < 0)
                {
                    happiness = 0;
                }
                petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);

                break;

            case PetCareState.Feed:
                hunger -= feedTickRate;
                if (hunger < 0)
                {
                    hunger = 0;
                }
                petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);

                break;

            case PetCareState.Clean:
                cleanliness -= cleanTickRate;
                if (cleanliness < 0)
                {
                    cleanliness = 0;
                }
                petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);
                break;
        }
    }

    //Store Data
    public void StoreAllPetCareData()
    {
        if (!isDataFromJSON)
        {
            Debug.Log("Setting Data");
            petCareData.SetPetCareData(lastTimeHappy.ToString(), lastTimeFeed.ToString(), lastTimeClean.ToString(), happiness, hunger, cleanliness, energy);
        }
        else
        {
            Pet pet = new Pet(lastTimeHappy.ToString(), lastTimeFeed.ToString(), lastTimeClean.ToString(), happiness, hunger, cleanliness, energy);
            DatabaseManager.instance.SavePet(pet);
        }
    }


    //Scriptable Object Data
    public void GetAndSetAllPetCareData()
    {
        //Happiness
        lastTimeHappy = DateTime.Parse(petCareData.lastTimeHappy);
        Debug.Log("Happy Time Difference Seconds: " + (DateTime.Now - lastTimeHappy).TotalSeconds);

        if ((DateTime.Now - lastTimeHappy).TotalSeconds > timingManager.happyTimeLength)
        {
            Debug.Log("Happiness => 0");
        }
        else
        {
            Debug.Log("Calculate Happiness");
        }
        

        happiness = petCareData.happiness;
        petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);


        //Hunger
        lastTimeFeed = DateTime.Parse(petCareData.lastTimeFeed);
        TimeSpan feedTimeDiff = DateTime.Now - lastTimeFeed;
        Debug.Log("Feed Time Difference: " + feedTimeDiff.Days + " D, " + feedTimeDiff.Hours + " H, " + feedTimeDiff.Minutes + " M, " + feedTimeDiff.Seconds + " S");

        hunger = petCareData.hunger;
        petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);


        //Cleanliness
        lastTimeClean = DateTime.Parse(petCareData.lastTimeClean);
        TimeSpan cleanTimeDiff = DateTime.Now - lastTimeClean;
        Debug.Log("Clean Time Difference: " + cleanTimeDiff.Days + " D, " + cleanTimeDiff.Hours + " H, " + cleanTimeDiff.Minutes + " M, " + cleanTimeDiff.Seconds + " S");

        cleanliness = petCareData.cleanliness;
        petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);


        //Energy
        energy = petCareData.energy;
        petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
    }


    //Local Data (JSON)
    public void GetAndSetAllPetCareDataJSON()
    {
        Pet petCareData = DatabaseManager.instance.LoadPet();

        //Happiness
        lastTimeHappy = DateTime.Parse(petCareData.lastTimeHappy);
        TimeSpan happyTimeDiff = DateTime.Now - lastTimeHappy;
        Debug.Log("Happy Time Difference: " + happyTimeDiff.Days + " D, " + happyTimeDiff.Hours + " H, " + happyTimeDiff.Minutes + " M, " + happyTimeDiff.Seconds + " S");

        happiness = petCareData.happiness;
        petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);


        //Feed
        lastTimeFeed = DateTime.Parse(petCareData.lastTimeFeed);
        TimeSpan feedTimeDiff = DateTime.Now - lastTimeFeed;
        Debug.Log("Feed Time Difference: " + feedTimeDiff.Days + " D, " + feedTimeDiff.Hours + " H, " + feedTimeDiff.Minutes + " M, " + feedTimeDiff.Seconds + " S");

        hunger = petCareData.hunger;
        petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);


        //Cleanliness
        lastTimeClean = DateTime.Parse(petCareData.lastTimeClean);
        TimeSpan cleanTimeDiff = DateTime.Now - lastTimeClean;
        Debug.Log("Clean Time Difference: " + cleanTimeDiff.Days + " D, " + cleanTimeDiff.Hours + " H, " + cleanTimeDiff.Minutes + " M, " + cleanTimeDiff.Seconds + " S");

        cleanliness = petCareData.cleanliness;
        petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);


        //Energy
        energy = petCareData.energy;
        petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
    }
}
