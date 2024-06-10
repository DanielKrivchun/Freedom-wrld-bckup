using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;

public class NavMeshController : MonoBehaviour
{
    #region PUBLIC
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    [Space]
    public GameObject m_obj;
    [Space]
    public int CUrrntPathNo;
    [Space]
    public PathPointManager m_path_point;
    #endregion

    #region PRIVATE
    private NavMeshAgent m_agent;
    RaycastHit m_hit_info = new RaycastHit();

    private Vector2 m_smooth_delta_pos = Vector2.zero;
    private Vector2 m_velocity = Vector2.zero;

    public List<Vector3> m_positions;
    private float m_distance;
    public PetAnimation GenratedPet;
    #endregion


    private Camera m_camera;
    public int m_currunt_index;
    private Vector3 m_currunt_pos;

    #region UNITY METHODS

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        m_agent = GetComponent<NavMeshAgent>();


        //m_currunt_index = 0;
        if (m_input_value.m_enable_navmesh)
        {
            _InitilizePath();
            transform.position = m_positions[m_currunt_index];
            m_obj.transform.position = m_positions[m_currunt_index];
            _SetDestination(m_positions[m_currunt_index]);
            _ChangeAnimationHere(_AnimState.Run);
        }
    }

    private void _ChangeAnimationHere(_AnimState _state)
    {
        Debug.Log("Changed ANimation here");
        GenratedPet._ChangeAnimationState(_state);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case _Tags.WinLine:
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<Collider>().enabled = false;

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

            case _Tags.Land:
                _ChangeAnimationHere(_AnimState.Run);
                break;

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

        if (m_distance < 0.3f)
        {
            _ChangeCurruntPoint();
        }
        //Debug.Log(m_distance);

    }
    #endregion

    private void _ChangeCurruntPoint()
    {
        m_currunt_index++;
        if (m_currunt_index >= m_positions.Count)
        {
            Debug.Log("Path Complete");
            return;
        }


        m_currunt_pos = m_positions[m_currunt_index];
        m_obj.transform.position = m_currunt_pos;

        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(m_currunt_pos, out myNavHit, 100, -1))
        {
            _SetDestination(myNavHit.position);
        }
        else
        {
            _SetDestination(m_currunt_pos);
        }

        //Debug.Log(m_currunt_pos);
        //_SetDestination(m_positions[m_currunt_index]);
    }

    /// <summary>
    /// Initilize path
    /// </summary>
    void _InitilizePath()
    {
        m_positions = new List<Vector3>();
        m_positions = m_path_point.prePositions[CUrrntPathNo].m_positions;
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
