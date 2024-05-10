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
    public int feedTickRate;
    public int cleanTickRate;
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

            case PetCareState.Energy:
                energy += value;
                if (energy > 100)
                {
                    energy = 100;
                }

                petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
                lastTimeSleep = DateTime.Now;
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
                lastTimeHappy = DateTime.Now;
                break;

            case PetCareState.Feed:
                hunger -= feedTickRate;
                if (hunger < 0)
                {
                    hunger = 0;
                }

                petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);
                lastTimeFeed = DateTime.Now;
                break;

            case PetCareState.Clean:
                cleanliness -= cleanTickRate;
                if (cleanliness < 0)
                {
                    cleanliness = 0;
                }

                petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);
                lastTimeClean = DateTime.Now;
                break;

            case PetCareState.Energy:
                energy -= energyTickRate;
                if (energy < 0)
                {
                    energy = 0;
                }

                petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
                lastTimeSleep = DateTime.Now;
                break;
        }
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
        Debug.Log("Happy Time Difference Seconds: " + (DateTime.Now - lastTimeHappy).TotalSeconds);
        int lostHappiness = (int)((DateTime.Now - lastTimeHappy).TotalSeconds / timingManager.happyTimeLength);
        Debug.Log("Lost Happiness - " + lostHappiness);

        happiness = petCareData.happiness;
        petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);


        //Hunger
        lastTimeFeed = DateTime.Parse(petCareData.lastTimeFeed);
        Debug.Log("Feed Time Difference Seconds: " + (DateTime.Now - lastTimeFeed).TotalSeconds);
        int lostHunger = (int)((DateTime.Now - lastTimeFeed).TotalSeconds / timingManager.feedTimeLength);
        Debug.Log("Lost Hunger - " + lostHunger);

        hunger = petCareData.hunger;
        petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);


        //Cleanliness
        lastTimeClean = DateTime.Parse(petCareData.lastTimeClean);
        Debug.Log("Feed Time Difference Seconds: " + (DateTime.Now - lastTimeClean).TotalSeconds);
        int lostCleanliness = (int)((DateTime.Now - lastTimeClean).TotalSeconds / timingManager.cleanTimeLength);
        Debug.Log("Lost Cleanliness - " + lostCleanliness);

        cleanliness = petCareData.cleanliness;
        petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);


        //Energy
        lastTimeSleep = DateTime.Parse(petCareData.lastTimeSleep);
        Debug.Log("Feed Time Difference Seconds: " + (DateTime.Now - lastTimeSleep).TotalSeconds);
        int lostEnergy = (int)((DateTime.Now - lastTimeSleep).TotalSeconds / timingManager.energyTimeLength);
        Debug.Log("Lost Energy - " + lostEnergy);

        energy = petCareData.energy;
        petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
    }


    //Local Data (JSON)
    public void GetAndSetAllPetCareDataJSON()
    {
        Pet petCareData = DatabaseManager.instance.LoadPet();

        //Happiness
        lastTimeHappy = DateTime.Parse(petCareData.lastTimeHappy);
        Debug.Log("Happy Time Difference Seconds: " + (DateTime.Now - lastTimeHappy).TotalSeconds);
        int lostHappiness = (int)((DateTime.Now - lastTimeHappy).TotalSeconds / timingManager.happyTimeLength);
        Debug.Log("Lost Happiness - " + lostHappiness);

        happiness = petCareData.happiness;
        petCareUIManager.happyFillSlider.DOValue(happiness, 0.5f);


        //Feed
        lastTimeFeed = DateTime.Parse(petCareData.lastTimeFeed);
        Debug.Log("Feed Time Difference Seconds: " + (DateTime.Now - lastTimeFeed).TotalSeconds);
        int lostHunger = (int)((DateTime.Now - lastTimeFeed).TotalSeconds / timingManager.feedTimeLength);
        Debug.Log("Lost Hunger - " + lostHunger);

        hunger = petCareData.hunger;
        petCareUIManager.hungerFillSlider.DOValue(hunger, 0.5f);


        //Cleanliness
        lastTimeClean = DateTime.Parse(petCareData.lastTimeClean);
        Debug.Log("Feed Time Difference Seconds: " + (DateTime.Now - lastTimeClean).TotalSeconds);
        int lostCleanliness = (int)((DateTime.Now - lastTimeClean).TotalSeconds / timingManager.cleanTimeLength);
        Debug.Log("Lost Cleanliness - " + lostCleanliness);

        cleanliness = petCareData.cleanliness;
        petCareUIManager.cleanFillSlider.DOValue(cleanliness, 0.5f);


        //Energy
        lastTimeSleep = DateTime.Parse(petCareData.lastTimeSleep);
        Debug.Log("Feed Time Difference Seconds: " + (DateTime.Now - lastTimeSleep).TotalSeconds);
        int lostEnergy = (int)((DateTime.Now - lastTimeSleep).TotalSeconds / timingManager.energyTimeLength);
        Debug.Log("Lost Energy - " + lostEnergy);

        energy = petCareData.energy;
        petCareUIManager.energyFillSlider.DOValue(energy, 0.5f);
    }
}
