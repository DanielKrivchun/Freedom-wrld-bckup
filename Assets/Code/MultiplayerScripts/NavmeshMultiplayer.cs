using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.AI;
using TMPro;
using System.Xml.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;
public class NavmeshMultiplayer : NetworkBehaviour
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
    #endregion

    #region NETWORKED OBJECTS
    [Networked] public string MyName { get; set; }
    [Networked] public int MyPathNumber { get; set; }
    [Networked] public string MyPrefabID { get; set; }

    [Networked, OnChangedRender(nameof(_OnWInNumberAlocated))]
    public int MyWiningNumber { get; set; }

    #endregion

    #region NETWORK FUCTIONS
    public override void Spawned() // fusionun startı
    {
        path_point = FindObjectOfType<PathPointManager>();
        SetLocalObjects();
        _SetupConfigs();
        if (Runner.IsServer)
        {
            IsServer = true;
        }

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
        m_agent.speed = Random.Range(5, PetConfigs.Speed);
        m_agent.acceleration = Random.Range(15, PetConfigs.Acceleration);
    }

    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            IsLocalPlayer = true;
            playerName = RaceManager.instance.LocalPlayerNickname;
            Debug.Log("Sending RPC with Name   " + playerName);
            MyName = playerName.ToString();
            RPC_SetNameAndPrefab(playerName, RaceManager.instance.PrefabID);
            nameText.text = playerName.ToString();
            gameObject.name = MyName.ToString();
            _SetupCamera();
        }
        else
        {
            Debug.Log("My Name Is " + MyName);
            gameObject.name = MyName.ToString();
            nameText.text = MyName.ToString();
        }
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
            Debug.Log(other.tag + "    " + MyName);
            MyWiningNumber = RaceManager.instance._GetMyWinningNo();
            RaceComplete = true;
            m_agent.SetDestination(transform.position);
            GetComponent<NavMeshAgent>().enabled = false;
            StartCoroutine(SetMyWinPosition());
        }
    }


    private IEnumerator SetMyWinPosition()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        int temp = MyWiningNumber - 1;
        Vector3 pos = RaceManager.instance.WinPoints[temp].position;

        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("Position Set now Just Play win animation over here " + gameObject.name);
        transform.eulerAngles = new Vector3(0f, 90f, 0f);
        transform.position = pos;
        NetwrokUI.Instance.WinUI.SetActive(false);
        NetworkCamera.Instance._ActiveWinScene();
    }

    private void _OnWInNumberAlocated()
    {
        Debug.Log("_OnWInNumberAlocated " + gameObject.name);
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("Yes Win number is allowcated  " + MyWiningNumber + "      " + MyName);
            NetworkEventManager._EventWon(MyWiningNumber);
        }

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
            _SetDestination(move_positions[m_currunt_index]);
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
        m_currunt_pos = move_positions[m_currunt_index];
        _SetDestination(move_positions[m_currunt_index]);
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

    public void _SetDestination(Vector3 _target_pos)
    {
        m_currunt_pos = _target_pos;
        m_agent.SetDestination(m_currunt_pos);
    }

    void _CalculateDistance()
    {
        m_distance = Vector3.Distance(m_currunt_pos, transform.position);
        //Debug.Log(m_distance);
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
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


    void _OnRecivedRPC()
    {
        Debug.Log("Recived RPC HERE  " + Runner.IsServer + "  MY ACTUAL NAME IS   " + MyName);
        MyName = playerName.ToString();
        nameText.text = MyName.ToString();
        gameObject.name = MyName;
        _GenratePetPrefab();
    }

    private void _GenratePetPrefab()
    {
        Debug.Log("This works on Server  only  " + Runner.IsServer);
        if (GenratedPet == null)
        {
            Debug.Log("   MyName  " + MyName + "  MyPrefabID  " + MyPrefabID);
            GameObject obj = Instantiate(RaceManager.instance.PetPrefabHolder._GetMyPrefab(MyPrefabID), transform);
            GenratedPet = obj.GetComponent<PetAnimation>();
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            nameText.text = MyName.ToString();
            gameObject.name = MyName;
        }
        else
        {
            Debug.Log("ALready Genrated");
        }
    }

    #endregion
}
