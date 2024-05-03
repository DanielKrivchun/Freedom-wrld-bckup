using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";

    InputControl input;

    NavMeshAgent agent;
    Animator animator;

    //[Header("Movement")]
    //[SerializeField] ParticleSystem clickEffect;
    //[SerializeField] LayerMask clickableLayers;
    [Header("SCRIPTABLE OBJECTS")]
    public InputValue m_input_value;
    public PetConfigs PetConfig;
    [Space]
    public bool StartClicker;
    #region PUBLIC
    #endregion
    #region PRIVATE

    float lookRotationSpeed = 8f;

    public float CPR;
    private int TotalClick = 0;
    private float TimeInSecond = 0f;

    private Vector3 Directions;
    private Vector2 Input;

    private CharacterController ChController;

    private Camera MainCam;

    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        animator = GetComponent<Animator>();
        ChController = GetComponent<CharacterController>();
        MainCam = Camera.main;
        //input = new InputControl();
        //AssignInputs();
    }


    private void Update()
    {
        if (StartClicker)
        {
            TimeInSecond += Time.deltaTime;
        }

        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            Debug.Log("Worked");
            CPR = (TotalClick) / (TimeInSecond);
            Debug.Log(CPR);
        }

        _PlayerRotation();
        _ApplyMovement();
    }

    #endregion

    #region MOVEMENT

    private void _PlayerRotation()
    {
        if (Input.sqrMagnitude == 0) return;
        Directions = Quaternion.Euler(0.0f, MainCam.transform.eulerAngles.y, 0.0f) * new Vector3(Input.x, 0.0f, Input.y);
        var targetRotation = Quaternion.LookRotation(Directions, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, PetConfig.RotationSpeed * Time.deltaTime);
    }

    private void _ApplyMovement()
    {
        PetConfig.Speed = Mathf.MoveTowards(PetConfig.Speed, PetConfig.Incrimental, PetConfig.Acceleration * Time.deltaTime);
        ChController.Move(Directions * PetConfig.Speed * Time.deltaTime);
        _Move();
    }

    public void _Move()
    {
        Input = m_input_value.Input;
        Directions = new Vector3(Input.x, 0.0f, Input.y);
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
