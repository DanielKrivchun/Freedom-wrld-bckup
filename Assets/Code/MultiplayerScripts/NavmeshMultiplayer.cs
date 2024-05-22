using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.AI;
using TMPro;
using System.Xml.Linq;
public class NavmeshMultiplayer : NetworkBehaviour
{
    #region PUBLIC VARIABLES
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    [Space]
    public TextMeshPro nameText;
    [Header("Navmesh Agent")]
    public NavMeshAgent m_agent;
    [Header("ANIMATOR")]

    public PetAnimation PetAnimation;

    [Header("Script Refrence")]
    private PathPointManager path_point;

    public NetworkString<_32> playerName;
    public PlayerRef playerRef;
    public List<Vector3> move_positions;
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

    #region Provate variables
    private Vector3 m_currunt_pos;
    private float m_distance;
    private int m_currunt_index;
    #endregion

    #region NETWORKED OBJECTS
    [Networked] public string MyName { get; set; }
    [Networked] public int MyPathNumber { get; set; }
    [Networked] public int MyWiningNumber { get; set; }

    #endregion

    #region NETWORK FUCTIONS
    public override void Spawned() // fusionun startı
    {
        path_point = FindObjectOfType<PathPointManager>();
        SetLocalObjects();
    }

    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            playerName = RaceManager.instance.LocalPlayerNickname;
            ColoredDebug.Log("Sending RPC with Name" + playerName, Color.green);
            MyName = playerName.ToString();
            RpcSetNickNameClients(playerName);
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

    #region COLISION DETECTION

    private void OnTriggerEnter(Collider other)
    {
        if (Runner.IsServer)
        {
            Debug.Log(other.tag + "    " + MyName);
            MyWiningNumber = RaceManager.instance._GetMyWinningNo();
        }
    }
    #endregion

    #region ANIMATION CAMERA
    private void _ChangeAnimationHere(_AnimState _state)
    {
        PetAnimation._ChangeAnimationState(_state);
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
        Debug.Log(MyPathNumber);
        _InitilizePath();
        _SetDestination(move_positions[m_currunt_index]);
        _ChangeAnimationHere(_AnimState.Running);
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
    private void RpcSetNickNameClients(NetworkString<_32> nickname)
    {
        playerName = nickname;
        _OnRecivedRPC();
    }

    void _OnRecivedRPC()
    {
        Debug.Log("Recived RPC HERE  " + Runner.IsServer);
        nameText.text = playerName.ToString();
        MyName = playerName.ToString();
    }

    #endregion
}
