using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Collections.Unicode;
using UnityEngine.AI;

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

    public NetworkString<_32> playerName;
    public PlayerRef playerRef;
    public List<Vector3> move_positions;
    #endregion

    #region Private variables
    private Vector3 m_currunt_pos;
    private float m_distance;
    private int m_currunt_index;
    public bool IsServer;
    public bool IsLocalPlayer;
    public bool RaceComplete;

    private NetworkTransform networkTransform;
    private NavMeshHit hit;
    private Vector3 finalPosition;
    #endregion

    #region NETWORKED OBJECTS
    [Networked] public string MyName { get; set; }
    [Networked] public int MyPathNumber { get; set; }
    [Networked] public string MyPrefabID { get; set; }

    [Networked, OnChangedRender(nameof(_OnWInNumberAlocated))]
    public int MyWiningNumber { get; set; }

    #endregion

    #region NETWORK FUCTIONS

    private void Start()
    {
        Debug.Log("WORKED");
        networkTransform = GetComponent<NetworkTransform>();
        path_point = FindObjectOfType<PathPointManager>();
        SetLocalObjects();
        _SetupConfigs();
        if (Runner.IsServer)
        {
            IsServer = true;
        }
        Speed = PetConfigs._GetMySpeed();
        StartCoroutine(_GenrateMyPrefab());
    }

    IEnumerator _GenrateMyPrefab()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("This Choroutine Worked " + gameObject.name);
        _GenratePetPrefab();
    }

    /// <summary>
    /// Remove random on Actual Project
    /// </summary>
    void _SetupConfigs()
    {
        m_agent.speed = Random.Range(3, 8);
        m_agent.acceleration = Random.Range(15, Speed);
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
        GenratedPet.gameObject.SetActive(false);
        _GenratePetPrefabOnWin();
        yield return new WaitForEndOfFrame();
        _ChangeAnimationHere(_AnimState.Jump);
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

    #endregion

    #region ANIMATION CAMERA
    private void _ChangeAnimationHere(_AnimState _state)
    {
        Debug.Log("Changed ANimation here");
        GenratedPet._ChangeAnimationState(_state);
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

    /// <summary>
    /// NETWORKED CALLS
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (!m_input_value.m_enable_navmesh)
        {
            return;
        }

        if (!IsServer && RaceComplete)
        {
            return;
        }
        //FIND DISTNACE HERE
        _CalculateDistance();
        if (m_distance < 1)
        {
            _ChangeCurruntPoint();
        }
    }

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
            gameObject.GetComponent<NetworkMecanimAnimator>().Animator = GenratedPet.Animator;
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
