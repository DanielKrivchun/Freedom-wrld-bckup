using System.Collections.Generic;

[System.Serializable]
public class PetData
{
    //Pet
    public string petname;

    //Wellbeing Stats
    public int health, happiness, hunger, cleanliness, energy;
    public bool isSick;

    //Timings
    public string birthTime, lastTimeHappy, lastTimeFeed, lastTimeClean, lastTimeEnergy, sleepStartTime;

    //Athletics Stats
    public int running, climbing, flying, swimming;

    //Chance Stats
    public int intelligence, luck;

    //XP & Rank
    public int rank, maxStamina;
    public float xp;

    public bool isSleeping;

    public List<PetFoodData> foodData;
}

[System.Serializable]
public class PetFoodData
{
    public string foodname;
    public int foodCount;
}
