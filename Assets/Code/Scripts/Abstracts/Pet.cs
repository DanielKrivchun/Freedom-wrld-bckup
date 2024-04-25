using System;

public class Pet
{
    public DateTime lastTimeFed, lastTimeHappy, lastTimeGainedEnergy;
    public int food, happiness, energy;

    public Pet(DateTime lastTimeFed, DateTime lastTimeHappy, DateTime lastTimeGainedEnergy, int food, int happiness, int energy)
    {
        this.lastTimeFed = lastTimeFed;
        this.lastTimeHappy = lastTimeHappy;
        this.lastTimeGainedEnergy = lastTimeGainedEnergy;
        this.food = food;
        this.happiness = happiness; 
        this.energy = energy;
    }
}
