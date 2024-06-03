using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ServerModels;
using System;
using System.Threading.Tasks;

public class GetServerTime : MonoBehaviour
{
    public void GetCurrentTime(Action<DateTime> nowTime)
    {
        PlayFabServerAPI.GetTime(new GetTimeRequest(),
            (response) =>
            {
                nowTime.Invoke(response.Time);
            },
            LogFailure);

    }


    public async Task<DateTime> GetCurrntTimeTask()
    {
        bool gettingTime = false;
        DateTime T = new DateTime();

        PlayFabServerAPI.GetTime(new GetTimeRequest(),
           (response) =>
           {
               T = response.Time;
           },
           LogFailure);

        while (!gettingTime)
        {
            await Task.Delay(10);
        }

        return T;
    }

    void OnGetTimeSuccess(GetTimeResult result)
    {
        Debug.Log("The time is: " + result.Time);

    }

    void LogFailure(PlayFabError error)
    {
        Debug.Log("There was a problem getting the time. Error: " + error.GenerateErrorReport());
    }
}
