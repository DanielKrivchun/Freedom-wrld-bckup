using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputController : MonoBehaviour
{
}

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
}

public struct netdata: INetworkInput
{
    public Vector3 direction;
}



[System.Serializable]
public class _AllPlayerData
{
    public PlayerRef playerRef;
    public NetworkObject networkObject;
}
