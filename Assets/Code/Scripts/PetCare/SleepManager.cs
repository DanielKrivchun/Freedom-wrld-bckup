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
    public float sleepTimer;

    [Space]
    public Button sleepBtn;
    public Text sleepCountdownTxt;

    private void OnEnable()
    {
        if(isCanSleep)
        {
            Debug.Log("False Called");
            sleepBtn.enabled = false;
        }
        else
        {
            Debug.Log("True Called");
            sleepBtn.enabled = true;
        }
    }

    private void Start()
    {
        sleepBtn.onClick.AddListener(()=> StartSleepingTimer());  
    }

    public void StartSleepingTimer()
    {
        sleepTimer = totalSleepTime;
        isCanSleep = true;
        petCareStateManager.sleepStartTime = DateTime.Now;
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
                isCanSleep = false;
                sleepTimer = totalSleepTime;

                sleepCountdownTxt.text = "START";
                sleepBtn.enabled = true;
                
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
