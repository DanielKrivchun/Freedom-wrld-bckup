using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SleepManager : MonoBehaviour
{
    public PetCareStateManager petCareStateManager;
    public ParticleEffectsManager particleEffectsManager;
    public GetServerTime getServerTime;

    [Space]
    public bool isCanSleep;
    public float totalSleepTime;

    [HideInInspector]
    public float sleepTimer;

    [Space]
    public Button sleepBtn;
    public Text sleepCountdownTxt;

    [Space]
    public GameObject player;
    public Transform sleepPoint;

    [Space]
    public GameObject petCareBtnHolder;

    private void OnEnable()
    {
        if(isCanSleep)
        {
            sleepBtn.enabled = false;
        }
        else
        {
            sleepBtn.enabled = true;
        }
    }

    private void Start()
    {
        sleepBtn.onClick.AddListener(()=> StartSleepingTimer());  
    }

    #region SET AND START SLEEPING
    private void StartSleepingTimer()
    {
        //If Cleanliness <= 10/100 && Hunger <= 10/100 , then the pet will not be able to sleep
        if (petCareStateManager.petDataRef.petData.cleanliness > 10 &&
            petCareStateManager.petDataRef.petData.hunger > 10)
        {
            sleepBtn.enabled = false;
            petCareStateManager.StopIdleTimer();
            petCareBtnHolder.SetActive(false);

            particleEffectsManager.StartSleepEffect();
            PetCareInputManager.instance.SetPetToInsideHomeOnSleepStart();

            sleepTimer = totalSleepTime;
            isCanSleep = true;

            petCareStateManager.petDataRef.petData.sleepData.isSleeping = true;
            getServerTime.GetCurrentTime(timeNow => { petCareStateManager.petDataRef.petData.sleepData.sleepStartTime = timeNow.ToString(); });
        } 
    }

    public void SetSleepingTimer(float sleepTimeTillNow)
    {
        petCareBtnHolder.SetActive(false);
        sleepTimer = totalSleepTime - sleepTimeTillNow;
        isCanSleep = true;
        gameObject.SetActive(true);

        player.transform.position = sleepPoint.position;
        particleEffectsManager.StartSleepEffect();
        PetCareInputManager.instance.SetPetToInsideHomeOnSleepStart();
    }
    #endregion

    #region RESET TIMER AND RESET CARETAKING STAT
    void ResetTimerAndSetPetStatData()
    {
        particleEffectsManager.StopSleepEffect();
        PetCareInputManager.instance.SetPetToOutsideHomeOnSleepComplete();

        isCanSleep = false;
        sleepTimer = totalSleepTime;

        sleepCountdownTxt.text = "START";
        sleepBtn.enabled = true;

        petCareStateManager.petDataRef.petData.sleepData.isSleeping = false;
        petCareStateManager.ManageEnergyDataFiller(100);

        petCareBtnHolder.SetActive(true);
        gameObject.SetActive(false);

        petCareStateManager.ResetPetCareTakingState();
    }
    #endregion

    #region TIMER
    private void Update()
    {
        //Sleep
        if (isCanSleep)
        {
            SleepingTimer();
        }
    }

    private void SleepingTimer()
    {
        if (isCanSleep)
        {
            if (sleepTimer > 0)
            {
                sleepTimer -= Time.deltaTime;
                UpdateTimer(sleepTimer);
            }
            else
            {
                ResetTimerAndSetPetStatData();
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float hours = Mathf.FloorToInt(currentTime / 3600);
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        sleepCountdownTxt.text = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    #endregion
}
