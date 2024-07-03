using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Collections.Unicode;
using UnityEngine.AI;
using DG.Tweening.Core.Easing;
using static Beamable.Common.Constants.Features;

public class NetworkAIPlayer : NetworkBehaviour
{
    #region PUBLIC VARIABLES
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    public AiPlayerConfigs PetConfigs;
    [Space]
    public float Speed;
    [Space]
    public TextMeshPro nameText;
    [Header("Navmesh Agent")]
    public NavMeshAgent m_agent;
    [Header("ANIMATOR")]
    public PetAnimation GenratedPet;

    [Header("Script Refrence")]
    private PathPointManager path_point;

    [Space]
    public ParticleSystem FootDustEffect;
    public ParticleSystem SwimmingEffect;
    public ParticleSystem FlyingEffect;
    public ParticleSystem Stunned;

    public NetworkString<_32> playerName;
    public PlayerRef playerRef;
    public List<Vector3> move_positions;

    private float RandomNumber;
    private float RndomLuckChanceTime;
    #endregion

    #region Private variables
    private Vector3 m_currunt_pos;
    private float m_distance;
    private int m_currunt_index;
    public bool IsServer;
    public bool IsLocalPlayer;
    public bool RaceComplete;

    private bool Inteligencecheck = false;

    public NetworkTransform networkTransform;
    private NavMeshHit hit;
    private Vector3 finalPosition;
    #endregion

    #region NETWORKED OBJECTS
    [Networked] public string MyName { get; set; }
    [Networked] public int MyPathNumber { get; set; }
    [Networked] public string MyPrefabID { get; set; }

    [Networked, OnChangedRender(nameof(_OnWInNumberAlocated))]
    public int MyWiningNumber { get; set; }
    public int SpeedController = 1;
    private Vector3 pos;

    private bool Stumbled = false;

    public Quaternion Q { get; private set; }
    public float MyDistanceOnPath;
    private RaceManager RaceManagerRef;

    private float SpeedMul = 1f;

    private string terrain = _Tags.Land;

    private float luckchance = 0f;
    #endregion

    #region NETWORK FUCTIONS
    private void OnDestroy()
    {
        NetworkEventManager.e_player_speed_change -= _OnStopStartPlayer;
    }

    private void Start()
    {
        Debug.Log("WORKED");
        NetworkEventManager.e_player_speed_change += _OnStopStartPlayer;
        networkTransform = GetComponent<NetworkTransform>();
        path_point = FindObjectOfType<PathPointManager>();
        RaceManagerRef = RaceManager.instance;
        SetLocalObjects();
        if (Runner.IsServer)
        {
            IsServer = true;
        }
        RndomLuckChanceTime = Random.Range(5, 10);
        Speed = PetConfigs._GetMySpeed();
        m_agent.speed = Speed;
        StartCoroutine(_GenrateMyPrefab());
    }

    private void _OnStopStartPlayer(int _no)
    {
        SpeedController = _no;
        m_agent.speed = Speed * SpeedController;

        if (_no == 0)
        {
            GenratedPet._ChangeAnimationState(_AnimState.Idle);
        }
        else
        {
            GenratedPet._ChangeAnimationState(_AnimState.Run);
        }

    }

    IEnumerator _GenrateMyPrefab()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("This Choroutine Worked " + gameObject.name);
        _GenratePetPrefab();
    }


    private void SetLocalObjects()
    {
        Debug.Log("I am Local Player");
        IsLocalPlayer = true;
        MyName = RaceManager.instance.namesJson._GetNames();
        playerName = MyName;
        Debug.Log("Sending RPC with Name   " + playerName);
        int a = Random.Range(0, RaceManager.instance.PetPrefabHolder.PetPrefabs.Count);
        MyPrefabID = RaceManager.instance.PetPrefabHolder.PetPrefabs[a].PrefabId;
        //MyPrefabID = "1";
        _SetName(playerName.ToString());
        gameObject.name = MyName.ToString();
    }

    #endregion

    #region UNITY METHODS

    private void OnEnable()
    {
        NetworkEventManager.e_get_set_go += _StartRun;
    }

    private void OnDisable()
    {
        NetworkEventManager.e_get_set_go -= _StartRun;
    }
    #endregion

    #region COLISION DETECTION

    private void OnTriggerEnter(Collider other)
    {
        if (Runner.IsServer)
        {
            switch (other.tag)
            {
                case _Tags.WinLine:
                    Debug.Log("WINLINE COLIDED" + other.tag + "    " + MyName);
                    MyWiningNumber = RaceManager.instance._GetMyWinningNo();
                    RaceComplete = true;
                    GetComponent<NavMeshAgent>().enabled = false;
                    GetComponent<Collider>().enabled = false;
                    StartCoroutine(SetMyWinPosition());
                    break;
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
                case _Tags.Jack:
                    _ColidedWIthJack(1);
                    break;
                case _Tags.JackSecond:
                    _ColidedWIthJack(2);
                    break;
            }
        }
        else
        {
            switch (other.tag)
            {
                case _Tags.Water:
                    _ChangeAnimationHere(_AnimState.Swimming);
                    break;
                case _Tags.Flying:
                    _ChangeAnimationHere(_AnimState.Flying);
                    break;
                case _Tags.Land:
                    _ChangeAnimationHere(_AnimState.Run);
                    break;
                case _Tags.Climbing:
                    _ChangeAnimationHere(_AnimState.Climbing);
                    break;
            }
        }
    }

    void _ColidedWIthJack(int _jackno)
    {
        RandomNumber = Random.Range(0f, 1f);
        if (RandomNumber <= 0.3f)
        {
            Inteligencecheck = true;
            RPC_ActivateJack(_jackno, MyPathNumber);
        }
    }

    private void _OnWInNumberAlocated()
    {
        Debug.Log("Yes Win number is allowcated  " + MyWiningNumber + "      " + MyName);
        _ChangeAnimationHere(_AnimState.Jump);
        RaceManager.instance._CheckAllPlayerCompleted();
    }

    private IEnumerator SetMyWinPosition()
    {
        Debug.Log("SetMyWinPosition");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        yield return new WaitForSecondsRealtime(0.2f);
        int temp = MyWiningNumber - 1;
        pos = RaceManager.instance.WinPoints[temp].position;
        Q = RaceManager.instance.WinPoints[temp].rotation;
        networkTransform.Teleport(pos, Q);
        //GenratedPet.gameObject.SetActive(false);
        //_GenratePetPrefabOnWin();
        yield return new WaitForEndOfFrame();
        _ChangeAnimationHere(_AnimState.Jump);
    }

    #endregion

    #region ANIMATION CAMERA AND EFFECTS
    private void _ChangeAnimationHere(_AnimState _state)
    {
        //Debug.Log("Changed ANimation here");
        GenratedPet._ChangeAnimationState(_state);
        _ChangeParticleEffects(_state);
    }

    private void _ChangeParticleEffects(_AnimState _state)
    {
        switch (_state)
        {
            case _AnimState.Run:
                FootDustEffect.gameObject.SetActive(true);
                FlyingEffect.gameObject.SetActive(false);
                SwimmingEffect.gameObject.SetActive(false);
                Stunned.gameObject.SetActive(false);
                break;
            case _AnimState.Flying:
                FlyingEffect.gameObject.SetActive(true);
                SwimmingEffect.gameObject.SetActive(false);
                FootDustEffect.gameObject.SetActive(false);
                Stunned.gameObject.SetActive(false);
                break;
            case _AnimState.Swimming:
                SwimmingEffect.gameObject.SetActive(true);
                FlyingEffect.gameObject.SetActive(false);
                FootDustEffect.gameObject.SetActive(false);
                Stunned.gameObject.SetActive(false);
                break;
            case _AnimState.Stumble:
                Stunned.gameObject.SetActive(true);
                SwimmingEffect.gameObject.SetActive(false);
                FlyingEffect.gameObject.SetActive(false);
                FootDustEffect.gameObject.SetActive(false);
                break;
        }
    }

    void _SetupCamera()
    {
        NetworkCamera.Instance._SetUpCamera(transform);
    }
    #endregion

    #region RUN SETUP
    public void _SetUpMyInitialData(int pr)
    {
        MyPathNumber = pr;
    }

    public void _StartRun()
    {
        if (IsServer)
        {
            Debug.Log(MyPathNumber);
            _InitilizePath();
            m_currunt_pos = _GetNextPos(move_positions[m_currunt_index]);
            _SetDestination();
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

        if (RaceComplete)
        {
            networkTransform.Teleport(pos, Q);
            return;
        }

        if (!IsServer && RaceComplete)
        {
            return;
        }

        if (RaceManagerRef.RaceStart)
        {
            if (terrain == _Tags.Land)
            {
                luckchance += Time.deltaTime;
                if (luckchance >= RndomLuckChanceTime)
                {
                    RandomNumber = Random.Range(0f, 1f);

                    if (Inteligencecheck)
                    {
                        Debug.Log(" Inteligencecheck hapning " + luckchance);
                        SpeedMul = 0f;
                        m_agent.speed = Speed * SpeedMul;
                        Stumbled = true;
                        Inteligencecheck = false;
                        RPC_LuckHanned();
                    }
                    else if (!Stumbled)
                    {
                        if (RandomNumber <= 0.3f)
                        {
                            Debug.Log(" LuckChance hapning " + RandomNumber);
                            SpeedMul = 0f;
                            Stumbled = true;
                            m_agent.speed = Speed * SpeedMul;
                            RPC_LuckHanned();
                        }
                        else
                        {
                            luckchance = 0f;
                        }
                    }
                }
            }
        }

        MyDistanceOnPath = RaceManagerRef._FindMyDistance(transform.position);
        RaceManagerRef.RankBasedPlayers[MyPathNumber].MyDistance = MyDistanceOnPath;
        //FIND DISTNACE HERE
        _CalculateDistance();
        if (m_distance < 1)
        {
            _ChangeCurruntPoint();
        }
    }
    #endregion

    #region NAVMESH METHODS
    private void _ChangeCurruntPoint()
    {
        m_currunt_index++;
        if (m_currunt_index >= move_positions.Count)
        {
            //Debug.Log("Path Complete");
            return;
        }
        m_currunt_pos = _GetNextPos(move_positions[m_currunt_index]);
        _SetDestination();
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

    /// <summary>
    /// Initilize path
    /// </summary>
    void _InitilizePath()
    {
        Debug.Log("My Path No  " + MyPathNumber + "   " + gameObject.name);
        move_positions = new List<Vector3>();
        move_positions = path_point.prePositions[MyPathNumber].m_positions;
    }

    public void _SetDestination()
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(m_agent.transform.position, m_currunt_pos, NavMesh.AllAreas, path);
        m_agent.SetPath(path);
    }

    void _CalculateDistance()
    {
        m_distance = Vector3.Distance(m_currunt_pos, transform.position);
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

    [Rpc(sources: RpcSources.All, RpcTargets.All)]
    public void RPC_ActivateJack(int JackNo, int Pathno)
    {
        RaceManagerRef._ActivateJack(JackNo, Pathno);
        luckchance = 15f;
        //StartCoroutine(_WaitAndStopPlayer());
    }

    [Rpc(sources: RpcSources.All, RpcTargets.All)]
    public void RPC_LuckHanned()
    {
        StartCoroutine(_WaitAndStopLuckChance());
    }

    IEnumerator _WaitAndStopLuckChance()
    {
        Debug.Log("_WaitAndStopLuckChance");
        _ChangeAnimationHere(_AnimState.Stumble);
        yield return new WaitForSecondsRealtime(2);
        luckchance = 0f;
        Stumbled = false;
        Inteligencecheck = false;
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
        Speed = PetConfigs._GetMySpeed();
        m_agent.speed = Speed;
    }

    void _OnRecivedRPC()
    {
        Debug.Log("Recived RPC HERE  " + Runner.IsServer + "  MY ACTUAL NAME IS   " + MyName);
        MyName = playerName.ToString();
        _SetName(MyName);
        gameObject.name = MyName;
        _GenratePetPrefab();
    }

    void _SetName(string _name)
    {
        nameText.text = _name;
    }

    private void _GenratePetPrefab()
    {
        Debug.Log("This works on Server  only  " + Runner.IsServer);
        if (GenratedPet == null)
        {
            Debug.Log("   MyName  " + MyName + "  MyPrefabID  " + MyPrefabID);
            GameObject obj = Instantiate(RaceManager.instance.PetPrefabHolder._GetMyPrefab(MyPrefabID), transform);
            GenratedPet = obj.GetComponent<PetAnimation>();
            gameObject.GetComponent<NetworkMecanimAnimator>().Animator = GenratedPet.AnimatorRef;
            gameObject.GetComponent<NetworkMecanimAnimator>().enabled = true;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _SetName(MyName);
            gameObject.name = MyName;

            _GenratedAIPlayer G = new _GenratedAIPlayer();
            G.aiplayer = this;
            G.AI = true;
            RaceManager.instance.TotalPlayers.Add(G);
            RaceManager.instance.TotalNumberOfPlayers++;
        }
        else
        {
            Debug.Log("ALready Genrated");
        }
    }

    #endregion
}
