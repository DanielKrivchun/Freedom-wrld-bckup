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

    public Transform[] spawnPoints;
    [Header("NetworkRunner Prefab")]
    public NetworkRunner networkRunnerPrefab;
    [Space]
    [Header("Player Prefab")]
    public NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;

    public event Action OnStartedRunnerConnection;
    public event Action OnPlayerJoinedSuccesfully;

    int a = 0;
    [SerializeField] private TMP_InputField inputField;
    public Vector2 m_input;


    public NetworkRunner networkRunnerInstance;

    public string LocalPlayerNickname { get; private set; }

    public static RaceManager instance;

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

        OnStartedRunnerConnection?.Invoke();

        if (networkRunnerInstance == null)
        {
            Debug.Log("Instantiated my object  " + mode);
            networkRunnerInstance = Instantiate(networkRunnerPrefab);
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

    public override void Spawned()
    {
        Debug.Log("This Works");
        if (Runner.IsServer) // burası host için yapıldı bir kere olucak
        {
            foreach (var item in Runner.ActivePlayers)
            {
                SpawnPlayer(item);
            }
        }
    }

    private void SpawnPlayer(PlayerRef playerRef) // I want to instantiate the object only if I am the server in order to check if I am the server or not.
    {
        if (Runner.IsServer)
        {
            var index = a;

            a++;

            var spawnPoint = spawnPoints[index].transform.position;

            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);
            //playerı serverde yaptık.
            Runner.SetPlayerObject(playerRef, playerObject); // local machine player olucak
        }
    }

    //FOR INPUT
    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();
    }

    #region INetworkRunnerCallbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
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
