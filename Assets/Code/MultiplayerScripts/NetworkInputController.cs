using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputController : MonoBehaviour
{
}

public struct NetworkInputData : INetworkInput
{
    public float TapMultiplier;
    public float Speed;
    public Vector2 direction;
}

public struct netdata : INetworkInput
{
    public Vector3 direction;
}



[System.Serializable]
public class _AllPlayerData
{
    public PlayerRef playerRef;
    public NetworkObject Player;
}

[System.Serializable]
public class _GenratedAIPlayer
{
    public bool AI;
    public NavmeshMultiplayer player;
    public NetworkAIPlayer aiplayer;
}

[System.Serializable]
public class _AIplayerDetails
{
    public PlayerRef playerRef;
    public NetworkAIPlayer aiplayer;
}
