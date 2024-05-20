using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.AI;
public class NavmeshMultiplayer : NetworkBehaviour
{
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    [Space]
    public int myplayerNo;

    public List<Vector3> m_positions;

    private int m_currunt_index;

    [Space]
    public PathPointManager PathPoint;

    private Vector3 m_currunt_pos;

    private float m_distance;
    public PlayerRef playerRef;
    public NavMeshAgent m_agent;
    public NetworkString<_8> playerName;


    public override void Spawned() // fusionun startı
    {
        Debug.Log("This Called");
        PathPoint = FindObjectOfType<PathPointManager>();
        SetLocalObjects();
    }

    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            //cam.transform.SetParent(null);
            //cam.SetActive(true);
            var nickName = playerName = RaceManager.instance.LocalPlayerNickname;
            Debug.Log("Checking name here  " + nickName);
            RpcSetNickName(nickName);

        }
        else
        {
            Debug.Log("This is else");
        }

    }


    public void _SetUpMyInitialData(PlayerRef pr)
    {
        playerRef = pr;
        Debug.Log("  " + playerRef.PlayerId);
    }

    public void _SetPathBasedOnIndex()
    {
        this.gameObject.name = playerRef.PlayerId.ToString();
        myplayerNo = playerRef.PlayerId;
        Debug.Log(myplayerNo);
        _InitilizePath();
        _SetDestination(m_positions[m_currunt_index]);
    }

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

    private void _ChangeCurruntPoint()
    {
        m_currunt_index++;
        if (m_currunt_index >= m_positions.Count)
        {
            Debug.Log("Path Complete");
            return;
        }
        m_currunt_pos = m_positions[m_currunt_index];
        _SetDestination(m_positions[m_currunt_index]);
    }
    /// <summary>
    /// Initilize path
    /// </summary>
    void _InitilizePath()
    {
        m_positions = new List<Vector3>();
        m_positions = PathPoint.prePositions[myplayerNo].m_positions;
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

    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickName(NetworkString<_8> nickname)
    {
        playerName = nickname;
        //Debug.Log(playerName);
    }
}
