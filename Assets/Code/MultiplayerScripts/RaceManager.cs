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
using DG.Tweening;
using Unity.Mathematics;
public class RaceManager : NetworkBehaviour, INetworkRunnerCallbacks
{

    #region PUBLIC
    public InputValue InputValue;
    public TextMeshProUGUI gamestarttimer;
    [Space]
    public TMP_Dropdown dropdown;
    [Space]
    public Transform[] spawnPoints;
    [Space]
    public Transform[] WinPoints;
    [Space]
    public Transform[] StumblePoints;
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
    public PetDataRef petdataref;
    [Space]
    public SceneSyncData SceneData;
    [Space]
    public NamesJson namesJson;
    [Space]
    public string PrefabID;
    [Space]
    public int CurrntWinCount;
    [Space]
    public int Min;
    public int Max;
    [Space]
    public int MyRank;
    public int MyXP;
    public int MyCoins;
    [Space]
    public float TotalSeconds;
    [Space]
    public int TotalNumberOfPlayers;
    public int TotalRealPlayers;
    public int ResetAgrreePlayers;
    public int AIplayersCount;
    public int CompletePlayerCount;
    public int MyWinNumber;
    public string LocalPlayerNickname { get; private set; }
    [Space]
    public List<_AllPlayerData> GenratedPlayers;
    [Space]
    public List<_GenratedAIPlayer> TotalPlayers;
    [Space]
    public List<_AIplayerDetails> AIplayerDetails;
    [Space]
    public List<_RankPlayers> RankBasedPlayers;

    private List<_RankPlayers> temp_list;

    #endregion

    #region NETWORKED OBJECTS
    [Networked] public int PathNumber { get; set; }

    [Networked, OnChangedRender(nameof(_OnWInNumberAlocated))]
    public string TimeLeft { get; set; }

    private bool RaceStart;
    private float timer = 0f;
    #endregion

    #region PRIVATE

    private Vector2 m_input;

    private bool AFKCheck;
    private float Timer;
    [Space]
    private float match_start_timer;
    private NetworkRunner networkRunnerInstance;

    private string selected_region = "";
    private int hours;
    private int minutes;
    private int seconds;


    #endregion

    public PathCreation.PathCreator Path;

    public static RaceManager instance;

    #region UNITY METHODS
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

    private void Start()
    {
        PrefabID = petdataref.petData.petPrefabID.ToString();
        SceneData._Reset();
        namesJson._SetMyData();

        NetworkEventManager.e_countdown_start += _OnCounddownStart;
    }

    private void OnDestroy()
    {
        NetworkEventManager.e_countdown_start -= _OnCounddownStart;
    }


    private void _OnCounddownStart()
    {
        if (Runner.IsServer)
        {
            int a = UnityEngine.Random.Range(0, 3);
            Debug.Log("Stumble point  " + a);
            RPC_SetStumbleObject(a);
            PathNumber = 0;
        }

        ResetAgrreePlayers = 0;
        MyWinNumber = 0;
        RaceStart = true;
        CompletePlayerCount = 0;
    }

    #endregion

    #region AFK KICKING FIXED UPDATE
    void Update()
    {
        if (AFKCheck)
        {
            _CheckForAFK();
        }

        if (networkRunnerInstance != null && networkRunnerInstance.IsServer)
        {
            TotalSeconds -= Time.deltaTime;

            hours = Mathf.FloorToInt(TotalSeconds / 3600);
            minutes = Mathf.FloorToInt(TotalSeconds / 60);
            seconds = Mathf.FloorToInt(TotalSeconds % 60);
            TimeLeft = (minutes) + " : " + seconds;
        }
    }

    public override void FixedUpdateNetwork()
    {
        //CHECK THIS ONLY ON SERVER
        if (Runner == null) return;
        if (!Runner.IsServer) return;
        if (!RaceStart) return;

        timer += Time.deltaTime;

        if (timer > 5f)
        {
            timer = 0f;

            temp_list = new List<_RankPlayers>();
            temp_list = RankBasedPlayers;

            temp_list = temp_list.OrderByDescending(asd => asd.MyDistance).ToList();
            int MyRank = 0;
            foreach (var item in TotalPlayers)
            {
                if (!item.AI)
                {
                    MyRank = temp_list.FindIndex(asd => asd.PathNo == item.player.MyPathNumber);
                    item.player._ChangingRanke(MyRank);
                }
            }

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


    #endregion

    #region GAME START AND MATCHMAKING
    public const string ELO_PROP_KEY = "C0";
    public const string MAP_PROP_KEY = "C1";
    private void _OnWInNumberAlocated()
    {
        gamestarttimer.text = "Game Will Start In :" + TimeLeft;
    }

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


    /// <summary>
    /// STARTING GAME AND JOINING ROOM 
    /// BASED ON LOCATION , RANK 
    /// </summary>
    /// <param name="mode"></param>
    public async void _StartGame(GameMode mode)
    {
        //GET MY NAME XP AND COINS OVER HERE
        LocalPlayerNickname = petdataref.petData.petname;
        MyXP = (int)petdataref.petData.xp;
        MyRank = (int)petdataref.petData.rank;



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

        var customProps = new Dictionary<string, SessionProperty>();

        customProps["RANK"] = _GetMyRank();
        Debug.Log("MY RANK " + MyRank);

        await networkRunnerInstance.StartGame(new StartGameArgs
        {
            GameMode = mode,
            //CustomLobbyName = "MyLobby",
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

    #region PLAYER SYNC
    /// <summary>
    /// Getting player config in player object based on RPC calls
    /// </summary>
    /// <returns></returns>
    public _PlayerConfigs _GetMyCOnfigs()
    {
        _PlayerConfigs P = new _PlayerConfigs();
        P.running = petdataref.petData.running;
        P.climbing = petdataref.petData.climbing;
        P.flying = petdataref.petData.flying;
        P.swimming = petdataref.petData.swimming;
        P.intelligence = petdataref.petData.intelligence;
        P.luck = petdataref.petData.luck;
        P.rank = petdataref.petData.rank;
        P.maxStamina = petdataref.petData.maxStamina;
        P.mycoins = MyCoins;
        P.xp = petdataref.petData.xp;
        return P;
    }

    #endregion

    #region ON RACE COMPLETE CACLULATION CP AND COINS
    public void _CheckAllPlayerCompleted()
    {
        CompletePlayerCount++;

        Debug.Log(CompletePlayerCount + "    " + TotalNumberOfPlayers);

        if (CompletePlayerCount == TotalNumberOfPlayers)
        {
            List<string> _ss = new List<string>();

            for (int i = 1; i < TotalNumberOfPlayers + 1; i++)
            {
                foreach (var item in TotalPlayers)
                {
                    if (!item.AI)
                    {
                        if (i == item.player.MyWiningNumber)
                        {
                            _ss.Add(item.player.MyName);
                        }
                    }
                    else
                    {
                        if (i == item.aiplayer.MyWiningNumber)
                        {
                            _ss.Add(item.aiplayer.MyName);
                        }
                    }
                }
            }

            NetwrokUI.Instance._SetupList(_ss);
            _XpIncrimental(MyWinNumber);
            SceneData.ShowWelcomeScreen = true;
            _ResetDataOnComplete();
        }
    }

    private void _ResetDataOnComplete()
    {
        ResetAgrreePlayers = 0;
        MyWinNumber = 0;
        PathNumber = 0;
        CompletePlayerCount = 0;
        RaceStart = false;
    }

    public void _ResetRaceManager()
    {
        Debug.Log(ResetAgrreePlayers);
        ResetAgrreePlayers++;

        if (ResetAgrreePlayers == TotalRealPlayers)
        {
            StartCoroutine(_ReplayGameAgain());
            Debug.Log("All Players agreed to match ");
            Debug.Log("Start Game Now");
        }
    }

    IEnumerator _ReplayGameAgain()
    {
        //DE SPWAN ALL AI PLAYERS
        _DespwanAllAIplayers();
        yield return new WaitForSecondsRealtime(1);

        for (int i = 1; i <= 5; i++)
        {
            if (i > TotalRealPlayers)
            {
                _GenrateAIPlayer();
                yield return new WaitForEndOfFrame();
            }
        }

        yield return new WaitForSecondsRealtime(1);
        NetwrokUI.Instance.RPC_StartGame();
    }


    public void _XpIncrimental(int _mywinno)
    {
        int mul = 1;
        int coinstoadd = 0;
        switch (_mywinno)
        {
            case 1:
                mul = 50;
                coinstoadd = 80;
                break;
            case 2:
                mul = 30;
                coinstoadd = 60;
                break;
            case 3:
                mul = 20;
                coinstoadd = 10;
                break;
        }
        int newxp = (MyXP) + (1 / MyRank) * mul;
        MyCoins += coinstoadd;
        MyXP += newxp;
        Debug.Log(newxp);
        Debug.Log(coinstoadd);

        SceneData.XpGained += newxp;
        SceneData.CoinsGained += coinstoadd;

        //SHOW TEXT
        NetwrokUI.Instance._UpdatedText(coinstoadd, newxp);

        //ADD COINS FROM BeamableInventoryManager AddCurrency
        //AND XP WILL BE CALCULATED IN NEXT SCENE
    }

    #endregion

    #region ON RACE STARTS
    public void _StartGameForPlayers()
    {
        //Debug.Log("_StartGameForPlayers");
        NetworkEventManager._EventStartGame();
        NetworkEventManager._EventCameraChange(_CamState.Follow);
        InputValue.m_enable_navmesh = true;
    }
    #endregion

    #region PATHNUMBER SETUP
    public int _GetPathNo()
    {
        int a = PathNumber;
        PathNumber++;
        return a;
    }
    #endregion

    #region WIN LOGIC
    public int _GetMyWinningNo()
    {
        CurrntWinCount++;
        return CurrntWinCount;
    }
    #endregion

    #region PLAYER SPWANR AND DESPWAN

    private void _SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            //CHECK HERE FOR TOTAL NULBER OF PLAYERS
            if (TotalNumberOfPlayers >= 5)
            {
                TotalNumberOfPlayers--;
                Debug.Log("Players are morethen 5 or 5 ");
                //REMOVE AI PLAYER HERE AND ADD REAL PLAYER
                int newpathno = _DespwanAIplayer();
                Vector3 spwanp = spawnPoints[newpathno].transform.position;
                _SpwanPlayerCalculations(playerRef, newpathno, spwanp);
                return;
            }

            if (PathNumber <= 0)
            {
                PathNumber = 0;
            }
            Vector3 spawnPoint = spawnPoints[PathNumber].transform.position;
            _SpwanPlayerCalculations(playerRef, PathNumber, spawnPoint);
            PathNumber++;
            if (PathNumber >= 0)
            {
                NetwrokUI.Instance._OpenStartUI();
            }
        }
    }

    void _SpwanPlayerCalculations(PlayerRef _playerRef, int _pathno, Vector3 _spwanpos)
    {
        NetworkObject playerObject = Runner.Spawn(PlayerPrefab, _spwanpos, Quaternion.identity, _playerRef);
        playerObject.transform.position = _spwanpos;
        playerObject.GetComponent<NavmeshMultiplayer>()._SetUpMyInitialData(_pathno);
        Runner.SetPlayerObject(_playerRef, playerObject);
        _AllPlayerData d = new _AllPlayerData();
        d.Player = playerObject;
        d.playerRef = _playerRef;
        GenratedPlayers.Add(d);
    }

    private int _DespwanAIplayer()
    {
        if (Runner.IsServer)
        {
            int a = 0;
            foreach (var item in TotalPlayers)
            {
                if (item.AI)
                {
                    a = item.aiplayer.MyPathNumber;
                    Debug.Log(a);
                    Destroy(item.aiplayer.gameObject);
                    TotalPlayers.Remove(item);
                    return a;
                }
            }
        }
        Debug.Log("No AI player Found");
        return 4;
    }

    private void _DespwanAllAIplayers()
    {
        if (Runner.IsServer)
        {
            foreach (var item in TotalPlayers)
            {
                if (item.AI)
                {
                    Destroy(item.aiplayer.gameObject);
                    TotalNumberOfPlayers--;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                foreach (var item in TotalPlayers)
                {
                    if (item.AI)
                    {
                        TotalPlayers.Remove(item);
                        break;
                    }
                }
            }

        }
    }

    private void _DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            //NOTIFY TO PLAYER WHICH PLAYER LEFT
            _AllPlayerData p = GenratedPlayers.Find(asd => asd.playerRef == playerRef);

            if (p.playerRef != null)
            {
                //NOTIFY TO PLAYER WHICH PLAYER LEFT
                Debug.Log(p.Player.GetComponent<NavmeshMultiplayer>().MyName);
                RPC_PlayerLeftNofirication(p.Player.GetComponent<NavmeshMultiplayer>().MyName);
                Runner.Despawn(p.Player);
                GenratedPlayers.Remove(GenratedPlayers.Find(asd => asd.playerRef == playerRef));
                TotalNumberOfPlayers--;
                TotalRealPlayers--;
            }
        }
    }
    #endregion

    #region INetworkRunnerCallbacks ALSO SETTING INPUT OVER HERE

    //FOR INPUTx
    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        //CHECK OF ROOM IS FULL IF FULL THEN DESPWAN PLAYER
        _SpawnPlayer(player);
        //CHECKING FOR AI PLAYER COUNT
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
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

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
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
        Vector3 spawnPoint = spawnPoints[PathNumber].transform.position;
        Debug.Log(spawnPoint);
        NetworkObject playerObject = Runner.Spawn(AIPlayer, spawnPoint, Quaternion.identity);
        playerObject.GetComponent<NetworkTransform>().transform.position = spawnPoint;
        Debug.Log(playerObject.transform.position);
        playerObject.GetComponent<NetworkAIPlayer>()._SetUpMyInitialData(PathNumber);
        PathNumber++;
    }

    #endregion

    #region RANK FINDER
    public float _FindMyDistance(Vector3 _pos)
    {
        return Path.path.GetClosestDistanceAlongPath(_pos);
    }

    #endregion

    #region RPC CALLS

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerLeftNofirication(string _name)
    {
        _PlayerLeftDetails(_name);
    }

    void _PlayerLeftDetails(string s)
    {
        Debug.Log("I am Getting  Details  " + s);
        NetworkEventManager._EventPlayerLeft(s);
        NetworkEventManager._EventOnStopPlayer(1);

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetStumbleObject(int a)
    {
        _SetStumbleObjec(a);
    }

    void _SetStumbleObjec(int a)
    {
        StumblePoints[a].gameObject.SetActive(true);
    }

    #endregion
}
