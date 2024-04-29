using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beamable.Common;
using Beamable;

public class BeamableManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyMethodViaAsyncAwait();
    }

    private async void MyMethodViaAsyncAwait()
    {
        var beamableAPI = await Beamable.API.Instance;
        Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
    }

}
