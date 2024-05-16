using UnityEngine;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour
{
    [Header("Pet Care Data Reference")]
    public PetDataRef petDataRef;

    [Space]
    public GameEventState petCareTimerEvent;

    [Space]
    public PetCareStateManager petCareStateManager;

    //Timers
    private static float happyTimer;
    private static float hungerTimer;
    private static float cleanTimer;
    private static float energyTimer;

    private void Start()
    {
        happyTimer = petCareStateManager.happyTimeLength;
        hungerTimer = petCareStateManager.hungerTimeLength;
        cleanTimer = petCareStateManager.cleanTimeLength;
        energyTimer = petCareStateManager.energyTimeLength;
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
            happyTimer = petCareStateManager.happyTimeLength;
            petCareTimerEvent.Raise(PetCareState.Happy);
        }
        else
        {
            happyTimer -= Time.deltaTime;
        }
        //Debug.Log("Happy - " + happyTimer);
    }

    private void SetFeedTimer()
    {
        if (hungerTimer <= 0)
        {
            hungerTimer = petCareStateManager.hungerTimeLength;
            petCareTimerEvent.Raise(PetCareState.Feed);
        }
        else
        {
            hungerTimer -= Time.deltaTime;
        }
        //Debug.Log("Feed - " + feedTimer);
    }

    private void SetCleanTimer() 
    { 
        if (cleanTimer <= 0)
        {
            cleanTimer = petCareStateManager.cleanTimeLength;
            petCareTimerEvent.Raise(PetCareState.Clean);
        }
        else
        {
            cleanTimer -= Time.deltaTime;
        }
        //Debug.Log("Clean - " + cleanTimer);
    }

    private void SetEnergyTimer()
    {
        if (energyTimer <= 0)
        {
            energyTimer = petCareStateManager.energyTimeLength;
            petCareTimerEvent.Raise(PetCareState.Energy);
        }
        else
        {
            energyTimer -= Time.deltaTime;
        }
        //Debug.Log("Energy - " + energyTimer);
    }
}