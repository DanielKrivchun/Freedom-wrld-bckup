using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    #region PUBLIC
    public float m_base_speed;
    [Space]
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    public PetConfigs m_pet_config;
    [Space]
    public PetAnimation m_pet_anim_controller;
    [Space]
    public bool StartClicker;
    #endregion

    #region PRIVATE


    public float CPR;
    private int m_total_click = 0;
    private float m_time_in_second = 0f;

    private Vector3 m_directions;
    private Vector2 m_input;

    private CharacterController m_ch_controller;

    private Camera m_main_Cam;
  


    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        m_ch_controller = GetComponent<CharacterController>();
        m_main_Cam = Camera.main;
    }

    private void Update()
    {
        //if (StartClicker)
        //{
        //    m_time_in_second += Time.deltaTime;
        //}

        //if (UnityEngine.Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("Worked");
        //    CPR = (m_total_click) / (m_time_in_second);
        //    Debug.Log(CPR);
        //}

        if (m_input_value.m_enable_input)
        {

            _PlayerRotation();
            _ApplyMovement();
        }
    }

    #endregion

    #region MOVEMENT FOR EDITOR WSAD
    private void _PlayerRotation()
    {
        if (m_input.sqrMagnitude == 0)
        {
            m_pet_anim_controller._ChangeAnimationState(_AnimState.Idle);
            return;
        }

        m_directions = Quaternion.Euler(0.0f, m_main_Cam.transform.eulerAngles.y, 0.0f) * new Vector3(m_input.x, 0.0f, m_input.y);
        var targetRotation = Quaternion.LookRotation(m_directions, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_pet_config.m_rotationspeed * Time.deltaTime);
        m_pet_anim_controller._ChangeAnimationState(_AnimState.Run);
    }

    private void _ApplyMovement()
    {
        m_pet_config.m_speed = Mathf.MoveTowards(m_pet_config.m_speed, m_pet_config.m_incrimental, m_pet_config.m_acceleration * Time.deltaTime);
        m_ch_controller.Move(m_directions * m_pet_config.m_speed * Time.deltaTime); 
        _Move();
    }

    public void _Move()
    {
        m_input = m_input_value.m_input;
        m_directions = new Vector3(m_input.x, 0.0f, m_input.y);
    }

    #endregion

    //CHANGE SPEED ACORDING TO LLEVELS
    #region SPEED CHANGE
    public void _OnSteminaChange()
    {

    }

    public void _OnLowStemina()
    {

    }

    public void _SwitchToOutOfStemina()
    {

    }


    #endregion

    //void AssignInputs()
    //{
    //    input.Main.Move.performed += ctx => ClickToMove();
    //}

    //void ClickToMove()
    //{
    //    Debug.Log("ClickToMove");
    //    RaycastHit hit;
    //    if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100, clickableLayers))
    //    {
    //        agent.destination = hit.point;
    //        if (clickEffect != null)
    //        {
    //            Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
    //        }
    //    }
    //}

    //void OnEnable()
    //{
    //    input.Enable();
    //}

    //void OnDisable()
    //{
    //    input.Disable();
    //}
}
