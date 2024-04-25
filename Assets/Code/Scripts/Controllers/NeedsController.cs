using System;
using UnityEngine;

public class NeedsController : MonoBehaviour
{
    public int food, happiness, energy;
    public int foodTickRate, happinessTickRate, energyTickRate;
    public DateTime lastTimeFed, lastTimeHappy, lastTimeGainedEnergy;

    public void Initialize(int food, int happiness, int energy, int foodTickRate, int happinessTickRate, int energyTickRate)
    {
        lastTimeFed = DateTime.Now;
        lastTimeHappy = DateTime.Now;
        lastTimeGainedEnergy = DateTime.Now;
        this.food = food;
        this.happiness = happiness;
        this.energy = energy;
        this.foodTickRate = foodTickRate;
        this.happinessTickRate = happinessTickRate;
        this.energyTickRate = energyTickRate;
    }

    private void Update()
    {

        if (TimingManager.gameHourTimer < 0)
        {
            ChangeFood(-foodTickRate);
            ChangeHappiness(-happinessTickRate);
            ChangeEnergy(-energyTickRate);
        }
    }

    public void ChangeFood(int amount)
    {
        food += amount;
        if (amount > 0)
        {
            lastTimeFed = DateTime.Now;
        }
        if (food < 0)
        {
            PetManager.instance.Die();
        }
        else if (food > 100) food = 100;
    }

    public void ChangeHappiness(int amount)
    {
        happiness += amount;
        if (amount > 0)
        {
            lastTimeHappy = DateTime.Now;
        }
        if (happiness < 0)
        {
            PetManager.instance.Die();
        }
        else if (happiness > 100) happiness = 100;
    }

    public void ChangeEnergy(int amount)
    {
        energy += amount;
        if (amount > 0)
        {
            lastTimeGainedEnergy = DateTime.Now;
        }
        if (energy < 0)
        {
            PetManager.instance.Die();
        }
        else if (energy > 100) energy = 100;
    }
}
