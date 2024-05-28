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
    public string birthTime, lastTimeHappy, lastTimeFeed, lastTimeClean, lastTimeEnergy, lastLoginTime;

    //Athletics Stats
    public int running, climbing, flying, swimming;

    //Chance Stats
    public int intelligence, luck;

    //XP & Rank
    public int rank, maxStamina;
    public float xp;

    //Sleep Data
    public PetSleepData sleepData;

    //Food Data
    public List<PetFoodData> foodData;

    //Training Data
    public PetOngoingTrainingData ongoingTrainingData;
}

[System.Serializable]
public class PetSleepData
{
    public bool isSleeping;
    public string sleepStartTime;
}

[System.Serializable]
public class PetFoodData
{
    public FoodItems foodName;
    public int foodCount;
}

[System.Serializable]
public class PetOngoingTrainingData
{
    public bool isTraining;
    public PetTraining ongoingTraining;
    public string ongoingTrainingName;
    public string trainingStartTime;
}
