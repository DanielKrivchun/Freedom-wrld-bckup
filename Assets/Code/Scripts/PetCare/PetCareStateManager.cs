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
    [Header("Health")]
    /// <summary>
    ///  When Health < 60% (390/650), then it is in a state of "Sick"
    ///  sickHealthThreshold = 390
    /// </summary> 
    public int sickHealthThreshold;
    /// <summary>
    ///  If Health reaches zero (ie, Cleanliness == Happiness == Hunger == Energy == 0) => (Health = 0) for 8 hours straight, Pet will pass away.
    ///  lowerHealthTimeInHour = 8
    /// </summary> 
    public float lowerHealthTimeInHour;

    [Space]
    [Header("Happiness")]
    public int maxHappiness;
    /// <summary>
    ///  Happy lasts 2/3 of a real day so time = 16 hours = 960 minutes
    ///  So after 9.6 minutes User lost 1 happiness
    ///  happyTimeLength = 9.6 minutes = 576 seconds
    /// </summary>
    public float happyTimeLength;
    // TickRate is the value which will reduce after Time Length is over
    public int happinessTickRate;


    [Space]
    [Header("Hunger")]
    public int maxHunger;
    /// <summary>
    ///  Hunger lasts 1/3 day of a real day so time = 8 hours = 480 minutes
    ///  So after 4.8 minutes User lost 1 hunger
    ///  hungerTimeLength = 4.8 minutes = 288 seconds
    /// </summary>
    public float hungerTimeLength;
    // TickRate is the value which will reduce after Time Length is over
    public int hungerTickRate;
    [Space]
    /// <summary>
    ///  If Hunger <= 10/100 for more than 7 days straight, Pet will pass away.
    ///  lowHungerThreshold = 10 
    ///  lowerHungerTimeInDay = 7
    /// </summary> 
    public int lowHungerThreshold;
    public float lowerHungerTimeInDay;


    [Space]
    [Header("Cleanliness")]
    public int maxCleanliness;
    /// <summary>
    ///  Cleanliness lasts 1/1 day of a real day so time = 24 hours = 1440 minutes
    ///  So after 14.4 minutes User lost 1 cleanliness
    ///  cleanTimeLength = 14.4 minutes = 864 seconds
    /// </summary>
    public float cleanTimeLength;
    // TickRate is the value which will reduce after Time Length is over
    public int cleanlinessTickRate;
    [Space]
    /// <summary>
    ///  If Cleanliness <= 10/100 for more than 21 days straight, Pet will pass away.
    ///  lowCleanlinessThreshold = 10
    ///  lowerCleanlinessTimeInDay = 21
    /// </summary> 
    public int lowCleanlinessThreshold;
    public float lowerCleanlinessTimeInDay;


    [Space]
    [Header("Energy & Sleep")]
    public int maxEnergy;
    /// <summary>
    ///  Sleep(Energy) lasts 1/2 day of a real day so time = 12 hours = 720 minutes
    ///  So after 7.2 minutes User lost 1 energy
    ///  energyTimeLength = 7.2 minutes = 432 seconds
    /// </summary>
    public float energyTimeLength;
    // TickRate is the value which will reduce after Time Length is over
    public int energyTickRate;
    [Space]
    /// <summary>
    ///  If the pet has not slept for 10 days straight, Pet will pass away.
    ///  lowerSleepTimeInDay = 10
    /// </summary> 
    public float lowerSleepTimeInDay;

    [Space(25)]
    public SleepManager sleepManager;
    public TimingManager timingManager;

    Vector3 startPos;

    // Total seconds of 1 day for days calculation
    public static float oneDaySeconds = 86400f;
    // Total seconds of 1 hour for hours calculation
    public static float oneHourSeconds = 3600f;

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

        if(!petDataRef.petData.isSick && petDataRef.petData.health < sickHealthThreshold)
        {
            petDataRef.petData.isSick = true;
        }

        if (petDataRef.petData.isSick)
        {
            Mathf.Clamp(petDataRef.petData.health, 0, sickHealthThreshold);
        }
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

    //Get & set data from Beamable
    public void SetDataOfCloudAndManageStats()
    {
        //Happiness
        int lostHappiness = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeHappy)).TotalSeconds / happyTimeLength)) * happinessTickRate;
        Debug.Log("Lost Happiness - " + lostHappiness);
        ManageHappinessDataFiller(-lostHappiness);


        //Feed
        int lostHunger = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeFeed)).TotalSeconds / hungerTimeLength)) * hungerTickRate;
        Debug.Log("Lost Hunger - " + lostHunger);

        //Checking Pet Death Situation
        if (petDataRef.petData.hunger - lostHunger <= lowHungerThreshold)
        {
            if (IsPetDeathDueToHunger())
            {
                //Uncomment this return statement because the pet has died, so there is no need to check further anything
                //return;
            }
        }
        ManageHungerDataFiller(-lostHunger);


        //Cleanliness
        int lostCleanliness = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeClean)).TotalSeconds / cleanTimeLength)) * cleanlinessTickRate;
        Debug.Log("Lost Cleanliness - " + lostCleanliness);

        //Checking Pet Death Situation
        if (petDataRef.petData.cleanliness - lostCleanliness <= lowCleanlinessThreshold)
        {
            if (IsPetDeathDueToCleanliness())
            {
                //Uncomment this return statement because the pet has died, so there is no need to check further anything
                //return;
            }
        }
        ManageCleanlinessDataFiller(-lostCleanliness);

        //Checking Pet Death Situation
        float noSleepTime = (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeEnergy)).TotalSeconds;
        if ((noSleepTime / oneDaySeconds) > lowerSleepTimeInDay
            && !petDataRef.petData.isSleeping)
        {
            Debug.Log("Pet die due to NO SLEEP! " + (noSleepTime / oneDaySeconds));

            //Uncomment this return statement because the pet has died, so there is no need to check further anything
            //return;
        }

        //Energy
        int lostEnergy = ((int)((DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeEnergy)).TotalSeconds / energyTimeLength)) * energyTickRate;
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
            if(IsPetDeathDueToHealth(lostHappiness, lostHunger, lostCleanliness, lostEnergy))
            {
                //Uncomment this return statement because the pet has died, so there is no need to check further anything
                //return;
            }
        }
    }

    public bool IsPetDeathDueToHunger()
    {
        int lowHunger;
        float timeBeforeLowLevelHunger = 0f;

        //Checking for Hunger time after Hunger <= 10
        if (petDataRef.petData.hunger > lowHungerThreshold)
        {
            lowHunger = Mathf.Abs(petDataRef.petData.hunger - 10);
        }
        else
        {
            lowHunger = 0;
        }

        while (lowHunger > 0)
        {
            timeBeforeLowLevelHunger += hungerTimeLength;
            lowHunger -= hungerTickRate;
        }

        float timeOfLowHunger = (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeFeed)).TotalSeconds - timeBeforeLowLevelHunger;

        //Checking time is passing LowerHungerTimeInDay limit
        if ((timeOfLowHunger / oneDaySeconds) > lowerHungerTimeInDay)
        {
            Debug.Log("Pet die due to NO FOOD!" + (timeOfLowHunger / oneDaySeconds));
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPetDeathDueToCleanliness()
    {
        int lowCleanliness;
        float timeBeforeLowerCleanliness = 0f;

        //Checking for Cleanliness time after Hunger <= 10
        if (petDataRef.petData.cleanliness > lowCleanlinessThreshold)
        {
            lowCleanliness = Mathf.Abs(petDataRef.petData.cleanliness - 10);
        }
        else
        {
            lowCleanliness = 0;
        }

        while (lowCleanliness > 0)
        {
            timeBeforeLowerCleanliness += cleanTimeLength;
            lowCleanliness -= cleanlinessTickRate;
        }

        float timeOfLowerCleanliness = (float)(DateTime.Now - DateTime.Parse(petDataRef.petData.lastTimeClean)).TotalSeconds - timeBeforeLowerCleanliness;
        Debug.Log("timeOfLowerCleanliness in Day - " + TimeSpan.FromDays(timeOfLowerCleanliness));

        //Checking time is passing LowerCleanlinessTimeInDay limit
        if ((timeOfLowerCleanliness / oneDaySeconds) > lowerCleanlinessTimeInDay)
        {
            Debug.Log("Pet die due to NO CLEANLINESS!" + (timeOfLowerCleanliness / oneDaySeconds));
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPetDeathDueToHealth(int lostHappiness, int lostHunger, int lostCleanliness, int lostEnergy)
    {
        float timeOfLowerHappiness = 0f;
        float timeOfLowerHunger = 0f;
        float timeOfLowerCleanliness = 0f;
        float timeOfLowerEnergy = 0f;

        //Checking for Happiness time after Happiness = 0
        int lowerHappiness = lostHappiness - maxHappiness;
        while (lowerHappiness > 0)
        {
            timeOfLowerHappiness += happyTimeLength;
            lowerHappiness -= happinessTickRate;
        }

        //Checking for Hunger time after Hunger = 0
        int lowerHunger = lostHunger - maxHunger;
        while (lowerHunger > 0)
        {
            timeOfLowerHunger += hungerTimeLength;
            lowerHunger -= hungerTickRate;
        }

        //Checking for Cleanliness time after Cleanliness = 0
        int lowerCleanliness = lostCleanliness - maxCleanliness;
        while (lowerCleanliness > 0)
        {
            timeOfLowerCleanliness += cleanTimeLength;
            lowerCleanliness -= cleanlinessTickRate;
        }

        //Checking for Energy time after Energy = 0
        int lowerEnergy = lostEnergy - maxEnergy;
        while (lowerEnergy > 0)
        {
            timeOfLowerEnergy += energyTimeLength;
            lowerEnergy -= energyTickRate;
        }

        //Checking all times are passing LowerHealthTimeInHour limit
        if ((timeOfLowerHappiness / oneHourSeconds) > lowerHealthTimeInHour && (timeOfLowerHunger / oneHourSeconds) > lowerHealthTimeInHour
            && (timeOfLowerCleanliness / oneHourSeconds) > lowerHealthTimeInHour && (timeOfLowerEnergy / oneHourSeconds) > lowerHealthTimeInHour)
        {
            Debug.Log("Pet die due to NO HEALTH!" + ", Hap-" + (timeOfLowerHappiness / oneHourSeconds) + ", Hun-" + (timeOfLowerHunger / oneHourSeconds) + ", Cle-" + (timeOfLowerCleanliness / oneHourSeconds) + ", Ene-" + (timeOfLowerEnergy / oneHourSeconds));
            return true;
        }
        else
        {
            return false;
        }
    }
}
