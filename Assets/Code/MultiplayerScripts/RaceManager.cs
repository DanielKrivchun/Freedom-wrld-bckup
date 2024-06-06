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



   
   
    [Space]
    public List<_AIData> GenratedAIDetails;
 
    #region PUBLIC
    public TMP_InputField inputField;
    public InputValue InputValue;
    [Space]
    public TMP_Dropdown dropdown;
    [Space]
    public Transform[] spawnPoints;
    [Space]
    public Transform[] WinPoints;
    [Space]
    [Header("Player configs")]
    [Header("NetworkRunner Prefab")]
    public NetworkRunner NetworkRunnerPrefab;
    [Header("Player Prefab")]
    public NetworkPrefabRef PlayerPrefab = NetworkPrefabRef.Empty;
    [Header("AIPlayer Prefab")]
    public NetworkPrefabRef AIPlayer;
    [Space]
    public PrefabHolder PetPrefabHolder;
    [Space]
    public string PrefabID;
    [Space]
    public int CurrntWinCount;
    [Space]
    public int Min;
    public int Max;
    public int MyRank;
    public string LocalPlayerNickname { get; private set; }
    [Space]
    public List<_AllPlayerData> GenratedPlayers;
    #endregion

    #region NETWORKED OBJECTS
    [Networked] public int PathNumber { get; set; }
    [Networked] public int TotalPlayer { get; set; }
    #endregion

    #region PRIVATE

    private Vector2 m_input;
    private bool AFKCheck;
    private float Timer;
    private float match_start_timer;
    private NetworkRunner networkRunnerInstance;
    #endregion

    public static RaceManager instance;

    private string selected_region = "";

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
            Debug.LogError("I AM  AFK KICK ME  NOW");
        }
    }

    public void _CheckHowManyPlayersAreInGame()
    {
        NetworkEventManager._EventNewPlayerJoined(TotalPlayer);
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
            //return;
            selected_region = "asia";
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



        string s1 = "C0 BETWEEN ";
        string s2 = " AND C1";

        Min = MyRank - 50;
        Max = MyRank + 50;

        //string matchmakingstring = "C0 BETWEEN " + Min + " AND " + Max;
        //string matchmakingstring = _GetMyRank();


        //Debug.Log(matchmakingstring);

        var customProps = new Dictionary<string, SessionProperty>();

        customProps["RANK"] = _GetMyRank();
        Debug.Log("MY RANK " + MyRank);

        await networkRunnerInstance.StartGame(new StartGameArgs
        {
            GameMode = mode,
            CustomLobbyName = "MyLobby",
            PlayerCount = 5,
            Scene = scene,
            SceneManager = networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>(),
            SessionProperties = customProps,
            CustomPhotonAppSettings = appSettings

        });


    }

    string _GetMyRank()
    {
        if (MyRank < 50)
        {
            return "A";
        }

        return "B";

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

    public override void Spawned()
    {
        Debug.Log("Spwanded Worked");
    }

    private void _SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (PathNumber <= 0)
            {
                PathNumber = 0;
            }
            Vector3 spawnPoint = spawnPoints[PathNumber].transform.position;
            Debug.Log(spawnPoint);
            NetworkObject playerObject = Runner.Spawn(PlayerPrefab, spawnPoint, Quaternion.identity, playerRef);
            playerObject.transform.position = spawnPoint;
            Debug.Log(playerObject.transform.position);
            playerObject.GetComponent<NavmeshMultiplayer>()._SetUpMyInitialData(PathNumber);
            //playerı serverde yaptık.
            Runner.SetPlayerObject(playerRef, playerObject);
            PathNumber++;
            TotalPlayer++;

            _AllPlayerData d = new _AllPlayerData();
            d.playerRef = playerRef;
            d.networkObject = playerObject;

            GenratedPlayers.Add(d);

            if (PathNumber >= 0)
            {
                NetwrokUI.Instance._OpenStartUI();
            }
        }

        if (Runner.IsClient)
        {
            Debug.Log("I am client so checking for AI player");
            _CheckForAIPlayers();
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
        //throw new NotImplementedException();x
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
        //CHECKING FOR AI PLAYER COUNT
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

    #region AI PLAYER
    void _CheckForAIPlayers()
    {
        Debug.Log("Checking For AI Players");
        //RPC_GetAIDetails();
    }

    public void _GenrateAIPlayer()
    {
        Debug.Log("AI player genration");
        if (PathNumber <= 0)
        {
            PathNumber = 0;
        }
        Vector3 spawnPoint = spawnPoints[PathNumber].transform.position;
        Debug.Log(spawnPoint);
        NetworkObject playerObject = Runner.Spawn(AIPlayer, spawnPoint, Quaternion.identity);
        playerObject.GetComponent<NetworkTransform>().transform.position = spawnPoint;
        Debug.Log(playerObject.transform.position);
        playerObject.GetComponent<NetworkAIPlayer>()._SetUpMyInitialData(PathNumber);
        PathNumber++;
    }

    void _SendAIDetails()
    {
        Debug.Log("I am Sedning AI Details");
    }

    #endregion

    #region RPC CALLS
    [Rpc(RpcSources.InputAuthority, RpcTargets.InputAuthority)]
    public void RPC_GetAIDetails()
    {
        _SendAIDetails();
    }
    #endregion
}

[System.Serializable]
public class _AIData
{
    public string AIName;
}
