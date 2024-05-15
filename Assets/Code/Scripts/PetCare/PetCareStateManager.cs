using Beamable.CloudSavingService;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PetCareStateManager : MonoBehaviour
{
    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

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

    [Space]
    public int happinessTickRate;
    public int hungerTickRate;
    public int cleanlinessTickRate;
    public int energyTickRate;

    [Space]
    public int lowHungerRate;
    public float lowHungerTimeLimit;

    [Space]
    public int lowCleanlinessRate;
    public float lowCleanlinessTimeLimit;

    [Space]
    public float lowSleepTimeLimit;
    public float lowHealthTimeLimit;

    [Space]
    public SleepManager sleepManager;
    public TimingManager timingManager;

    Vector3 startPos;

    private void Start()
    {
        startPos = player.transform.position;
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

    public void UpdateHealth()
    {
        petDataRef.petData.health = (2 * petDataRef.petData.happiness) + (2 * petDataRef.petData.cleanliness) + (int)(1.5 * petDataRef.petData.hunger) + petDataRef.petData.energy;
    }

    public void ManageHappinessDataFiller(int value)
    {
        petDataRef.petData.happiness += value;
        if (petDataRef.petData.happiness > 100)
        {
            petDataRef.petData.happiness = 100;
        }
        else if (petDataRef.petData.happiness < 0)
        {
            petDataRef.petData.happiness = 0;
        }

        petCareUIManager.happyFillSlider.DOValue(petDataRef.petData.happiness, 0.5f);
        petDataRef.petData.lastTimeHappy = DateTime.Now.ToString();

        UpdateHealth();
    }

    public void ManageHungerDataFiller(int value)
    {
        petDataRef.petData.hunger += value;
        if (petDataRef.petData.hunger > 100)
        {
            petDataRef.petData.hunger = 100;
        }
        else if (petDataRef.petData.hunger < 0)
        {
            petDataRef.petData.hunger = 0;
        }

        petCareUIManager.hungerFillSlider.DOValue(petDataRef.petData.hunger, 0.5f);
        petDataRef.petData.lastTimeFeed = DateTime.Now.ToString();

        UpdateHealth();
    }

    public void ManageCleanlinessDataFiller(int value)
    {
        petDataRef.petData.cleanliness += value;
        if (petDataRef.petData.cleanliness > 100)
        {
            petDataRef.petData.cleanliness = 100;
        }
        else if (petDataRef.petData.cleanliness < 0)
        {
            petDataRef.petData.cleanliness = 0;
        }

        petCareUIManager.cleanFillSlider.DOValue(petDataRef.petData.cleanliness, 0.5f);
        petDataRef.petData.lastTimeClean = DateTime.Now.ToString();

        UpdateHealth();
    }

    public void ManageEnergyDataFiller(int value)
    {
        petDataRef.petData.energy += value;
        if (petDataRef.petData.energy > 100)
        {
            petDataRef.petData.energy = 100;
        }
        else if (petDataRef.petData.energy < 0)
        {
            petDataRef.petData.energy = 0;
        }

        petCareUIManager.energyFillSlider.DOValue(petDataRef.petData.energy, 0.5f);
        petDataRef.petData.lastTimeEnergy = DateTime.Now.ToString();

        UpdateHealth();
    }

    /*//Store Data to Beamable
    public void StoreAllPetCareData()
    {
        BeamableCloudSaveManager.instance.SaveData(petDataRef.petData);
    }*/

    //Get & set data from Beamable
    public void SetDataOfCloudAndManageStats()
    {
        //Happiness
        int lostHappiness = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeHappy)).TotalSeconds / timingManager.happyTimeLength)) * happinessTickRate;
        Debug.Log("Lost Happiness - " + lostHappiness);
        ManageHappinessDataFiller(-lostHappiness);


        //Feed
        int lostHunger = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeFeed)).TotalSeconds / timingManager.feedTimeLength)) * hungerTickRate;
        Debug.Log("Lost Hunger - " + lostHunger);

        //Checking Pet Death Situation
        if (petDataRef.petData.hunger - lostHunger <= lowHungerRate)
        {
            CheckForPetDeathDueToHunger();
        }
        ManageHungerDataFiller(-lostHunger);


        //Cleanliness
        int lostCleanliness = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeClean)).TotalSeconds / timingManager.cleanTimeLength)) * cleanlinessTickRate;
        Debug.Log("Lost Cleanliness - " + lostCleanliness);

        //Checking Pet Death Situation
        if (petDataRef.petData.cleanliness - lostCleanliness <= lowCleanlinessRate)
        {
            CheckForPetDeathDueToCleanliness();
        }
        ManageCleanlinessDataFiller(-lostCleanliness);

        //Checking Pet Death Situation
        if ((float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeEnergy)).TotalSeconds > lowSleepTimeLimit
            && !petDataRef.petData.isSleeping)
        {
            Debug.Log("Pet die due to NO SLEEP! " + (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeEnergy)).TotalSeconds);
            return;
        }

        //Energy
        int lostEnergy = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeEnergy)).TotalSeconds / timingManager.energyTimeLength)) * energyTickRate;
        Debug.Log("Lost Energy - " + lostEnergy);

        ManageEnergyDataFiller(-lostEnergy);

        //Sleep
        if (petDataRef.petData.isSleeping)
        {
            if ((DateTime.Now - DateTime.Parse(petDataRef.petData.sleepStartTime)).TotalSeconds > sleepManager.totalSleepTime)
            {
                Debug.Log("Sleep time over");
                ManageEnergyDataFiller(100);
            }
            else
            {
                Debug.Log("Sleep Time is Not over yet");
                sleepManager.sleepTimer = sleepManager.totalSleepTime - (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.sleepStartTime)).TotalSeconds;
                sleepManager.isCanSleep = true;
            }
        }

        //Checking Pet Death Situation
        if (petDataRef.petData.happiness == 0 && petDataRef.petData.hunger == 0 && petDataRef.petData.cleanliness == 0 && petDataRef.petData.energy == 0)
        {
            CheckForPetDeathDueToHealth(lostHappiness, lostHunger, lostCleanliness, lostEnergy);
        }
    }

    public void CheckForPetDeathDueToHunger()
    {
        int lowHunger;
        float timeBeforeLowLevelHunger = 0f;

        if (petDataRef.petData.hunger > lowHungerRate)
        {
            lowHunger = Mathf.Abs(petDataRef.petData.hunger - 10);
        }
        else
        {
            lowHunger = 0;
        }

        while (lowHunger > 0)
        {
            timeBeforeLowLevelHunger += timingManager.feedTimeLength;
            lowHunger -= hungerTickRate;
        }

        float timeOfLowHunger = (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeFeed)).TotalSeconds - timeBeforeLowLevelHunger;

        if (timeOfLowHunger > lowHungerTimeLimit)
        {
            Debug.Log("Pet die due to NO FOOD!" + timeOfLowHunger);
        }
    }

    public void CheckForPetDeathDueToCleanliness()
    {
        int lowCleanliness;
        float timeBeforeLowerCleanliness = 0f;

        if (petDataRef.petData.cleanliness > lowCleanlinessRate)
        {
            lowCleanliness = Mathf.Abs(petDataRef.petData.cleanliness - 10);
        }
        else
        {
            lowCleanliness = 0;
        }

        while (lowCleanliness > 0)
        {
            timeBeforeLowerCleanliness += timingManager.cleanTimeLength;
            lowCleanliness -= cleanlinessTickRate;
        }

        float timeOfLowerCleanliness = (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeClean)).TotalSeconds - timeBeforeLowerCleanliness;

        if (timeOfLowerCleanliness > lowCleanlinessTimeLimit)
        {
            Debug.Log("Pet die due to NO CLEANLINESS!" + timeOfLowerCleanliness);
        }
    }

    public void CheckForPetDeathDueToHealth(int lostHappiness, int lostHunger, int lostCleanliness, int lostEnergy)
    {
        float timeOfLowerHappiness = 0f;
        float timeOfLowerHunger = 0f;
        float timeOfLowerCleanliness = 0f;
        float timeOfLowerEnergy = 0f;

        if ((petDataRef.petData.happiness - lostHappiness) < 0)
        {
            int lowerHappiness = lostHappiness - petDataRef.petData.happiness;

            while (lowerHappiness > 0)
            {
                timeOfLowerHappiness += timingManager.happyTimeLength;
                lowerHappiness -= happinessTickRate;
            }
        }

        if ((petDataRef.petData.hunger - lostHunger) < 0)
        {
            int lowerHunger = lostHunger - petDataRef.petData.hunger;

            while (lowerHunger > 0)
            {
                timeOfLowerHunger += timingManager.feedTimeLength;
                lowerHunger -= hungerTickRate;
            }
        }

        if ((petDataRef.petData.cleanliness - lostCleanliness) < 0)
        {
            int lowerCleanliness = lostCleanliness - petDataRef.petData.cleanliness;

            while (lowerCleanliness > 0)
            {
                timeOfLowerCleanliness += timingManager.cleanTimeLength;
                lowerCleanliness -= cleanlinessTickRate;
            }
        }

        if ((petDataRef.petData.energy - lostEnergy) < 0)
        {
            int lowerEnergy = lostEnergy - petDataRef.petData.energy;

            while (lowerEnergy > 0)
            {
                timeOfLowerEnergy += timingManager.energyTimeLength;
                lowerEnergy -= energyTickRate;
            }
        }

        if(timeOfLowerHappiness > lowHealthTimeLimit && timeOfLowerHunger > lowHealthTimeLimit
            && timeOfLowerCleanliness > lowHealthTimeLimit && timeOfLowerEnergy > lowHealthTimeLimit)
        {
            Debug.Log("Pet die due to NO HEALTH!" + ", Hap-" + timeOfLowerHappiness + ", Hun-" + timeOfLowerHunger + ", Cle-" + timeOfLowerCleanliness + ", Ene-" + timeOfLowerEnergy);
        }
    }
}
