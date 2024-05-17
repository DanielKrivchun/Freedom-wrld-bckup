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

    public NavMeshAgent m_agent;

    private void Start()
    {
        PathPoint = FindObjectOfType<PathPointManager>();
    }

    public void _SetPathBasedOnIndex(int _index)
    {
        this.gameObject.name = _index.ToString();
        myplayerNo = _index;
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
}
