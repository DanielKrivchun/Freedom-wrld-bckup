using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject sleepBtn;

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
    public int hungerTickRate;
    public int cleanlinessTickRate;
    public int energyTickRate;

    public DateTime lastTimeFeed, lastTimeHappy, lastTimeClean, lastTimeSleep;

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

        switch (selectedPetCareState)
        {
            case PetCareState.Happy:
                bathObjectHolder.SetActive(false);
                eatObjectHolder.SetActive(false);
                sleepBtn.SetActive(false);
                break;

            case PetCareState.Feed:
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

    public void CheckForPetCareTimer(PetCareState state)
    {
        switch (state)
        {
            case PetCareState.Happy:
                ManageHappinessDataFiller(-happinessTickRate);
                break;

            case PetCareState.Feed:
                ManageHungerDataFiller(-hungerTickRate);
                break;

            case PetCareState.Clean:
                ManageCleanlinessDataFiller(-cleanlinessTickRate);
                break;

            case PetCareState.Energy:
                ManageEnergyDataFiller(-energyTickRate);
                break;
        }
    }

    public void ManageHappinessDataFiller(int value)
    {
        happiness += value;
        if (happiness > 100)
        {
            happiness = 100;
        }
        else if(happiness < 0)
        {
            happiness = 0;
        }

        petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);
        lastTimeHappy = DateTime.Now;
    }

    public void ManageHungerDataFiller(int value)
    {
        hunger += value;
        if (hunger > 100)
        {
            hunger = 100;
        }
        else if(hunger < 0)
        {
            hunger = 0;
        }

        petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);
        lastTimeFeed = DateTime.Now;
    }

    public void ManageCleanlinessDataFiller(int value)
    {
        cleanliness += value;
        if (cleanliness > 100)
        {
            cleanliness = 100;
        }
        else if(cleanliness < 0)
        {
            cleanliness = 0;
        }

        petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);
        lastTimeClean = DateTime.Now;
    }

    public void ManageEnergyDataFiller(int value)
    {
        energy += value;
        if (energy > 100)
        {
            energy = 100;
        }
        else if(energy < 0)
        {
            energy = 0;
        }

        petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
        lastTimeSleep = DateTime.Now;
    }

    //Store Data
    public void StoreAllPetCareData()
    {
        if (!isDataFromJSON)
        {
            petCareData.SetPetCareData(lastTimeHappy.ToString(), lastTimeFeed.ToString(), lastTimeClean.ToString(), lastTimeSleep.ToString(), happiness, hunger, cleanliness, energy);
        }
        else
        {
            Pet pet = new Pet(lastTimeHappy.ToString(), lastTimeFeed.ToString(), lastTimeClean.ToString(), lastTimeSleep.ToString(), happiness, hunger, cleanliness, energy);
            DatabaseManager.instance.SavePet(pet);
        }
    }


    //Scriptable Object Data
    public void GetAndSetAllPetCareData()
    {
        //Happiness
        lastTimeHappy = DateTime.Parse(petCareData.lastTimeHappy);
        happiness = petCareData.happiness;

        int lostHappiness = (int)((DateTime.Now - lastTimeHappy).TotalSeconds / timingManager.happyTimeLength);
        Debug.Log("Lost Happiness - " + lostHappiness);
        ManageHappinessDataFiller(-lostHappiness);


        //Hunger
        lastTimeFeed = DateTime.Parse(petCareData.lastTimeFeed);
        hunger = petCareData.hunger;

        int lostHunger = (int)((DateTime.Now - lastTimeFeed).TotalSeconds / timingManager.feedTimeLength);
        Debug.Log("Lost Hunger - " + lostHunger);
        ManageHungerDataFiller(-lostHunger);


        //Cleanliness
        lastTimeClean = DateTime.Parse(petCareData.lastTimeClean);
        cleanliness = petCareData.cleanliness;

        int lostCleanliness = (int)((DateTime.Now - lastTimeClean).TotalSeconds / timingManager.cleanTimeLength);
        Debug.Log("Lost Cleanliness - " + lostCleanliness);
        ManageCleanlinessDataFiller(-lostCleanliness);


        //Energy
        lastTimeSleep = DateTime.Parse(petCareData.lastTimeSleep);
        energy = petCareData.energy;

        int lostEnergy = (int)((DateTime.Now - lastTimeSleep).TotalSeconds / timingManager.energyTimeLength);
        Debug.Log("Lost Energy - " + lostEnergy);
        ManageEnergyDataFiller(-lostEnergy);
    }


    //Local Data (JSON)
    public void GetAndSetAllPetCareDataJSON()
    {
        Pet petCareData = DatabaseManager.instance.LoadPet();

        //Happiness
        lastTimeHappy = DateTime.Parse(petCareData.lastTimeHappy);
        happiness = petCareData.happiness;

        int lostHappiness = (int)((DateTime.Now - lastTimeHappy).TotalSeconds / timingManager.happyTimeLength);
        Debug.Log("Lost Happiness - " + lostHappiness);
        ManageHappinessDataFiller(-lostHappiness);


        //Feed
        lastTimeFeed = DateTime.Parse(petCareData.lastTimeFeed);
        hunger = petCareData.hunger;

        int lostHunger = (int)((DateTime.Now - lastTimeFeed).TotalSeconds / timingManager.feedTimeLength);
        Debug.Log("Lost Hunger - " + lostHunger);
        ManageHungerDataFiller(-lostHunger);


        //Cleanliness
        lastTimeClean = DateTime.Parse(petCareData.lastTimeClean);
        cleanliness = petCareData.cleanliness;

        int lostCleanliness = (int)((DateTime.Now - lastTimeClean).TotalSeconds / timingManager.cleanTimeLength);
        Debug.Log("Lost Cleanliness - " + lostCleanliness);
        ManageCleanlinessDataFiller(-lostCleanliness);


        //Energy
        lastTimeSleep = DateTime.Parse(petCareData.lastTimeSleep);
        energy = petCareData.energy;

        int lostEnergy = (int)((DateTime.Now - lastTimeSleep).TotalSeconds / timingManager.energyTimeLength);
        Debug.Log("Lost Energy - " + lostEnergy);
        ManageEnergyDataFiller(-lostEnergy);
    }
}
