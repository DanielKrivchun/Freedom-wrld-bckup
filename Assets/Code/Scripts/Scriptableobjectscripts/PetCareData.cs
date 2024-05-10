using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PetCareData", menuName = "PetCare/PetCareData", order = 100)]
public class PetCareData : ScriptableObject
{
    public string lastTimeHappy, lastTimeFeed, lastTimeClean, lastTimeSleep;
    public int happiness, hunger, cleanliness, energy;

    public void SetPetCareData(string lastTimeHappy, string lastTimeFeed, string lastTimeClean, string lastTimeSleep, int happiness, int hunger, int cleanliness, int energy)
    {
        this.lastTimeHappy = lastTimeHappy;
        this.lastTimeFeed = lastTimeFeed;
        this.lastTimeClean = lastTimeClean;
        this.lastTimeSleep = lastTimeSleep;
        this.happiness = happiness;
        this.hunger = hunger;
        this.cleanliness = cleanliness;
        this.energy = energy;
    }
}
