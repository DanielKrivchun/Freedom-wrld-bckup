using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PetCareStateManager : MonoBehaviour
{
    public static PetCareStateManager instance;

    [Space]
    public SimpleGameEvent offLoadingCanvasEvent;

    [Header("Pet Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public PetCareState selectedPetCareState;
    public PetCareUIManager petCareUIManager;

    [Space]
    public GameObject player;

    [Space]
    public PetCareObjectManager petCareObjectManager;

    [Space]
    public FoodObjectHolder foodObjectHolder;
    public List<Transform> foodSpawnTransforms;

    [HideInInspector]
    public bool isReadyForBath;

    [Header("Pet Stat Data")]
    public PetCareStatData petStatData;

    [Space(25)]
    public SleepManager sleepManager;
    public TimingManager timingManager;
    public PetTrainingManager petTrainingManager;

    Vector3 startPos;

    // Total seconds of 1 day for days calculation
    static float oneDaySeconds = 86400f;
    // Total seconds of 1 hour for hours calculation
    static float oneHourSeconds = 3600f;

    private bool isFoodItemsSet = false;
    private int foodSpawnIndex = 0;

    private GameObject generatedFood;
    private List<GameObject> generatedFoodItems = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(instance);
        }
    }

    private void Start()
    {
        startPos = player.transform.position;

        foodObjectHolder.ResetFoodObjects();
    }

    #region PET CARE EVENTS
    //Setting current pet care state and managing stat objects
    public void CheckForSelectedPetCareState(PetCareState state)
    {
        selectedPetCareState = state;
        player.transform.position = startPos;

        petCareObjectManager.ManagePetCareObjects(selectedPetCareState);
    }

    //Update Stats value on Time length over
    public void CheckForPetCareTimer(PetCareState state)
    {
        switch (state)
        {
            case PetCareState.Happy:
                ManageHappinessDataFiller(-petStatData.happinessTickRate);
                break;

            case PetCareState.Eat:
                ManageHungerDataFiller(-petStatData.hungerTickRate);
                break;

            case PetCareState.Clean:
                ManageCleanlinessDataFiller(-petStatData.cleanlinessTickRate);
                break;

            case PetCareState.Energy:
                ManageEnergyDataFiller(-petStatData.energyTickRate);
                break;
        }
    }
    #endregion

    #region PET FOOD ITEMS MANAGEMENT
    //Setting food item on table
    public void SetAvailabeFoodItemOnTable()
    {
        //Checking if food item not available on table then set food item from Petdata
        if (!isFoodItemsSet)
        {
            for (int i = 0; i < petDataRef.petData.foodData.Count; i++)
            {
                GenerateFoodAndSetTransformWithSpawnIndex(petDataRef.petData.foodData[i].foodName);
                isFoodItemsSet = true;
            }
        }
    }

    //Generating food item on table
    public void GenerateFoodItemOnTable(FoodItems foodType)
    {
        GenerateFoodAndSetTransformWithSpawnIndex(foodType);
        isFoodItemsSet = true;

        //Adding food item to Petdata
        PetFoodData petFoodData = new PetFoodData
        {
            foodName = foodType,
            foodCount = 1
        };
        petDataRef.petData.foodData.Add(petFoodData);
    }

    void GenerateFoodAndSetTransformWithSpawnIndex(FoodItems foodType)
    {
        generatedFood = foodObjectHolder.GetMyFood(foodType);
        generatedFood.transform.SetParent(foodSpawnTransforms[foodSpawnIndex]);
        generatedFood.transform.localPosition = Vector3.zero;
        generatedFoodItems.Add(generatedFood);
        generatedFood.GetComponent<EatObject>().foodSpawnIndex = foodSpawnIndex;

        //Setting food spawn index and setting isFoodItemsSet bool to true
        foodSpawnIndex++;
        if (foodSpawnIndex >= foodSpawnTransforms.Count)
        {
            foodSpawnIndex = 0;
        }
    }

    //Removing used food item from Petdata
    public void RemoveFoodFromTable(int foodSpawnIndex)
    {
        petDataRef.petData.foodData.RemoveAt(foodSpawnIndex);
        generatedFoodItems.RemoveAt(foodSpawnIndex);
        StartCoroutine(RearrangeFoodObjectsOnTable(foodSpawnIndex));
    }

    IEnumerator RearrangeFoodObjectsOnTable(int removedIndex)
    {
        yield return new WaitForSeconds(0.5f);

        foodSpawnIndex = removedIndex;
        for (int i = removedIndex; i < petDataRef.petData.foodData.Count; i++)
        {
            generatedFoodItems[i].transform.SetParent(foodSpawnTransforms[foodSpawnIndex]);
            generatedFoodItems[i].transform.localPosition = Vector3.zero;

            //Setting food spawn index and setting isFoodItemsSet bool to true
            foodSpawnIndex++;
            if (foodSpawnIndex >= foodSpawnTransforms.Count)
            {
                foodSpawnIndex = 0;
            }
        }
    }
    #endregion

    #region PET HEALTH
    //Update Pet from Sick to Healthy on use of Medicine
    public void UpdatePetSickToHealthy()
    {
        if (petDataRef.petData.isSick)
        {
            petDataRef.petData.isSick = false;
            petCareUIManager.ClosePetSickLabel();
            petCareUIManager.ShowNotificationUI("Nice job! \nYour pet is all better");
        }
    }

    //Health
    public void UpdateHealth()
    {
        petDataRef.petData.health = (2 * petDataRef.petData.happiness) + (2 * petDataRef.petData.cleanliness) + (int)(1.5 * petDataRef.petData.hunger) + petDataRef.petData.energy;

        //If health is lower than sick thresold then set pet to sick
        if (!petDataRef.petData.isSick && petDataRef.petData.health < petStatData.sickHealthThreshold)
        {
            petDataRef.petData.isSick = true;
            petCareUIManager.ShowPetSickLabel();
        }

        //Pet is sick so clamp health to sick thresold
        if (petDataRef.petData.isSick)
        {
            petCareUIManager.ShowPetSickLabel();
            petDataRef.petData.health = Mathf.Clamp(petDataRef.petData.health, 0, petStatData.sickHealthThreshold);
        }
    }
    #endregion

    #region WELLBEING STAT AND FILLER UPDATE
    //Happiness stat value and filler
    public void ManageHappinessDataFiller(int value)
    {
        petDataRef.petData.happiness = Mathf.Clamp(petDataRef.petData.happiness + value, 0, 100);
        petCareUIManager.happyFillSlider.DOValue(petDataRef.petData.happiness, 0.5f);
        petDataRef.petData.lastTimeHappy = DateTime.UtcNow.ToString();

        UpdateHealth();
    }

    //Hunger stat value and filler
    public void ManageHungerDataFiller(int value)
    {
        petDataRef.petData.hunger = Mathf.Clamp(petDataRef.petData.hunger + value, 0, 100);
        petCareUIManager.hungerFillSlider.DOValue(petDataRef.petData.hunger, 0.5f);
        petDataRef.petData.lastTimeFeed = DateTime.UtcNow.ToString();

        UpdateHealth();
    }

    //Cleanliness stat value and filler
    public void ManageCleanlinessDataFiller(int value)
    {
        petDataRef.petData.cleanliness = Mathf.Clamp(petDataRef.petData.cleanliness + value, 0, 100);
        petCareUIManager.cleanFillSlider.DOValue(petDataRef.petData.cleanliness, 0.5f);
        petDataRef.petData.lastTimeClean = DateTime.UtcNow.ToString();

        UpdateHealth();
    }

    //Energy stat value and filler
    public void ManageEnergyDataFiller(int value)
    {
        petDataRef.petData.energy = Mathf.Clamp(petDataRef.petData.energy + value, 0, 100);
        petCareUIManager.energyFillSlider.DOValue(petDataRef.petData.energy, 0.5f);
        petDataRef.petData.lastTimeEnergy = DateTime.UtcNow.ToString();

        UpdateHealth();
    }
    #endregion

    #region CHECKING LOST VALUE OF STATS
    //Get & set data from Beamable
    public void SetDataOfCloudAndManageStats()
    {
        timingManager.gameObject.SetActive(true);

        //Happiness
        int lostHappiness = CalculateLostStatValue(petDataRef.petData.lastTimeHappy, petStatData.happyTimeLength, petStatData.happinessTickRate);
        Debug.Log("Lost Happiness - " + lostHappiness);
        ManageHappinessDataFiller(-lostHappiness);


        //Feed
        int lostHunger = CalculateLostStatValue(petDataRef.petData.lastTimeFeed, petStatData.hungerTimeLength, petStatData.hungerTickRate);
        Debug.Log("Lost Hunger - " + lostHunger);

        //Checking Pet Death Situation
        if ((petDataRef.petData.hunger - lostHunger) <= petStatData.lowHungerThreshold)
        {
            if (IsPetDeathDueToHunger())
            {
                return;
            }
        }
        ManageHungerDataFiller(-lostHunger);


        //Cleanliness
        int lostCleanliness = CalculateLostStatValue(petDataRef.petData.lastTimeClean, petStatData.cleanTimeLength, petStatData.cleanlinessTickRate);
        Debug.Log("Lost Cleanliness - " + lostCleanliness);

        //Checking Pet Death Situation
        if ((petDataRef.petData.cleanliness - lostCleanliness) <= petStatData.lowCleanlinessThreshold)
        {
            if (IsPetDeathDueToCleanliness())
            {
                return;
            }
        }
        ManageCleanlinessDataFiller(-lostCleanliness);

        //Checking Pet Death Situation
        float noSleepTime = CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.lastTimeEnergy);

        if ((noSleepTime / oneDaySeconds) > petStatData.lowerSleepTimeInDay && !petDataRef.petData.sleepData.isSleeping)
        {
            petCareUIManager.ShowPetDeathUI("Pet die due to NO SLEEP more than " + petStatData.lowerSleepTimeInDay + " days!");
            return;
        }

        //Energy
        int lostEnergy = CalculateLostStatValue(petDataRef.petData.lastTimeEnergy, petStatData.energyTimeLength, petStatData.energyTickRate);
        Debug.Log("Lost Energy - " + lostEnergy);

        ManageEnergyDataFiller(-lostEnergy);

        //Sleep
        if (petDataRef.petData.sleepData.isSleeping)
        {
            if (CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.sleepData.sleepStartTime) > sleepManager.totalSleepTime)
            {
                Debug.Log("Sleep time over");
                ManageEnergyDataFiller(100);
            }
            else
            {
                Debug.Log("Sleep Time is Not over yet");
                sleepManager.sleepTimer = sleepManager.totalSleepTime - CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.sleepData.sleepStartTime);
                sleepManager.isCanSleep = true;

                PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Sleep);
            }
        }

        //Checking Pet Death Situation
        if (petDataRef.petData.happiness == 0 && petDataRef.petData.hunger == 0 && petDataRef.petData.cleanliness == 0 && petDataRef.petData.energy == 0)
        {
            if (IsPetDeathDueToHealth(lostHappiness, lostHunger, lostCleanliness, lostEnergy))
            {
                return;
            }
        }

        //If there is no death of Pet then Spawn pet prefab
        petCareUIManager.SpawnPetPrefab();

        //Check for last login
        if (IsUserLoginNewDay())
        {
            Debug.Log("UserLogin New Day!");
            List<string> tempMsglist = new List<string>();

            if (!petDataRef.petData.isSick)
            {
                //Flu
                tempMsglist.Add(PetGettingSickWithFluChance());
            }

            //Treasure Hunt
            tempMsglist.Add(PetGettingCoinsWithTreasureHuntChance());

            petCareUIManager.ManageNotificationMsg(tempMsglist);

            //Set Last Login Time to current
            petDataRef.petData.lastLoginTime = DateTime.UtcNow.ToString();
        }

        //Check for any ongoing Training
        if (petDataRef.petData.ongoingTrainingData.isTraining)
        {
            petTrainingManager.CheckForAnyOngoingTraining();
        }

        offLoadingCanvasEvent.Raise();
    }

    //Calculating Lost Stat Vale
    int CalculateLostStatValue(string lastTime, float timeLength, int tickRate)
    {
        return (int)(CheckTimeDiffWithCurrentTimeInSec(lastTime) / timeLength) * tickRate;
    }

    //Calculating Time diff with current time in seconds
    public float CheckTimeDiffWithCurrentTimeInSec(string lastTime)
    {
        return (float)(DateTime.UtcNow - DateTime.Parse(lastTime)).TotalSeconds;
    }

    #endregion

    #region CHECKING FOR PET DEATH
    public bool IsPetDeathDueToHunger()
    {
        int lowHunger;

        //Checking for Hunger time after Hunger <= 10
        if (petDataRef.petData.hunger > petStatData.lowHungerThreshold)
        {
            lowHunger = Mathf.Abs(petDataRef.petData.hunger - petStatData.lowHungerThreshold);
        }
        else
        {
            lowHunger = 0;
        }

        float timeBeforeLowLevelHunger = CalculateLowerStatTime(lowHunger, petStatData.hungerTimeLength, petStatData.hungerTickRate);
        float timeOfLowHunger = CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.lastTimeFeed) - timeBeforeLowLevelHunger;

        //Checking time is passing LowerHungerTimeInDay limit
        if ((timeOfLowHunger / oneDaySeconds) > petStatData.lowerHungerTimeInDay)
        {
            petCareUIManager.ShowPetDeathUI("Pet die due to NO FOOD more than " + petStatData.lowerHungerTimeInDay + " days!");
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

        //Checking for Cleanliness time after Hunger <= 10
        if (petDataRef.petData.cleanliness > petStatData.lowCleanlinessThreshold)
        {
            lowCleanliness = Mathf.Abs(petDataRef.petData.cleanliness - petStatData.lowCleanlinessThreshold);
        }
        else
        {
            lowCleanliness = 0;
        }

        float timeBeforeLowerCleanliness = CalculateLowerStatTime(lowCleanliness, petStatData.cleanTimeLength, petStatData.cleanlinessTickRate);
        float timeOfLowerCleanliness = CheckTimeDiffWithCurrentTimeInSec(petDataRef.petData.lastTimeClean) - timeBeforeLowerCleanliness;

        //Checking time is passing LowerCleanlinessTimeInDay limit
        if ((timeOfLowerCleanliness / oneDaySeconds) > petStatData.lowerCleanlinessTimeInDay)
        {
            petCareUIManager.ShowPetDeathUI("Pet die due to NO CLEANLINESS more than " + petStatData.lowerCleanlinessTimeInDay + " days!");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPetDeathDueToHealth(int lostHappiness, int lostHunger, int lostCleanliness, int lostEnergy)
    {
        //Checking for Happiness time after Happiness = 0
        int lowerHappiness = Mathf.Abs(lostHappiness - petStatData.maxHappiness);
        float timeOfLowerHappiness = CalculateLowerStatTime(lowerHappiness, petStatData.happyTimeLength, petStatData.happinessTickRate);

        //Checking for Hunger time after Hunger = 0
        int lowerHunger = Mathf.Abs(lostHunger - petStatData.maxHunger);
        float timeOfLowerHunger = CalculateLowerStatTime(lowerHunger, petStatData.hungerTimeLength, petStatData.hungerTickRate);

        //Checking for Cleanliness time after Cleanliness = 0
        int lowerCleanliness = Mathf.Abs(lostCleanliness - petStatData.maxCleanliness);
        float timeOfLowerCleanliness = CalculateLowerStatTime(lowerCleanliness, petStatData.cleanTimeLength, petStatData.cleanlinessTickRate);

        //Checking for Energy time after Energy = 0
        int lowerEnergy = Mathf.Abs(lostEnergy - petStatData.maxEnergy);
        float timeOfLowerEnergy = CalculateLowerStatTime(lowerEnergy, petStatData.energyTimeLength, petStatData.energyTickRate);

        //Checking all times are passing LowerHealthTimeInHour limit
        if ((timeOfLowerHappiness / oneHourSeconds) > petStatData.lowerHealthTimeInHour && (timeOfLowerHunger / oneHourSeconds) > petStatData.lowerHealthTimeInHour
            && (timeOfLowerCleanliness / oneHourSeconds) > petStatData.lowerHealthTimeInHour && (timeOfLowerEnergy / oneHourSeconds) > petStatData.lowerHealthTimeInHour)
        {
            petCareUIManager.ShowPetDeathUI("Pet die due to NO HEALTH more than " + petStatData.lowerHealthTimeInHour + " hours!");
            return true;
        }
        else
        {
            return false;
        }
    }

    float CalculateLowerStatTime(int lowerStatValue, float timelength, int tickRate)
    {
        float timeOfLowerStat = 0;

        while (lowerStatValue > 0)
        {
            timeOfLowerStat += timelength;
            lowerStatValue -= tickRate;
        }

        return timeOfLowerStat;
    }
    #endregion


    #region RANDOM EVENTS
    //Checking if user is login on new day
    public bool IsUserLoginNewDay()
    {
        if ((DateTime.UtcNow - DateTime.Parse(petDataRef.petData.lastLoginTime)).TotalDays >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Flu (Random Event)
    public string PetGettingSickWithFluChance()
    {
        if (UnityEngine.Random.Range(0, 100) <= petStatData.fluChance)
        {
            petDataRef.petData.isSick = true;
            petCareUIManager.ShowPetSickLabel();
            return "Oh no, " + petDataRef.petData.petname + " caught a flu! \nBuy / Use a Medicine bottle for them to make them all better!";
        }

        return null;
    }

    //Treasure Hunt (Random Event)
    public string PetGettingCoinsWithTreasureHuntChance()
    {
        if (UnityEngine.Random.Range(0, 100) <= petStatData.treasureHuntChance)
        {
            int coins = (int)(1f / petDataRef.petData.rank * 100f);

            //Coins needs to be added
            ManageCleanlinessDataFiller(petStatData.cleanlinessTreasureHuntValue);

            return petDataRef.petData.petname + " found some coins while you were gone, digging for burried treasure!\n" + coins + " coins found" + "\n -20 Cleanliness";
        }

        return null;
    }
    #endregion


    #region XP, RANK AND MAX STAMINA
    //Increase XP with value
    public void IncreaseXP(float value)
    {
        petDataRef.petData.xp += value;

        //If Rank is increased then Show RankUp popup and update stamina
        if (CheckForRankUp())
        {
            petDataRef.petData.rank++;

            //Show Rank popup
            petCareUIManager.ShowRankUpUI();

            //Update Max Stamina
            UpdateMaxStamina();
        }
    }

    //Checking for Rank Up using XP
    public bool CheckForRankUp()
    {
        float xpNeededToRankUp = 40000 * (Mathf.Pow(1.53f, petDataRef.petData.rank - 1) - 1);

        if (petDataRef.petData.xp >= xpNeededToRankUp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Update Max Stamina based on Rank
    public void UpdateMaxStamina()
    {
        petDataRef.petData.maxStamina = 90 + (petDataRef.petData.rank * 10);
    }
    #endregion
}
