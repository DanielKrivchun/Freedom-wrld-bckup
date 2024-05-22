using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Fusion.Sockets;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
public class RaceManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public InputValue InputValue;
    [Space]
    public Transform[] spawnPoints;
    [Header("NetworkRunner Prefab")]
    public NetworkRunner NetworkRunnerPrefab;
    [Space]
    [Header("Player Prefab")]
    public NetworkPrefabRef PlayerPrefab = NetworkPrefabRef.Empty;

    int a = 0;
    [SerializeField] private TMP_InputField inputField;
    public Vector2 m_input;
    [Networked] public int PathNumber { get; set; }

    public int CurrntWinCount;

    private NetworkRunner networkRunnerInstance;

    public string LocalPlayerNickname { get; private set; }

    public static RaceManager instance;

    public List<NetworkObject> GenratedPlayers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async void _StartGame(GameMode mode)
    {
        LocalPlayerNickname = inputField.text;

        if (networkRunnerInstance == null)
        {
            Debug.Log("Instantiated my object  " + mode);
            networkRunnerInstance = Instantiate(NetworkRunnerPrefab);
        }

        networkRunnerInstance.AddCallbacks(this);

        networkRunnerInstance.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var scenenetwork = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            scenenetwork.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        await networkRunnerInstance.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = "Test",
            Scene = scene,
            SceneManager = networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>()
        });


    }
    public void _StartGameForPlayers()
    {
        Debug.Log("_StartGameForPlayers");
        NetworkEventManager._EventStartGame();
        InputValue.m_enable_navmesh = true;
    }

    #region WIN LOGIC
    public int _GetMyWinningNo()
    {
        CurrntWinCount++;
        ColoredDebug.Log("Currunt Win Number " + CurrntWinCount);
        return CurrntWinCount;
    }
    #endregion


    #region PLAYER SPWANR

    private void _SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (PathNumber <= 0)
            {
                PathNumber = 0;
            }
            Vector3 spawnPoint = spawnPoints[PathNumber].transform.position;
            NetworkObject playerObject = Runner.Spawn(PlayerPrefab, spawnPoint, Quaternion.identity, playerRef);
            playerObject.GetComponent<NavmeshMultiplayer>()._SetUpMyInitialData(PathNumber);
            //playerı serverde yaptık.
            Runner.SetPlayerObject(playerRef, playerObject);
            PathNumber++;
            GenratedPlayers.Add(playerObject);

            if (PathNumber >= 2)
            {
                NetwrokUI.Instance._OpenStartUI();
            }
        }
    }

    private void _DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }
        // reset player object
        Runner.SetPlayerObject(playerRef, null);
    }
    #endregion

    //FOR INPUT
    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();
    }

    #region INetworkRunnerCallbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
        ColoredDebug.Log("OnPlayerJoined", Color.green);
        _SpawnPlayer(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
        _DespawnPlayer(player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.direction = m_input;
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }


    #endregion
}
