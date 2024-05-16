[System.Serializable]
public class PetData
{
    //Pet
    public string petname;

    //Wellbeing Stats
    public int health, happiness, hunger, cleanliness, energy;
    public bool isSick;

    //Timings
    public string lastTimeHappy, lastTimeFeed, lastTimeClean, lastTimeEnergy, sleepStartTime;

    //Athletics Stats
    public int running, climbing, flying, swimming;

    //Chance Stats
    public int intelligence, luck;

    public bool isSleeping;
}
