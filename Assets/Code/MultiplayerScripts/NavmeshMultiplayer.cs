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
    public override void Spawned() // fusionun startı
    {
        path_point = FindObjectOfType<PathPointManager>();
        SetLocalObjects();
        _SetupConfigs();
        if (Runner.IsServer)
        {
            IsServer = true;
        }
        else
        {
            m_agent.enabled = false;
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
        m_agent.speed = Random.Range(3, PetConfigs.Speed);
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

            }
        }
        //else
        //{
        //    switch (other.tag)
        //    {
        //        case _Tags.Water:
        //            _ChangeAnimationHere(_AnimState.Swimming);
        //            break;
        //        case _Tags.Flying:
        //            _ChangeAnimationHere(_AnimState.Flying);
        //            break;
        //        case _Tags.Land:
        //            _ChangeAnimationHere(_AnimState.Run);
        //            break;
        //    }
        //}
    }
    #endregion

    #region WIN LOGC 

    private IEnumerator SetMyWinPosition()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("SetMyWinPosition " + MyName);
            yield return new WaitForSecondsRealtime(0.5f);
            int temp = MyWiningNumber - 1;
            Vector3 pos = RaceManager.instance.WinPoints[temp].position;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            yield return new WaitForSecondsRealtime(0.5f);
            Debug.Log("Position Set now Just Play win animation over here " + gameObject.name);
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
            transform.position = pos;
            _ChangeAnimationHere(_AnimState.Jump);
            NetworkCamera.Instance._ActiveWinScene();
        }
        else
        {
            Debug.Log("SetMyWinPosition Reset of the clients");
            yield return new WaitForSecondsRealtime(0.5f);
            int temp = MyWiningNumber - 1;
            Vector3 pos = RaceManager.instance.WinPoints[temp].position;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            yield return new WaitForSecondsRealtime(0.5f);
            transform.eulerAngles = new Vector3(0f, 90f, 0f);
            transform.position = pos;
            Debug.Log("Position Set now Just Play win animation over here " + gameObject.name + temp);
            _ChangeAnimationHere(_AnimState.Jump);
        }


    }

    private void _OnWInNumberAlocated()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            Debug.Log("Yes Win number is allowcated  " + MyWiningNumber + "      " + MyName);
            NetworkEventManager._EventWon(MyWiningNumber);
            //CHECK HERE FOR CLIENT AND THEN SHOW CAMERA ANIMATION
            if (Runner.IsClient)
            {
                Debug.Log("I am client so i need to change camera here");
                NetwrokUI.Instance.WinUI.SetActive(true);
                NetworkCamera.Instance._ActiveWinScene();
                _ChangeAnimationHere(_AnimState.Jump);
            }
        }
        else
        {
            Debug.Log("Chaning animation for all other " + MyName);
            _ChangeAnimationHere(_AnimState.Jump);
        }

    }

    #endregion

    #region ANIMATION CAMERA
    private void _ChangeAnimationHere(_AnimState _state)
    {
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
            _SetDestination(_GetNextPos(move_positions[m_currunt_index]));
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

    /// <summary>
    /// NETWORKED CALLS
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (!m_input_value.m_enable_navmesh)
        {
            return;
        }

        if (!IsServer || RaceComplete)
        {
            return;
        }

        //Debug.Log("Calculating");

        //FIND DISTNACE HERE
        _CalculateDistance();
        if (m_distance < 0.1)
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
        _SetDestination(_GetNextPos(m_currunt_pos));
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
            gameObject.GetComponent<NetworkMecanimAnimator>().Animator = GenratedPet.Animator;
            gameObject.GetComponent<NetworkMecanimAnimator>().enabled = true;
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
