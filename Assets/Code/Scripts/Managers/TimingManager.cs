using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public GameEventState petCareTimerEvent;
    public PetCareStateManager petCareStateManager;

    [Space]
    public static float happyTimer;
    public float happyTimeLength;

    public static float feedTimer;
    public float feedTimeLength;

    public static float cleanTimer;
    public float cleanTimeLength;

    private void Start()
    {
        happyTimer = happyTimeLength;
        feedTimer = feedTimeLength;
        cleanTimer = cleanTimeLength;
    }

    private void Update()
    {
        if(petCareStateManager.happiness > 0)
        {
            SetHappyTimer();
        }

        if(petCareStateManager.hunger > 0)
        {
            SetFeedTimer();
        }

        if(petCareStateManager.cleanliness > 0)
        {
            SetCleanTimer();
        }
    }

    private void SetHappyTimer()
    {
        //HAPPY
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

    public void SetFeedTimer()
    {
        //FEED
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

    public void SetCleanTimer() 
    { 
        //CLEAN
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
}


