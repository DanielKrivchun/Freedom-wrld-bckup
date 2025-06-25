using UnityEngine;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour
{
    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public GameEventState petCareTimerEvent;

    [Space]
    public PetCareStatData petStateData;

    //Timers
    private static float happyTimer;
    private static float hungerTimer;
    private static float cleanTimer;
    private static float energyTimer;

    private void Start()
    {
        happyTimer = petStateData.happyTimeLength;
        hungerTimer = petStateData.hungerTimeLength;
        cleanTimer = petStateData.cleanTimeLength;
        energyTimer = petStateData.energyTimeLength;
    }

    private void Update()
    {
        //Happy
        if(petDataRef.petData.happiness > 0)
        {
            SetHappyTimer();
        }

        //Eat
        if(petDataRef.petData.hunger > 0)
        {
            SetFeedTimer();
        }

        //Clean
        if(petDataRef.petData.cleanliness > 0)
        {
            SetCleanTimer();
        }

        //Energy
        if (petDataRef.petData.energy > 0)
        {
            SetEnergyTimer();
        }
    }

    private void SetHappyTimer()
    {
        if (happyTimer <= 0)
        {
            happyTimer = petStateData.happyTimeLength;
            petCareTimerEvent.Raise(PetCareState.Happy);
        }
        else
        {
            happyTimer -= Time.deltaTime;
        }
    }

    private void SetFeedTimer()
    {
        if (hungerTimer <= 0)
        {
            hungerTimer = petStateData.hungerTimeLength;
            petCareTimerEvent.Raise(PetCareState.Eat);
        }
        else
        {
            hungerTimer -= Time.deltaTime;
        }
    }

    private void SetCleanTimer() 
    { 
        if (cleanTimer <= 0)
        {
            cleanTimer = petStateData.cleanTimeLength;
            petCareTimerEvent.Raise(PetCareState.Clean);
        }
        else
        {
            cleanTimer -= Time.deltaTime;
        }
    }


    private void SetEnergyTimer()
    {
        if (energyTimer <= 0)
        {
            energyTimer = petStateData.energyTimeLength;
            petCareTimerEvent.Raise(PetCareState.Energy);
        }
        else
        {
            energyTimer -= Time.deltaTime;
        }
    }
}