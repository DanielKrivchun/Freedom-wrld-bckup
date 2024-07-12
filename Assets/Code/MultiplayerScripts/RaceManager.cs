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
using System.Threading.Tasks;
using Unity.VisualScripting;
public class RaceManager : NetworkBehaviour
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
    public _StumbleObjects[] StumblePoints_1;
    public _StumbleObjects[] StumblePoints_2;
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
    public _FakeUsers AINames;
    public List<int> usedNames;
    //[Space]
    //public NamesJson namesJson;
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

    private bool XpCalculations;
    public string LocalPlayerNickname { get; private set; }
    public string LocalnetworkID { get; set; }
    [Space]
    public List<_AllPlayerData> GenratedPlayers;
    [Space]
    public List<_GenratedAIPlayer> TotalPlayers;
    [Space]
    public List<_AIplayerDetails> AIplayerDetails;
    [Space]
    public List<_RankPlayers> RankBasedPlayers;

    private List<_RankPlayers> temp_list;

    private bool aigenration;
    public bool startgamenow;

    #endregion

    #region NETWORKED OBJECTS
    [Networked] public int PathNumber { get; set; }

    [Networked, OnChangedRender(nameof(_OnTimerChanged))]
    public string TimeLeft { get; set; }

    public bool RaceStart;
    private float timer = 0f;
    #endregion

    #region PRIVATE

    public Vector2 m_input;

    private bool AFKCheck;
    private float Timer;
    [Space]
    private float match_start_timer;
    private NetworkRunner networkRunnerInstance;
    private RunnerHandller runnerhandller;

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
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        PrefabID = petdataref.petData.petPrefabID.ToString();

        usedNames.Clear();
        NetworkEventManager.e_countdown_start += _OnCounddownStart;
    }

    private void OnDestroy()
    {
        NetworkEventManager.e_countdown_start -= _OnCounddownStart;
    }
    #endregion

    #region EVENT CALLBACKS
    private void _OnCounddownStart()
    {
        if (Runner.IsServer)
        {
            int a = UnityEngine.Random.Range(0, 3);
            PathNumber = 0;
        }

        ResetAgrreePlayers = 0;
        MyWinNumber = 0;
        usedNames.Clear();
        CompletePlayerCount = 0;
        CurrntWinCount = 0;
        _ClearUnwantedPlayers();
        StartCoroutine(_WaitAndCountPlayers());
    }

    IEnumerator _WaitAndCountPlayers()
    {
        yield return new WaitForSecondsRealtime(1f);
        TotalNumberOfPlayers = TotalPlayers.Count;
    }

    private void _ClearUnwantedPlayers()
    {

        for (int i = 0; i < 5; i++)
        {
            foreach (var item in TotalPlayers)
            {

                if (item.AI)
                {
                    if (item.aiplayer == null)
                    {
                        TotalPlayers.Remove(item);
                        _ClearUnwantedPlayers();
                        return;
                    }
                }
                else
                {
                    if (item.player == null)
                    {
                        Debug.Log("Removed player");
                        TotalPlayers.Remove(item);
                        _ClearUnwantedPlayers();
                        return;
                    }
                }
            }
        }

    }
    #endregion

    #region AI PLAYER NAMES
    public async void _SetMyData()
    {
        TextAsset s = Resources.Load("Names") as TextAsset;
        AINames = JsonUtility.FromJson<_FakeUsers>(s.ToString());
        await Utils._Waiter(500);
    }

    public string _GetNames()
    {
        int a = _GetRandomNumber();

        Debug.Log("Random Number " + a);
        if (usedNames.Contains(a))
        {
            a = _GetRandomNumber();
        }

        return AINames.Names[a];
    }

    private int _GetRandomNumber()
    {
        Debug.Log("Name list count " + AINames.Names.Count);
        return UnityEngine.Random.Range(0, AINames.Names.Count);
    }
    #endregion

    #region AFK KICKING FIXED UPDATE
    void Update()
    {
        if (AFKCheck)
        {
            _CheckForAFK();
        }

        if (networkRunnerInstance != null && networkRunnerInstance.IsServer && !RaceStart)
        {
            TotalSeconds -= Time.deltaTime;
            hours = Mathf.FloorToInt(TotalSeconds / 3600);
            minutes = Mathf.FloorToInt(TotalSeconds / 60);
            seconds = Mathf.FloorToInt(TotalSeconds % 60);
            TimeLeft = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (Runner != null)
            {
                if (minutes <= 0 && Runner.IsServer)
                {
                    if (seconds < 10f && !aigenration)
                    {
                        aigenration = true;
                        StartCoroutine(_GenrateAIPlayerSlowly());
                    }

                    if (seconds <= 0 && !startgamenow)
                    {
                        startgamenow = true;
                        if (!RaceStart)
                        {
                            NetwrokUI.Instance._StartRace();
                        }
                    }
                }
            }
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
                if (!item.AI && item.player != null)
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
            //Debug.LogError("I AM  AFK KICK ME  NOW");
        }
    }


    #endregion

    #region GAME START AND MATCHMAKING
    public const string ELO_PROP_KEY = "C0";
    public const string MAP_PROP_KEY = "C1";
    private void _OnTimerChanged()
    {
        gamestarttimer.text = "Game will start in..." + TimeLeft;
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
        runnerhandller = networkRunnerInstance.GetComponent<RunnerHandller>();
        networkRunnerInstance.AddCallbacks(runnerhandller);
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
            IsVisible = true,
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

    #region LEAVE GAME
    public async Task _LeaveGame()
    {
        if (networkRunnerInstance != null)
        {
            if (Runner.IsServer)
            {
                Debug.Log("Host leaved");
                await networkRunnerInstance.Shutdown(shutdownReason: ShutdownReason.HostMigration);
            }
            else
            {
                Debug.Log("Client leaved");
                await networkRunnerInstance.Shutdown(shutdownReason: ShutdownReason.GameClosed);
            }

        }
    }

    #endregion

    #region INPUT SET
    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();
        runnerhandller.m_input = m_input;

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
                        if (item.player != null)
                        {
                            if (i == item.player.MyWiningNumber)
                            {
                                _ss.Add(item.player.MyName);
                            }
                        }

                    }
                    else
                    {
                        if (i == item.aiplayer.MyWiningNumber)
                        {
                            _ss.Add(item.aiplayer.MyName + " AI");
                        }
                    }
                }
            }

            NetwrokUI.Instance._SetupList(_ss);
            if (XpCalculations)
            {
                _XpIncrimental(MyWinNumber);
            }
            else
            {
                //SHOW TEXT
                NetwrokUI.Instance._UpdatedText(0, 0);
            }
            _ResetDataOnComplete();
            NetworkEventManager._EventDisableNameTags();
            NetworkEventManager._EventGameComplete();
        }
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
            case 4:
                mul = 0;
                break;
            case 5:
                mul = 0;
                break;
        }
        int newxp = (MyXP) + (1 / MyRank) * mul;
        MyCoins += coinstoadd;
        MyXP += newxp;
        Debug.Log(newxp);
        Debug.Log(coinstoadd);

        int C = PlayerPrefs.GetInt(_Strings.CoinsToAdd);
        int X = PlayerPrefs.GetInt(_Strings.XpToAdd);

        C += coinstoadd;
        X += newxp;

        PlayerPrefs.SetInt(_Strings.CoinsToAdd, C);
        PlayerPrefs.SetInt(_Strings.XpToAdd, X);
        PlayerPrefs.SetInt(_Strings.DatFromRaceScene, 1);



        Debug.Log("My XP incrimental is " + X);
        Debug.Log("My Coins incrimental is " + C);

        //SHOW TEXT
        NetwrokUI.Instance._UpdatedText(coinstoadd, newxp);

        //ADD COINS FROM BeamableInventoryManager AddCurrency
        //AND XP WILL BE CALCULATED IN NEXT SCENE
    }

    #endregion

    #region RESET ON GAME COMPLETE

    private void _ResetDataOnComplete()
    {
        ResetAgrreePlayers = 0;
        MyWinNumber = 0;
        PathNumber = 0;
        CompletePlayerCount = 0;
        RaceStart = false;
        CurrntWinCount = 0;
        AFKCheck = true;
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
    #endregion

    #region ON RACE STARTS
    public void _StartGameForPlayers()
    {
        networkRunnerInstance.SessionInfo.IsVisible = false;
        NetworkEventManager._EventStartGame();
        NetworkEventManager._EventCameraChange(_CamState.Follow);
        InputValue.m_enable_navmesh = true;
        RaceStart = true;
        startgamenow = true;
        aigenration = true;
        if (TotalRealPlayers >= 2)
        {
            XpCalculations = true;
        }
        else
        {
            XpCalculations = false;
        }
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
    public void _SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner == null) return;
        if (Runner.IsServer)
        {
            //CHECK HERE FOR TOTAL NULBER OF PLAYERS
            if (TotalNumberOfPlayers >= 5)
            {
                TotalNumberOfPlayers--;
                //Debug.Log("Players are morethen 5 or 5 ");
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

            _RacePlayersCalculation();

        }
        else
        {
            NetwrokUI.Instance._StartUIforClients();
        }
    }


    void _RacePlayersCalculation()
    {
        if (TotalRealPlayers >= 2)
        {
            StartCoroutine(_GenrateAIPlayerSlowly());
        }
    }

    IEnumerator _GenrateAIPlayerSlowly()
    {
        networkRunnerInstance.SessionInfo.IsVisible = false;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            _GenrateAIPlayer();
        }
        NetwrokUI.Instance.StartGameButton.gameObject.SetActive(true);
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
        else
        {
            TotalNumberOfPlayers = TotalRealPlayers;
        }
    }

    public void _DespawnPlayer(PlayerRef playerRef)
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
                _ClearUnwantedPlayers();
            }
        }

        TotalRealPlayers--;
        TotalNumberOfPlayers--;
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
        if (PathNumber == 5) return;
        Debug.Log("AI player genration " + PathNumber);
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

    #region STUMBLE & JACK
    public void _ActivateJack(int _jackno, int pathno)
    {
        Transform T = null;
        switch (_jackno)
        {
            case 1:
                T = StumblePoints_1[pathno].CannonPosition.transform;
                T.DOLocalMoveY(0f, 0.2f).OnComplete(() =>
                {
                    StumblePoints_1[pathno].ParticleEffect.SetActive(true);
                    StartCoroutine(_WaitAndDisableCannon(T));
                });
                break;
            case 2:
                T = StumblePoints_2[pathno].CannonPosition.transform;
                T.DOLocalMoveY(0f, 0.2f).OnComplete(() =>
                {
                    StumblePoints_2[pathno].ParticleEffect.SetActive(true);
                    StartCoroutine(_WaitAndDisableCannon(T));
                });
                break;
        }
    }

    IEnumerator _WaitAndDisableCannon(Transform obj)
    {
        yield return new WaitForSecondsRealtime(2f);
        obj.DOLocalMoveY(-2f, 0.1f);

    }

    #endregion

    #region RPC CALLS

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerLeftNofirication(string _name)
    {
        _PlayerLeftDetails(_name);
    }

    //[Rpc(RpcSources.All, RpcTargets.InputAuthority)]
    //public void RPC_PlayerFinishedRace()
    //{
    //    List<string> _ss = new List<string>();

    //    for (int i = 1; i < TotalNumberOfPlayers + 1; i++)
    //    {
    //        foreach (var item in TotalPlayers)
    //        {
    //            if (!item.AI)
    //            {
    //                if (item.player.MyWiningNumber == 0)
    //                {
    //                    _ss.Add("");
    //                    break;
    //                }

    //                if (i == item.player.MyWiningNumber)
    //                {
    //                    _ss.Add(item.player.MyName);
    //                    break;
    //                }
    //            }
    //            else
    //            {
    //                if (item.aiplayer.MyWiningNumber == 0)
    //                {
    //                    _ss.Add("");
    //                    break;

    //                }
    //                if (i == item.aiplayer.MyWiningNumber)
    //                {
    //                    _ss.Add(item.aiplayer.MyName);
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    NetwrokUI.Instance._SetupList(_ss);
    //}

    void _PlayerLeftDetails(string s)
    {
        Debug.Log("I am Getting  Details  " + s);
        NetworkEventManager._EventPlayerLeft(s);
        NetworkEventManager._EventOnStopPlayer(1);

    }

    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public void RPC_SetStumbleObject(int a)
    //{
    //    _SetStumbleObjec(a);
    //}

    //void _SetStumbleObjec(int a)
    //{
    //    //StumblePoints[a].gameObject.SetActive(true);
    //}

    #endregion
}
