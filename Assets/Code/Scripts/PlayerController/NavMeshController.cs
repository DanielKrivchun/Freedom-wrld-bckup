using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    #region PUBLIC
    public List<Transform> m_target_pos;
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    #endregion

    #region PRIVATE
    private NavMeshAgent m_agent;
    RaycastHit m_hit_info = new RaycastHit();

    private Vector2 m_smooth_delta_pos = Vector2.zero;
    private Vector2 m_velocity = Vector2.zero;

    private List<Vector3> m_positions;
    private float m_distance;

    #endregion


    private Camera m_camera;
    private int m_currunt_index;
    private Vector3 m_currunt_pos;

    #region UNITY METHODS

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        m_agent = GetComponent<NavMeshAgent>();

        if (m_input_value.m_enable_navmesh)
        {
            _InitilizePath();
            _SetDestination(m_positions[m_currunt_index]);
        }
    }


    private void Update()
    {
        if (!m_input_value.m_enable_navmesh)
        {
            return;
        }

        //FIND DISTNACE HERE
        _CalculateDistance();

        if (m_distance < 1)
        {
            m_currunt_index++;
            _ChangeCurruntPoint();
        }
        Debug.Log(m_distance);

    }
    #endregion

    private void _ChangeCurruntPoint()
    {
        m_currunt_pos = m_positions[m_currunt_index];

    }

    /// <summary>
    /// Initilize path
    /// </summary>
    void _InitilizePath()
    {
        m_positions = new List<Vector3>();

        foreach (Transform t in m_target_pos)
        {
            m_positions.Add(t.position);
        }
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
}
