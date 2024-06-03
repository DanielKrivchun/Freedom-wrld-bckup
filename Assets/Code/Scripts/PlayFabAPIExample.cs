using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayFabAPIExample : MonoBehaviour
{
    private string playFabTitleId = "825DE";
    private string playFabLoginUrl = "https://{0}.playfabapi.com/Client/LoginWithCustomID";
    private string playFabGetTimeUrl = "https://{0}.playfabapi.com/Client/GetTime";
    private string sessionTicket;

    void Start()
    {
        StartCoroutine(LoginToPlayFab());
    }

    IEnumerator LoginToPlayFab()
    {
        string url = string.Format(playFabLoginUrl, playFabTitleId);

        var requestPayload = new
        {
            TitleId = playFabTitleId,
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        string json = JsonUtility.ToJson(requestPayload);
        UnityWebRequest www = UnityWebRequest.Put(url, json);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login failed: " + www.error);
        }
        else
        {
            Debug.Log("Login successful!");

            var responseJson = www.downloadHandler.text;
            var loginResponse = JsonUtility.FromJson<PlayFabLoginResponse>(responseJson);
            sessionTicket = loginResponse.SessionTicket;

            StartCoroutine(GetServerTime());
        }
    }

    IEnumerator GetServerTime()
    {
        string url = string.Format(playFabGetTimeUrl, playFabTitleId);

        UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "");
        www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("X-Authorization", sessionTicket);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get server time: " + www.error);
        }
        else
        {
            var responseJson = www.downloadHandler.text;
            var timeResponse = JsonUtility.FromJson<PlayFabTimeResponse>(responseJson);
            Debug.Log("Current server time (UTC): " + timeResponse.Time);
        }
    }
}

[Serializable]
public class PlayFabLoginResponse
{
    public string SessionTicket;
}

[Serializable]
public class PlayFabTimeResponse
{
    public DateTime Time;
}
