using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetDataRef", menuName = "ScriptableObject/PetDataRef", order = 100)]
public class PetDataRef : ScriptableObject
{
    public PetData petData = new PetData();

    public void SetPetAllData(string petName, int health, int happiness, int cleanliness, int hunger, int energy,
                                string lastTimeHappy, string lastTimeFeed, string lastTimeClean, string lastTimeEnergy,  
                                int running, int climbing, int flying, int swimming, int intelligence, int luck,
                                string sleepStartTime, bool isSleeping)
    {
        petData.petname = petName;

        petData.health = health;
        petData.happiness = happiness;
        petData.hunger = hunger;
        petData.cleanliness = cleanliness;
        petData.energy = energy;
        
        petData.lastTimeHappy = lastTimeHappy;
        petData.lastTimeFeed = lastTimeFeed;
        petData.lastTimeClean = lastTimeClean;
        petData.lastTimeEnergy = lastTimeEnergy;
        petData.sleepStartTime = sleepStartTime;
        petData.isSleeping = isSleeping;

        petData.running = running;
        petData.climbing = climbing;
        petData.flying = flying;
        petData.swimming = swimming;

        petData.intelligence = intelligence;
        petData.luck = luck;
        
    }

    public void SetPetWellbeingStatsData(int health, int happiness, int cleanliness, int hunger, int energy, 
                                        string lastTimeHappy, string lastTimeFeed, string lastTimeClean, string lastTimeEnergy, string sleepStartTime, bool isSleeping)
    {
        petData.health = health;
        petData.happiness = happiness;
        petData.hunger = hunger;
        petData.cleanliness = cleanliness;
        petData.energy = energy;

        petData.lastTimeHappy = lastTimeHappy;
        petData.lastTimeFeed = lastTimeFeed;
        petData.lastTimeClean = lastTimeClean;
        petData.lastTimeEnergy = lastTimeEnergy;
        petData.sleepStartTime = sleepStartTime;
        petData.isSleeping = isSleeping;
    }

    public void SetPetAthleticsStatsData(int running, int climbing, int flying, int swimming)
    {
        petData.running = running;
        petData.climbing = climbing;
        petData.flying = flying;
        petData.swimming = swimming;
    }

    public void SetPetChanceStatsData(int intelligence, int luck)
    {
        petData.intelligence = intelligence;
        petData.luck = luck;
    }
}
