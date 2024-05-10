using System;

public class Pet
{
    public string lastTimeHappy, lastTimeFeed, lastTimeClean;
    public int happiness, hunger, cleanliness, energy;

    public Pet(string lastTimeHappy, string lastTimeFeed, string lastTimeClean, int happiness, int cleanliness, int hunger, int energy)
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
