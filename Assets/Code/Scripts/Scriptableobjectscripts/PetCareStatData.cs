using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetCareStatData", menuName = "ScriptableObject/PetCareStatData", order = 100)]

public class PetCareStatData : ScriptableObject
{
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


    [Header("Happiness")]
    public int maxHappiness;
    /// <summary>
    ///  OLD:
    ///  Happy lasts 2/3 of a real day so time = 16 hours = 960 minutes
    ///  So after 9.6 minutes User lost 1 happiness
    ///  happyTimeLength = 9.6 minutes = 576 seconds
    /// </summary>
    /// <summary>
    ///  NEW:
    ///  Happy lasts 2 real days so time = 48 hours = 2880 minutes
    ///  So after 28.8 minutes User lost 1 happiness
    ///  happyTimeLength = 28.8 minutes = 1728 seconds
    /// </summary>
    public float happyTimeLength;
    // TickRate is the value which will reduce after Time Length is over
    public int happinessTickRate;


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

    [Header("Flu (Random Event)")]
    /// <summary>
    ///  Pet has a 3% chance getting sick / flu
    ///  fluChance = 3
    /// </summary> 
    public int fluChance;

    [Header("Treasure Hunt (Random Event)")]
    /// <summary>
    ///  Pet has a 50% chance of getting ([1/Rank] * 100) number of coins & -20 cleanliness
    ///  treasureHuntChance = 50
    ///  coins = [1/Rank] * 100
    ///  cleanlinessTreasureHuntValue = -20
    /// </summary> 
    public int treasureHuntChance;
    public int cleanlinessTreasureHuntValue;
}
