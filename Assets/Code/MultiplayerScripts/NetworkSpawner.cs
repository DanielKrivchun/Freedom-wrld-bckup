using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public InputValue inputValue;
    public NetworkCamera networkCamera;

    private NetworkRunner networkRunner;

    public Vector2 m_input;

    [Space]
    public NetworkPrefabRef prefab;

    public List<_AllPlayerData> genratedPlayers;
    [Space]
    public List<NavmeshMultiplayer> m_objects;
    private NetworkObject networkPlayerObject;

    //private Dictionary<PlayerRef, NetworkObject> genratedplayers = new Dictionary<PlayerRef, NetworkObject>();

    private void Start()
    {
        inputValue.m_enable_navmesh = false;
    }

    public async void _GameMode(GameMode Mode)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var scenenetwork = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            scenenetwork.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = Mode,
            SessionName = "Test",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void _StartGameForPlayers()
    {
        var foundplayers = FindObjectsOfType<NavmeshMultiplayer>();
        m_objects = foundplayers.ToList();
        Debug.Log("m_objects.Count " + m_objects.Count);

        foreach (_AllPlayerData item in genratedPlayers)
        {
            Debug.Log(item.playerRef.PlayerId);

            if (item.networkObject == null)
            {
                foreach (NavmeshMultiplayer playfabs in m_objects)
                {

                }
            }
        }

        foreach (var item in foundplayers)
        {
            Debug.Log(item.name);
            item._SetPathBasedOnIndex();
        }

        inputValue.m_enable_navmesh = true;
    }


    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();
    }

    #region INetworkRunnerCallbacks
    public void OnConnectedToServer(NetworkRunner runner)
    {
        ColoredDebug.Log("OnConnectedToServer", Color.green);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        ColoredDebug.Log("OnHostMigration", Color.green);
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        ColoredDebug.Log("OnObjectEnterAOI " + runner.name, Color.green);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        ColoredDebug.Log("OnPlayerJoined  IsServer", Color.green);


        if (runner.IsServer)
        {
            Debug.Log("I am Server");
            // Create a unique position for the player
            Vector3 spawnPosition = Vector3.zero;
            networkPlayerObject = runner.Spawn(prefab, spawnPosition, Quaternion.identity, player);
            networkPlayerObject.name = player.PlayerId.ToString();
            // Keep track of the player avatars for easy access
            _AllPlayerData d = new _AllPlayerData();
            d.playerRef = player;
            d.networkObject = networkPlayerObject;
            genratedPlayers.Add(d);
            //RPC_SetData(d);
            networkCamera._SetUpCamera(networkPlayerObject.transform);
            //CHECK COUNT OF PLAYER HERE
            //networkPlayerObject.GetComponent<NavmeshMultiplayer>()._SetUpMyInitialData();
            int a = genratedPlayers.Count;
            Debug.Log("TOTAL PLAYERS IN GAME " + a);

            if (a >= 2)
            {
                NetwrokUI.Instance._OpenStartUI();
            }
        }

        if (runner.IsClient)
        {
            Debug.Log("I am Client  " + player.PlayerId);
            _AllPlayerData d = new _AllPlayerData();
            d.playerRef = player;
            d.networkObject = null;
            genratedPlayers.Add(d);
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        ColoredDebug.Log("OnPlayerLeft  " + player.PlayerId, Color.red);

        _AllPlayerData p = genratedPlayers.Find(asd => asd.playerRef == player);

        if (p.networkObject != null)
        {
            runner.Despawn(p.networkObject);
            genratedPlayers.Remove(genratedPlayers.Find(asd => asd.playerRef == player));
        }

    }

    public void OnInput(NetworkRunner runner, Fusion.NetworkInput input)
    {
        var data = new NetworkInputData();
        data.direction = m_input;
        input.Set(data);
    }


    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    #endregion
    #region RPC Remote Procedure Call
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetData()
    {
        //genratedPlayers.Add(data);
    }
    #endregion
}
