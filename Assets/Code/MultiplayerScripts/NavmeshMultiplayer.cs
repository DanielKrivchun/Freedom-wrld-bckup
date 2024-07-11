using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
public class NavmeshMultiplayer : NetworkBehaviour, IBeforeUpdate
{
    #region PUBLIC VARIABLES
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    public PetConfigs PetConfigs;
    [Space]
    public TextMeshPro nameText;
    [Header("Navmesh Agent")]
    public NavMeshAgent m_agent;
    [Header("ANIMATOR")]
    public PetAnimation GenratedPet;
    [Header("Script Refrence")]
    private PathPointManager path_point;
    [Space]
    public GameObject NameTextObject;
    [Space]
    public ParticleSystem FootDustEffect_1;
    public ParticleSystem FootDustEffect_2;
    public ParticleSystem SwimmingEffect;
    public ParticleSystem FlyingEffect;
    public ParticleSystem Stunned;

    public NetworkString<_32> playerName;
    public PlayerRef playerRef;
    public List<Vector3> move_positions;
    [Space]
    public _PlayerConfigs playerconfigs;
    [Space]
    public float MySpeed;
    public float MyStamina;
    [Space]
    //public int MyRankNo;
    [Space]
    private float SpeedController = 1f;

    [Space]
    public float Luck;
    public float Inteligence;

    public float LuckMax { get; private set; }
    public float InteligenceMax { get; private set; }
    #endregion

    #region Private variables
    private Vector3 CurruntPos;
    private float Distance;
    private int CurruntIndex;
    public bool IsServer;
    public bool IsLocalPlayer;
    public bool RaceComplete;
    [Space]
    public float MyDistanceOnPath;
    [Space]
    public float NetworkedSetTap;
    public float MyNetworkSpeed;

    private NavMeshHit hit;
    private Vector3 finalPosition;
    private Vector3 WinPosition = Vector3.zero;

    private bool CheckStumbleNow = false;

    #endregion

    #region NETWORKED OBJECTS

    [Networked] public string MyName { get; set; }
    [Networked] public int MyPathNumber { get; set; }
    [Networked] public string MyPrefabID { get; set; }

    [Networked] public Vector3 MyPos { get; set; }


    [Networked, OnChangedRender(nameof(_OnWInNumberAlocated))]
    public int MyWiningNumber { get; set; }

    public float luckchance = 0f;
    private float RandomNumber;

    public NetworkTransform networkTransform;
    private Vector3 pos;

    private RaceManager RaceManagerRef;

    [Networked, OnChangedRender(nameof(_OnRankNumberChanged))]
    public int MyRankNo { get; set; }

    private string terrain = _Tags.Land;

    private bool Stumbled = false;
    private bool inteligencecheck = false;

    public Quaternion Q { get; private set; }

    #endregion

    #region NETWORK FUCTIONS
    public override void Spawned() // fusionun startı
    {
        NetworkEventManager.e_player_speed_change += _OnStopStartPlayer;
        NetworkEventManager.e_reset_player += _ResetMe;
        NetworkEventManager.e_host_migration_done += _HostMigrated;
        RaceManagerRef = RaceManager.instance;
        path_point = FindObjectOfType<PathPointManager>();
        SetLocalObjects();
        _SetupConfigs();
        networkTransform = GetComponent<NetworkTransform>();
        if (Runner.IsServer)
        {
            IsServer = true;
        }
        else
        {
            m_agent.enabled = false;
        }
        StartCoroutine(_GenrateMyPrefab());
        //Debug.Log(Object.Id);
    }

    private void OnDestroy()
    {
        NetworkEventManager.e_player_speed_change -= _OnStopStartPlayer;
        NetworkEventManager.e_reset_player -= _ResetMe;
        NetworkEventManager.e_host_migration_done -= _HostMigrated;
    }

    private void _HostMigrated()
    {
        string s = JsonUtility.ToJson(RaceManager.instance._GetMyCOnfigs());
        RPC_GetMyConfigs(s);
    }

    private void _OnStopStartPlayer(int _no)
    {
        SpeedController = _no;

        if (_no == 0)
        {
            _ChangeAnimationHere(_AnimState.Idle);
        }
        else
        {
            _ChangeAnimationHere(_AnimState.Run);
        }
    }

    IEnumerator _GenrateMyPrefab()
    {
        yield return new WaitForSecondsRealtime(1f);
        //Debug.Log("This Choroutine Worked " + gameObject.name);
        _GenratePetPrefab();
    }

    /// <summary>
    /// Remove random on Actual Project
    /// </summary>
    void _SetupConfigs()
    {
        m_agent.speed = Random.Range(3, PetConfigs.BaseSpeed);
        m_agent.acceleration = Random.Range(15, PetConfigs.Acceleration);
    }

    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            RaceManagerRef.LocalnetworkID = Object.Id.ToString();
            IsLocalPlayer = true;
            playerName = RaceManager.instance.LocalPlayerNickname;
            Debug.Log("Sending RPC with Name   " + playerName);
            MyName = playerName.ToString();
            RPC_SetNameAndPrefab(playerName, RaceManager.instance.PrefabID);

            _SetMyConfigs();

            //RPC FOR SENDING CONFIGS
            string s = JsonUtility.ToJson(RaceManager.instance._GetMyCOnfigs());
            //Debug.Log(s + "    " + MyName);
            RPC_GetMyConfigs(s);
            _SetName(playerName.ToString());
            gameObject.name = MyName.ToString();
            _SetupCamera();
        }
        else
        {
            //Debug.Log("My Name Is " + MyName);
            gameObject.name = MyName.ToString();
            _SetName(MyName);
        }
    }

    void _SetName(string _name)
    {
        nameText.text = _name;
    }

    #endregion

    #region RESET PLAYER
    public void _ResetMe(string _name, string _id)
    {
        Debug.Log(_id + "   " + Object.Id);
        if (Object.Id.ToString() == _id)
        {
            Debug.Log("Reseting Me " + MyName);
            MyPathNumber = RaceManagerRef._GetPathNo();
            pos = RaceManager.instance.spawnPoints[MyPathNumber].position;
            Q = RaceManager.instance.spawnPoints[MyPathNumber].rotation;
            RPC_ChangeMyAnimation(0);
            CurruntIndex = 0;
            MyWiningNumber = 0;
            RaceManager.instance._ResetRaceManager();
            NameTextObject.transform.localScale = Vector3.one;
            _SetupCamera();
        }
    }
    #endregion

    #region UNITY METHODS

    private void OnEnable()
    {
        NetworkEventManager.e_get_set_go += _StartRun;
        NetworkEventManager.e_diabled_names += _GameComplete;
    }

    private void OnDisable()
    {
        NetworkEventManager.e_get_set_go -= _StartRun;
        NetworkEventManager.e_diabled_names -= _GameComplete;
    }

    private void _GameComplete()
    {
        NameTextObject.transform.localScale = Vector3.zero;
    }
    #endregion

    #region SETUP CONFIGS
    private void _SetMyConfigs()
    {
        PetConfigs.m_run_multiplier = playerconfigs.running;
        PetConfigs.m_swim_multiplier = playerconfigs.swimming;
        PetConfigs.m_climb_multiplier = playerconfigs.climbing;
        PetConfigs.m_fly_multiplier = playerconfigs.flying;
        PetConfigs.MyXp = (int)playerconfigs.xp;
        PetConfigs.MyCoins = playerconfigs.mycoins;
        PetConfigs.maxstemina = playerconfigs.maxStamina;

        RaceManager.instance.MyXP = PetConfigs.MyXp;
        RaceManager.instance.MyCoins = PetConfigs.MyCoins;

        MySpeed = PetConfigs.BaseSpeed + _GetMyStateMultiplier(playerconfigs.running);


        Luck = Mathf.Pow(2.718f, -1.5f * (playerconfigs.luck / 5000f));
        Inteligence = Mathf.Pow(2.718f, -1.5f * (playerconfigs.intelligence / 5000f));

        //3
        //0.9999
        LuckMax = 0.3f * Luck;
        InteligenceMax = 0.6f * Inteligence;


        //Debug.Log(Luck);
        Debug.Log(Inteligence);

        NetworkEventManager._EventConfigUpdated();
    }

    float _GetMyStateMultiplier(float _value)
    {
        return (1 + (_value / 100f));
        //return 10f;
    }

    #endregion

    #region COLISION DETECTION

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        if (Runner.IsServer)
        {
            switch (other.tag)
            {
                case _Tags.WinLine:
                    Debug.Log("WINLINE COLIDED" + other.tag + "    " + MyName);
                    m_agent.enabled = false;
                    GetComponent<Collider>().enabled = false;
                    Rigidbody rb = GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    MyWiningNumber = RaceManager.instance._GetMyWinningNo();
                    RaceComplete = true;

                    if (IsLocalPlayer)
                    {
                        NetworkEventManager._EventCameraChange(_CamState.DisableFollow);
                    }

                    int temp = MyWiningNumber - 1;
                    pos = RaceManager.instance.WinPoints[temp].position;
                    Q = RaceManager.instance.WinPoints[temp].rotation;
                    StartCoroutine(SetMyWinPosition());
                    break;
                case _Tags.Water:
                    MySpeed = PetConfigs.BaseSpeed + _GetMyStateMultiplier(playerconfigs.swimming);
                    _ChangeAnimationHere(_AnimState.Swimming);
                    terrain = _Tags.Water;
                    break;
                case _Tags.Flying:
                    MySpeed = PetConfigs.BaseSpeed + _GetMyStateMultiplier(playerconfigs.flying);
                    _ChangeAnimationHere(_AnimState.Flying);
                    terrain = _Tags.Flying;
                    break;
                case _Tags.Land:
                    luckchance = 0f;
                    MySpeed = PetConfigs.BaseSpeed + _GetMyStateMultiplier(playerconfigs.running);
                    _ChangeAnimationHere(_AnimState.Run);
                    terrain = _Tags.Land;
                    break;
                case _Tags.Climbing:
                    MySpeed = PetConfigs.BaseSpeed + _GetMyStateMultiplier(playerconfigs.climbing);
                    _ChangeAnimationHere(_AnimState.Climbing);
                    terrain = _Tags.Climbing;
                    break;
                case _Tags.Jack:
                    //INTELIGENCE CHECK 
                    RandomNumber = Random.Range(0f, 1f);
                    Debug.Log(RandomNumber);
                    if (RandomNumber <= InteligenceMax)
                    {
                        _ColidedWIthJack(1);
                    }
                    break;
                case _Tags.JackSecond:
                    RandomNumber = Random.Range(0f, 1f);
                    if (RandomNumber <= InteligenceMax)
                    {
                        _ColidedWIthJack(2);
                    }

                    break;
            }
        }
        else
        {
            switch (other.tag)
            {
                case _Tags.Water:
                    _ChangeAnimationHere(_AnimState.Swimming);
                    terrain = _Tags.Water;
                    break;
                case _Tags.Flying:
                    _ChangeAnimationHere(_AnimState.Flying);
                    terrain = _Tags.Flying;
                    break;
                case _Tags.Land:
                    _ChangeAnimationHere(_AnimState.Run);
                    terrain = _Tags.Land;
                    break;
                case _Tags.Climbing:
                    _ChangeAnimationHere(_AnimState.Climbing);
                    terrain = _Tags.Climbing;
                    break;
            }
        }
    }
    #endregion

    #region JACK STUMBLE
    void _ColidedWIthJack(int _jackno)
    {
        inteligencecheck = true;
        RPC_ActivateJack(_jackno, MyPathNumber);
    }

    IEnumerator _WaitAndStumble()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        luckchance = 6f;
    }

    #endregion

    #region RANK FINDING
    public void _ChangingRanke(int _Myrank)
    {
        //Debug.Log("My Ranke is changed " + MyName);
        _Myrank += 1;
        MyRankNo = _Myrank;
    }
    #endregion

    #region WIN LOGC 
    private IEnumerator SetMyWinPosition()
    {
        Debug.Log("SetMyWinPosition " + MyName);

        yield return new WaitForSecondsRealtime(0.2f);

        //GenratedPet.gameObject.SetActive(false);
        //_GenratePetPrefabOnWin();
        //WinPosition = pos;
        yield return new WaitForEndOfFrame();
        Debug.Log("Position Set now Just Play win animation over here " + gameObject.name);
        _ChangeAnimationHere(_AnimState.Jump);
    }

    private void _OnRankNumberChanged()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            //Debug.Log(MyName + "   " + MyRankNo);
            NetworkEventManager._EventUpdateMyNo(MyRankNo);
        }
    }

    private void _OnWInNumberAlocated()
    {
        if (!RaceManagerRef.RaceStart) return;

        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("Yes Win number is allowcated  " + MyWiningNumber + "      " + MyName);
            NetworkEventManager._EventWon(MyWiningNumber);
            NetwrokUI.Instance.WinUI.SetActive(true);
            NetworkCamera.Instance._ActiveWinScene();
            _ChangeAnimationHere(_AnimState.Jump);
            RaceManagerRef.MyWinNumber = MyWiningNumber;
            NetworkEventManager._EventDisableNameTags();
        }
        else
        {
            Debug.Log("Chaning animation for all other " + MyName);
            _ChangeAnimationHere(_AnimState.Jump);
        }
        NetwrokUI.Instance._SetMyNameOnPodium(MyWiningNumber, MyName);
        RaceManagerRef._CheckAllPlayerCompleted();
    }
    #endregion

    #region ANIMATION CAMERA AND EFFECTS
    private void _ChangeAnimationHere(_AnimState _state)
    {
        GenratedPet._ChangeAnimationState(_state);
        _ChangeParticleEffects(_state);
    }

    private void _ChangeParticleEffects(_AnimState _state)
    {
        switch (_state)
        {
            case _AnimState.Run:
                FootDustEffect_1.gameObject.SetActive(true);
                FootDustEffect_2.gameObject.SetActive(true);
                FlyingEffect.gameObject.SetActive(false);
                SwimmingEffect.gameObject.SetActive(false);
                Stunned.gameObject.SetActive(false);
                break;
            case _AnimState.Flying:
                FlyingEffect.gameObject.SetActive(true);
                SwimmingEffect.gameObject.SetActive(false);
                FootDustEffect_1.gameObject.SetActive(false);
                FootDustEffect_2.gameObject.SetActive(false);
                Stunned.gameObject.SetActive(false);
                break;
            case _AnimState.Swimming:
                SwimmingEffect.gameObject.SetActive(true);
                FlyingEffect.gameObject.SetActive(false);
                FootDustEffect_1.gameObject.SetActive(false);
                FootDustEffect_2.gameObject.SetActive(false);
                Stunned.gameObject.SetActive(false);
                break;
            case _AnimState.Stumble:
                Stunned.gameObject.SetActive(true);
                SwimmingEffect.gameObject.SetActive(false);
                FlyingEffect.gameObject.SetActive(false);
                FootDustEffect_1.gameObject.SetActive(false);
                FootDustEffect_2.gameObject.SetActive(false);
                break;
        }
    }

    void _SetupCamera()
    {
        NetworkCamera.Instance._SetUpCamera(transform, MyPathNumber);

        //DO START ANIMATION
        NetworkEventManager._EventCameraChange(_CamState.Start);
    }
    #endregion

    #region RUN SETUP
    public void _SetUpMyInitialData(int pr)
    {
        MyPathNumber = pr;
    }

    /// <summary>
    /// THIS METHOD WILL BE CALLED WHEN RACE STARTS
    /// </summary>
    public void _StartRun()
    {
        //RESET VLAUES
        CurruntIndex = 0;
        RaceComplete = false;
        GetComponent<Collider>().enabled = true;
        luckchance = 0f;
        NameTextObject.transform.localScale = Vector3.one;
        if (IsServer)
        {
            //Debug.Log(MyPathNumber);
            m_agent.enabled = true;
            _InitilizePath();
            CurruntPos = _GetNextPos(move_positions[CurruntIndex]);
            _SetDestination();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
        }
        else
        {
            m_agent.enabled = false;
        }
        _ChangeAnimationHere(_AnimState.Run);
    }

    public void _SetPathBasedOnIndex()
    {
        MyPathNumber = playerRef.PlayerId;
    }

    #endregion

    #region FIXED UPDATED
    /// <summary>
    /// NETWORKED CALLS
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (!m_input_value.m_enable_navmesh)
        {
            return;
        }

        if (Runner.TryGetInputForPlayer<NetworkInputData>(Object.InputAuthority, out var input)) // // this out keyword will find PlayerData script and assign the value all the information from that script and put it into input variable.
        {
            NetworkedSetTap = input.TapMultiplier;
            MyNetworkSpeed = input.Speed;
        }

        //MOVE PLAYER TO PODIUM
        if (RaceComplete)
        {
            networkTransform.Teleport(pos, Q);
            return;
        }

        //RETUNS IF CLIENT AND RACE IS COMPLETE
        if (!IsServer || RaceComplete)
        {
            return;
        }
        ////CALCULATION FOR LUCK
        ///
        if (RaceManagerRef.RaceStart)
        {
            if (terrain == _Tags.Land)
            {
                if (!CheckStumbleNow)
                {
                    luckchance += Runner.DeltaTime;
                }

                if (luckchance >= 5f)
                {
                    if (inteligencecheck && !CheckStumbleNow)
                    {
                        //Debug.Log("inteligencecheck");
                        MyNetworkSpeed = 0f;
                        Stumbled = true;
                        CheckStumbleNow = true;
                        RPC_LuckHanned();
                    }
                    else if (!Stumbled && !CheckStumbleNow)
                    {
                        RandomNumber = Random.Range(0f, 1f);
                        //Debug.Log(RandomNumber + " luck max " + LuckMax);
                        if (RandomNumber <= LuckMax)
                        {
                            MyNetworkSpeed = 0f;
                            Stumbled = true;
                            CheckStumbleNow = true;
                            RPC_LuckHanned();
                        }
                        else
                        {
                            //Debug.Log("luckchance Zero");
                            luckchance = 0f;
                        }
                    }
                    else if (CheckStumbleNow)
                    {
                        //Debug.Log("SpeedZero");
                        MyNetworkSpeed = 0f;
                    }
                    else
                    {
                        //Debug.Log("luckchance Zero");
                        luckchance = 0f;
                    }
                }
            }
        }
        //Changing animation speed
        if (GenratedPet != null)
        {
            GenratedPet._ChangeSpeed(m_agent.speed);
        }
        //Debug.Log("Calculating");
        //CALCULATING FOR SPEED FROM TAPING
        m_agent.speed = MyNetworkSpeed * NetworkedSetTap * SpeedController;

        //FIND DISTNACE HERE
        MyDistanceOnPath = RaceManagerRef._FindMyDistance(transform.position);
        RaceManagerRef.RankBasedPlayers[MyPathNumber].MyDistance = MyDistanceOnPath;
        _CalculateDistance();

        MyPos = transform.position;
        if (Distance < 4f)
        {
            _ChangeCurruntPoint();
        }
    }



    #endregion

    #region GETPLAYER INPUT
    public NetworkInputData GetPlayerNetworkInput() // playerdataları işlediğimiz yer. Buraya değerleri gönderiyuz FUN da alıyoruz.
    {
        NetworkInputData data = new NetworkInputData();
        data.TapMultiplier = PetConfigs.TapMultiplier;
        data.Speed = MySpeed;
        data.direction = WinPosition;
        return data; // then we will return the data.
    }
    #endregion

    #region NAVMESH METHODS
    private void _ChangeCurruntPoint()
    {
        CurruntIndex++;
        if (CurruntIndex >= move_positions.Count)
        {
            //Debug.Log("Path Complete");
            return;
        }

        //Calculate all path here
        CurruntPos = _GetNextPos(move_positions[CurruntIndex]);
        _SetDestination();
    }
    /// <summary>
    /// Initilize path
    /// </summary>
    void _InitilizePath()
    {
        //Debug.Log("My Path No  " + MyPathNumber + "   " + gameObject.name);
        move_positions = new List<Vector3>();
        move_positions = path_point.prePositions[MyPathNumber].m_positions;
    }

    public void _SetDestination()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(m_agent.transform.position, CurruntPos, NavMesh.AllAreas, path);
        m_agent.SetPath(path);
    }

    void _CalculateDistance()
    {
        Distance = Vector3.Distance(CurruntPos, transform.position);
    }

    public Vector3 _GetNextPos(Vector3 _pos)
    {
        hit = new NavMeshHit();
        finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(_pos, out hit, 10, 1))
        {
            finalPosition = hit.position;
        }
        else
        {
            finalPosition = _pos;
        }
        return finalPosition;
    }

    #endregion

    #region RPC CLLAS AND RECIVERS
    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetNameAndPrefab(NetworkString<_32> nickname, string _prefabid)
    {
        playerName = nickname;
        MyPrefabID = _prefabid;
        Debug.Log("My Prefab id is  " + _prefabid);
        _OnRecivedRPC();
    }

    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_GetMyConfigs(string _json)
    {
        //Debug.Log("I am sending RPC");
        _OnReciveConfigs(_json);
    }
    [Rpc(sources: RpcSources.All, RpcTargets.All)]
    public void RPC_ActivateJack(int JackNo, int Pathno)
    {
        RaceManagerRef._ActivateJack(JackNo, Pathno);
        luckchance = 6f;
        //StartCoroutine(_WaitAndStopPlayer());
    }

    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_JackInBox()
    {
        _OnRecivedStumble();
    }

    [Rpc(sources: RpcSources.All, RpcTargets.All)]
    public void RPC_LuckHanned()
    {
        StartCoroutine(_WaitAndStopLuckChance());
    }

    [Rpc(sources: RpcSources.All, RpcTargets.All)]
    public void RPC_ChangeMyAnimation(int _animno)
    {
        switch (_animno)
        {
            case 0:
                _ChangeAnimationHere(_AnimState.Idle);
                break;
        }
    }

    IEnumerator _WaitAndStopLuckChance()
    {
        //Debug.Log("_WaitAndStopLuckChance");
        _ChangeAnimationHere(_AnimState.Stumble);
        yield return new WaitForSecondsRealtime(2);
        Stumbled = false;
        inteligencecheck = false;
        luckchance = 0f;
        CheckStumbleNow = false;
        switch (terrain)
        {
            case _Tags.Land:
                _ChangeAnimationHere(_AnimState.Run);
                break;
            case _Tags.Water:
                _ChangeAnimationHere(_AnimState.Swimming);
                break;
            case _Tags.Flying:
                _ChangeAnimationHere(_AnimState.Flying);
                break;
            case _Tags.Climbing:
                _ChangeAnimationHere(_AnimState.Climbing);
                break;

        }
    }


    void _OnRecivedStumble()
    {
        Debug.Log("I recived rpc    " + MyName);
        luckchance = 5f;
    }

    private void _OnReciveConfigs(string _j)
    {
        Debug.Log("I recived RPC  " + gameObject.name + "   " + _j);
        playerconfigs = JsonUtility.FromJson<_PlayerConfigs>(_j);
        _SetMyConfigs();
    }

    void _OnRecivedRPC()
    {
        Debug.Log("Recived RPC HERE  " + Runner.IsServer + "  MY ACTUAL NAME IS   " + MyName);
        MyName = playerName.ToString();
        _SetName(MyName);
        gameObject.name = MyName;
        _GenratePetPrefab();
    }

    private void _GenratePetPrefab()
    {
        //Debug.Log("This works on Server  only  " + Runner.IsServer);
        if (GenratedPet == null)
        {
            //Debug.Log("   MyName  " + MyName + "  MyPrefabID  " + MyPrefabID);
            GameObject obj = Instantiate(RaceManager.instance.PetPrefabHolder._GetMyPrefab(MyPrefabID), transform);
            GenratedPet = obj.GetComponent<PetAnimation>();
            gameObject.GetComponent<NetworkMecanimAnimator>().Animator = GenratedPet.AnimatorRef;
            //gameObject.GetComponent<NetworkMecanimAnimator>().enabled = true;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _SetName(MyName);
            gameObject.name = MyName;

            //ADD PLAYER
            _GenratedAIPlayer G = new _GenratedAIPlayer();
            G.player = this;
            RaceManager.instance.TotalPlayers.Add(G);
            RaceManager.instance.TotalNumberOfPlayers++;
            RaceManager.instance.TotalRealPlayers++;

        }
        else
        {
            Debug.Log("ALready Genrated");
        }
    }

    private void _GenratePetPrefabOnWin()
    {
        int temp = MyWiningNumber - 1;
        Vector3 pos = RaceManager.instance.WinPoints[temp].position;
        GameObject obj = Instantiate(RaceManager.instance.PetPrefabHolder._GetMyPrefab(MyPrefabID), RaceManager.instance.WinPoints[temp]);
        PetAnimation P = obj.GetComponent<PetAnimation>();
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        P._ChangeAnimationState(_AnimState.Jump);
    }

    public void BeforeUpdate()
    {
        //if (Utils.IsLocalPlayer(Object))
        //{
        //    //PetConfigs.TapMultiplier
        //}
    }

    #endregion
}

[System.Serializable]
public class _PlayerConfigs
{
    //Athletics Stats
    public int running, climbing, flying, swimming, intelligence, luck, rank, maxStamina, mycoins;
    public float xp;
}
