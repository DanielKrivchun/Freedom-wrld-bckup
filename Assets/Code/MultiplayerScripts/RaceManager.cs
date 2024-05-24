using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Fusion.Sockets;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;
using System.Linq;
using Fusion.Photon.Realtime;
using UnityEngine.UI;
public class RaceManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public InputValue InputValue;
    [Space]
    public TMP_Dropdown dropdown;
    [Space]
    public Transform[] spawnPoints;
    [Space]
    public Transform[] WinPoints;
    [Header("NetworkRunner Prefab")]
    public NetworkRunner NetworkRunnerPrefab;
    [Space]
    public PrefabHolder PetPrefabHolder;
    [Space]
    [Header("Player Prefab")]
    public NetworkPrefabRef PlayerPrefab = NetworkPrefabRef.Empty;

    int a = 0;
    [SerializeField] private TMP_InputField inputField;
    public Vector2 m_input;
    [Networked] public int PathNumber { get; set; }
    public string PrefabID;
    public int CurrntWinCount;

    private bool AFKCheck;
    private float Timer;
    [Space]
    public int Min;
    public int Max;

    private NetworkRunner networkRunnerInstance;

    public string LocalPlayerNickname { get; private set; }

    public static RaceManager instance;

    public List<_AllPlayerData> GenratedPlayers;

    private string selected_region;

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


        string s1 = "C0 BETWEEN";
        string s2 = "AND C1";

        string final = s1 + " " + Min.ToString() + " AND " + Max.ToString() + s2;

        Debug.Log(final);
        Debug.Log("C0 BETWEEN 345 AND 475 AND C1");
    }



    #region AFK KICKING


    void Update()
    {
        if (AFKCheck)
        {
            _CheckForAFK();
        }
    }

    void _CheckForAFK()
    {
        if (Input.touchCount > 0)
        {
            Timer = 0f;
        }

        Timer += Time.deltaTime;

        if (Timer > 20f)
        {
            Debug.LogError("I AM  AFK KICK ME  NOW ");
        }
    }

    void _CheckHowManyPlayersAreInGame()
    {
        int a = networkRunnerInstance.ActivePlayers.Count();
        Debug.Log("Total Players" + a);
    }

    #endregion


    public const string ELO_PROP_KEY = "C0";
    public const string MAP_PROP_KEY = "C1";

    #region GAME START AND MATCHMAKING

    public void _SelectRegion(int typedText)
    {
        Debug.Log(typedText);
        Debug.Log(dropdown.options[typedText].text);
        selected_region = dropdown.options[typedText].text;
    }

    /// <summary>
    /// SET"S regioun
    /// </summary>
    /// <param name="region"></param>
    /// <param name="customAppID"></param>
    /// <param name="appVersion"></param>
    /// <returns></returns>
    private FusionAppSettings BuildCustomAppSetting(string region, string customAppID = null, string appVersion = "1.0.0")
    {

        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy(); ;

        appSettings.UseNameServer = true;
        appSettings.AppVersion = appVersion;

        if (string.IsNullOrEmpty(customAppID) == false)
        {
            appSettings.AppIdFusion = customAppID;
        }

        if (string.IsNullOrEmpty(region) == false)
        {
            appSettings.FixedRegion = region.ToLower();
        }

        // If the Region is set to China (CN),
        // the Name Server will be automatically changed to the right one
        // appSettings.Server = "ns.photonengine.cn";

        return appSettings;
    }


    public async void _StartGame(GameMode mode)
    {
        LocalPlayerNickname = inputField.text;

        if (selected_region.Length <= 0)
        {
            Debug.LogError("SELECT REGION");
            return;
        }

        var appSettings = BuildCustomAppSetting(selected_region);

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


        var customProps = new Dictionary<string, SessionProperty>();

        string s1 = "C0 BETWEEN";
        string s2 = "AND C1";

        string final = s1 + " " + Min.ToString() + " AND " + Max.ToString() + s2;

        Debug.Log(final);

        string sqlLobbyFilter = "C0 BETWEEN 345 AND 475 AND C1";
        customProps["RANK"] = sqlLobbyFilter;


        await networkRunnerInstance.StartGame(new StartGameArgs
        {
            GameMode = mode,
            CustomLobbyName = "MyLobby",
            SessionName = "TestRaceMap",
            PlayerCount = 5,
            Scene = scene,
            SceneManager = networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>(),
            SessionProperties = customProps,
            CustomPhotonAppSettings = appSettings
        });


    }

    #endregion


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

            _AllPlayerData d = new _AllPlayerData();
            d.playerRef = playerRef;
            d.networkObject = playerObject;

            GenratedPlayers.Add(d);

            if (PathNumber >= 2)
            {
                NetwrokUI.Instance._OpenStartUI();
            }
        }
    }

    private void _DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            _AllPlayerData p = GenratedPlayers.Find(asd => asd.playerRef == playerRef);

            if (p.networkObject != null)
            {
                Runner.Despawn(p.networkObject);
                GenratedPlayers.Remove(GenratedPlayers.Find(asd => asd.playerRef == playerRef));
            }
        }


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
        _CheckHowManyPlayersAreInGame();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
        _DespawnPlayer(player);
        _CheckHowManyPlayersAreInGame();
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
