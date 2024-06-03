using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ServerModels;
using System;

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

    void OnGetTimeSuccess(GetTimeResult result)
    {
        Debug.Log("The time is: " + result.Time);

    }

    void LogFailure(PlayFabError error)
    {
        Debug.Log("There was a problem getting the time. Error: " + error.GenerateErrorReport());
    }
}
