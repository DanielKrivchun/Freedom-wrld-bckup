using UnityEngine;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour
{
    public GameEventState petCareTimerEvent;
    public PetCareStateManager petCareStateManager;

    [Space]
    public float happyTimeLength;
    public static float happyTimer;

    public static float feedTimer;
    public float feedTimeLength;

    public static float cleanTimer;
    public float cleanTimeLength;

    public static float energyTimer;
    public float energyTimeLength;

    private void Start()
    {
        happyTimer = happyTimeLength;
        feedTimer = feedTimeLength;
        cleanTimer = cleanTimeLength;
        energyTimer = energyTimeLength;
    }

    private void Update()
    {
        //Happy
        if(petCareStateManager.happiness > 0)
        {
            SetHappyTimer();
        }

        //Eat
        if(petCareStateManager.hunger > 0)
        {
            SetFeedTimer();
        }

        //Clean
        if(petCareStateManager.cleanliness > 0)
        {
            SetCleanTimer();
        }

        //Energy
        if (petCareStateManager.energy > 0)
        {
            SetEnergyTimer();
        }
    }

    private void SetHappyTimer()
    {
        if (happyTimer <= 0)
        {
            happyTimer = happyTimeLength;
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
        if (feedTimer <= 0)
        {
            feedTimer = feedTimeLength;
            petCareTimerEvent.Raise(PetCareState.Feed);
        }
        else
        {
            feedTimer -= Time.deltaTime;
        }
        //Debug.Log("Feed - " + feedTimer);
    }

    private void SetCleanTimer() 
    { 
        if (cleanTimer <= 0)
        {
            cleanTimer = cleanTimeLength;
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
            energyTimer = energyTimeLength;
            petCareTimerEvent.Raise(PetCareState.Energy);
        }
        else
        {
            energyTimer -= Time.deltaTime;
        }
        //Debug.Log("Energy - " + energyTimer);
    }
}