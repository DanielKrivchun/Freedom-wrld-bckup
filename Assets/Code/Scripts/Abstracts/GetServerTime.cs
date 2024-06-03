using PlayFab;
using PlayFab.ServerModels;
using System;
using System.Threading.Tasks;
using UnityEngine;

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


    public async Task<DateTime> GetCurrentTimeTask()
    {
        bool isTimeSet = false;
        DateTime T = new();

        PlayFabServerAPI.GetTime(new GetTimeRequest(),
           (response) =>
           {
               T = response.Time;
               isTimeSet = true;
           },
           LogFailure);

        while (!isTimeSet)
        {
            await Task.Delay(500);
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
