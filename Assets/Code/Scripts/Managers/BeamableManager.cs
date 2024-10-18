using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beamable.Common;
using Beamable;

public class BeamableManager : MonoBehaviour
{

    //private string curruntID;
    //private string oldID;

    // Start is called before the first frame update
    void Start()
    {
        MyMethodViaAsyncAwait();
        //oldID = PlayerPrefs.GetString("oldID");
        //curruntID = PlayerPrefs.GetString("curruntID");

        //if (oldID.Length == 0)
        //{
        //    Debug.Log("Old ID is not defined");
        //    oldID = "oldID";
        //}

        //if (curruntID.Length == 0)
        //{
        //    Debug.Log("curruntID is not defined");
        //    curruntID = "curruntID";
        //}

    }

    private async void MyMethodViaAsyncAwait()
    {
        var beamableAPI = await Beamable.API.Instance;
        Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");

        //if (curruntID != oldID)
        //{
        //    Debug.Log("Acount is switched");
        //    PlayerPrefs.SetString("curruntID", beamableAPI.User.id.ToString());
        //    PlayerPrefs.SetString("oldID", beamableAPI.User.id.ToString());
        //    curruntID = beamableAPI.User.id.ToString();
        //    oldID = beamableAPI.User.id.ToString();
        //    PlayerPrefs.DeleteKey("IsPetCreated");

        //}
    }

}
