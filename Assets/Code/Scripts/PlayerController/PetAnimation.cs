using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
    public Animator AnimatorRef;
    [Space]
    private float curruntspeed;
    public float Speed;

    public _AnimState m_currunt_anim_state;

    /// <summary>
    /// CHANGE ANIMATIONS HERE
    /// SEND PARAMATER TO CHANGE
    /// </summary>
    public void _ChangeAnimationState(_AnimState _state)
    {
        switch (_state)
        {
            case _AnimState.Idle:
                AnimatorRef.SetFloat(_Strings.Velocity, 0);
                AnimatorRef.SetBool(_Strings.IsSleeping, false);
                AnimatorRef.SetBool(_Strings.IsSick, false);
                AnimatorRef.SetBool(_Strings.Idle, true);
                AnimatorRef.SetBool(_Strings.Jumping, false);
                m_currunt_anim_state = _AnimState.Idle;
                break;

            case _AnimState.Wallk:
                AnimatorRef.SetFloat(_Strings.Velocity, 0.5f);
                m_currunt_anim_state = _AnimState.Wallk;
                break;

            case _AnimState.Run:
                AnimatorRef.SetBool(_Strings.Idle, false);
                AnimatorRef.SetFloat(_Strings.Velocity, 1f);
                AnimatorRef.SetBool(_Strings.StumbleBool, false);
                m_currunt_anim_state = _AnimState.Run;
                break;

            case _AnimState.Jump:
                AnimatorRef.SetFloat(_Strings.Velocity, 0f);
                AnimatorRef.SetBool(_Strings.Jumping, true);
                m_currunt_anim_state = _AnimState.Jump;
                break;

            case _AnimState.Flying:
                AnimatorRef.SetFloat(_Strings.Velocity, 2f);
                AnimatorRef.SetBool(_Strings.StumbleBool, false);
                m_currunt_anim_state = _AnimState.Flying;
                break;

            case _AnimState.Swimming:
                AnimatorRef.SetFloat(_Strings.Velocity, 1.5f);
                AnimatorRef.SetBool(_Strings.StumbleBool, false);
                m_currunt_anim_state = _AnimState.Swimming;
                break;

            case _AnimState.Climbing:
                AnimatorRef.SetFloat(_Strings.Velocity, 2.5f);
                AnimatorRef.SetBool(_Strings.StumbleBool, false);
                m_currunt_anim_state = _AnimState.Climbing;
                break;

            case _AnimState.Sick:
                AnimatorRef.SetBool(_Strings.IsSick, true);
                AnimatorRef.SetFloat(_Strings.Velocity, 0);
                AnimatorRef.SetBool(_Strings.Idle, false);
                m_currunt_anim_state = _AnimState.Sick;
                break;

            case _AnimState.Eating:
                AnimatorRef.SetTrigger(_Strings.Eat);
                m_currunt_anim_state = _AnimState.Eating;
                break;

            case _AnimState.Happy:
                AnimatorRef.SetTrigger(_Strings.Happy);
                m_currunt_anim_state = _AnimState.Happy;
                break;

            case _AnimState.Bath:
                AnimatorRef.SetTrigger(_Strings.Bath);
                m_currunt_anim_state = _AnimState.Bath;
                break;

            case _AnimState.Sleep:
                AnimatorRef.SetBool(_Strings.IsSleeping, true);
                m_currunt_anim_state = _AnimState.Sleep;
                break;
            case _AnimState.Stumble:
                AnimatorRef.SetFloat(_Strings.Velocity, 0f);
                AnimatorRef.SetBool(_Strings.StumbleBool, true);
                m_currunt_anim_state = _AnimState.Stumble;
                break;

            default:
                break;
        }
    }

    public void _ChangeSpeed(float _speed)
    {
        if (curruntspeed == _speed)
        {
            return;
        }

        curruntspeed = _speed;
        switch (m_currunt_anim_state)
        {
            case _AnimState.Run:
                Speed = (_speed / 2f);
                AnimatorRef.speed = Speed;
                break;

            case _AnimState.Swimming:
                Speed = (_speed / 4f);
                AnimatorRef.speed = Speed;
                break;

            case _AnimState.Climbing:
                Speed = (_speed / 1.5f);
                AnimatorRef.speed = Speed;
                break;
        }
    }
}
