using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SleepManager : MonoBehaviour
{
    public PetCareStateManager petCareStateManager;
    public ParticleEffectsManager particleEffectsManager;

    [Space]
    public bool isCanSleep;
    public float totalSleepTime;
    [HideInInspector]
    public float sleepTimer;

    [Space]
    public Button sleepBtn;
    public Text sleepCountdownTxt;

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

    private void StartSleepingTimer()
    {
        //If Cleanliness <= 10/100 && Hunger <= 10/100 , then the pet will not be able to sleep
        if (petCareStateManager.petDataRef.petData.cleanliness > 10 &&
            petCareStateManager.petDataRef.petData.hunger > 10)
        {
            particleEffectsManager.StartSleepEffect();
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Sleep);

            sleepTimer = totalSleepTime;
            isCanSleep = true;
            petCareStateManager.petDataRef.petData.sleepData.isSleeping = true;
            petCareStateManager.petDataRef.petData.sleepData.sleepStartTime = DateTime.UtcNow.ToString();
        } 
    }

    public void SetSleepingTimer(float sleepTimeTillNow)
    {
        sleepTimer = totalSleepTime - sleepTimeTillNow;
        isCanSleep = true;

        gameObject.SetActive(true);
    }

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
                Debug.Log("Time is UP!");
                particleEffectsManager.StopSleepEffect();
                PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Idle);

                isCanSleep = false;
                sleepTimer = totalSleepTime;

                sleepCountdownTxt.text = "START";
                sleepBtn.enabled = true;

                petCareStateManager.petDataRef.petData.sleepData.isSleeping = false;
                petCareStateManager.ManageEnergyDataFiller(100);
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
}
