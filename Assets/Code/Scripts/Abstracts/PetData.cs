
public class PetData
{
    public string lastTimeHappy, lastTimeFeed, lastTimeClean, lastTimeEnergy, sleepStartTime;
    public int happiness, hunger, cleanliness, energy;
    public bool isSleeping;

    public PetData(){}

    public PetData(string lastTimeHappy, string lastTimeFeed, string lastTimeClean, string lastTimeEnergy, string sleepStartTime, bool isSleeping, int happiness, int cleanliness, int hunger, int energy)
    {
        this.lastTimeHappy = lastTimeHappy;
        this.lastTimeFeed = lastTimeFeed;
        this.lastTimeClean = lastTimeClean;
        this.lastTimeEnergy = lastTimeEnergy;
        this.sleepStartTime = sleepStartTime;
        this.isSleeping = isSleeping;
        this.happiness = happiness;
        this.hunger = hunger;
        this.cleanliness = cleanliness;
        this.energy = energy;
    }
}
