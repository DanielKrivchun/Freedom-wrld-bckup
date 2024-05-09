using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PetCareData", menuName = "PetCare/PetCareData", order = 100)]
public class PetCareData : ScriptableObject
{
    public DateTime lastTimeHappy, lastTimeFeed, lastTimeClean;

    public int happiness, hunger, cleanliness, energy;

    public PetCareData(DateTime lastTimeHappy, DateTime lastTimeFeed, DateTime lastTimeClean, 
                        int happiness, int hunger, int cleanliness, int energy) 
    {
        this.lastTimeHappy = lastTimeHappy;
        this.lastTimeFeed = lastTimeFeed;
        this.lastTimeClean = lastTimeClean;
        this.happiness = happiness;
        this.hunger = hunger;
        this.cleanliness = cleanliness;
        this.energy = energy;
    }
}
