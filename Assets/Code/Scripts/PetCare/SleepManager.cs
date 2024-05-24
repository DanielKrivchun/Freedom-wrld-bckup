using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SleepManager : MonoBehaviour
{
    public PetCareStateManager petCareStateManager;

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

    public void StartSleepingTimer()
    {
        //If Cleanliness <= 10/100 && Hunger <= 10/100 , then the pet will not be able to sleep
        if (petCareStateManager.petDataRef.petData.cleanliness > 10 &&
            petCareStateManager.petDataRef.petData.hunger > 10)
        {
            PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Sleep);

            sleepTimer = totalSleepTime;
            isCanSleep = true;
            petCareStateManager.petDataRef.petData.isSleeping = true;
            petCareStateManager.petDataRef.petData.sleepStartTime = DateTime.UtcNow.ToString();
        } 
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
                PetCareInputManager.instance.petAnim._ChangeAnimationState(_AnimState.Idle);

                isCanSleep = false;
                sleepTimer = totalSleepTime;

                sleepCountdownTxt.text = "START";
                sleepBtn.enabled = true;

                petCareStateManager.petDataRef.petData.isSleeping = false;
                petCareStateManager.ManageEnergyDataFiller(100);
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float hours = Mathf.FloorToInt(currentTime / 3660);
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        sleepCountdownTxt.text = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
