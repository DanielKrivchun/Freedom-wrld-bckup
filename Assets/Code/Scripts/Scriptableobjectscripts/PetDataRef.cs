using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;

[CreateAssetMenu(fileName = "PetDataRef", menuName = "ScriptableObject/PetDataRef", order = 100)]
public class PetDataRef : ScriptableObject
{
    //Cloud Data
    public PetData petData;

    //Local Data
    public PetLocalData petLocalData;

    public void SetPetAllData(string petName, int health, int happiness, int cleanliness, int hunger, int energy, bool isSick,
                                string petBirthTime, string lastTimeHappy, string lastTimeFeed, string lastTimeClean, string lastTimeEnergy, string lastLoginTime,
                                int running, int climbing, int flying, int swimming, int intelligence, int luck,
                                int rank, float xp, int maxStamina)
    {
        petData = new PetData();

        petData.petname = petName;

        petData.health = health;
        petData.happiness = happiness;
        petData.hunger = hunger;
        petData.cleanliness = cleanliness;
        petData.energy = energy;
        petData.isSick = isSick;

        petData.birthTime = petBirthTime;
        petData.lastTimeHappy = lastTimeHappy;
        petData.lastTimeFeed = lastTimeFeed;
        petData.lastTimeClean = lastTimeClean;
        petData.lastTimeEnergy = lastTimeEnergy;

        petData.lastLoginTime = lastLoginTime;

        petData.running = running;
        petData.climbing = climbing;
        petData.flying = flying;
        petData.swimming = swimming;


        petData.intelligence = intelligence;
        petData.luck = luck;

        petData.xp = xp;
        petData.rank = rank;
        petData.maxStamina = maxStamina;

        petData.foodData = new List<PetFoodData>();
        petData.sleepData = new PetSleepData();
        petData.ongoingTrainingData = new PetOngoingTrainingData();
    }
}

[System.Serializable]
public class PetLocalData
{
    public string petID;
}
