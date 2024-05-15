using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    public PetConfigs m_pet_config;
    [Space]
    public PetAnimation m_pet_anim_controller;


    private Vector3 m_directions;
    private Vector2 m_input;

    private NetworkCharacterController m_ch_controller;

    private Camera m_main_Cam;

    // Start is called before the first frame update
    void Start()
    {
        m_main_Cam = Camera.main;
        m_ch_controller=GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            m_directions = data.direction;
            _PlayerRotation();
            _ApplyMovement();
        }


    }



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
        m_ch_controller.Move(m_directions * m_pet_config.m_speed * Runner.DeltaTime);
        _Move();
    }

    public void _Move()
    {

        m_directions = new Vector3(m_input.x, 0.0f, m_input.y);
    }
}
