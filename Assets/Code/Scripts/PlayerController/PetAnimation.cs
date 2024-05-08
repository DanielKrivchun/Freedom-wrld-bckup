using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
    public Animator Animator;

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
                Animator.SetFloat(_Strings.Velocity, 0);
                Animator.SetBool(_Strings.Idle, true);
                m_currunt_anim_state = _AnimState.Idle;
                break;

            case _AnimState.Wallk:
                break;

            case _AnimState.Run:
                Animator.SetBool(_Strings.Idle, false);
                Animator.SetFloat(_Strings.Velocity, 1);
                m_currunt_anim_state = _AnimState.Run;
                break;
            
            case _AnimState.Jump:
                break;

            case _AnimState.Running:
                break;

            case _AnimState.Eating:
                Animator.SetTrigger(_Strings.Eat);
                m_currunt_anim_state = _AnimState.Eating;
                break;

            case _AnimState.Happy:
                Animator.SetTrigger(_Strings.Happy);
                m_currunt_anim_state = _AnimState.Happy;
                break;

            case _AnimState.Toilet:
                Animator.SetTrigger(_Strings.Toilet);
                m_currunt_anim_state = _AnimState.Toilet;
                break;

            case _AnimState.Bath:
                Animator.SetTrigger(_Strings.Bath);
                m_currunt_anim_state = _AnimState.Bath;
                break;

            case _AnimState.Sleep:
                Animator.SetTrigger(_Strings.Sleep);
                m_currunt_anim_state = _AnimState.Sleep;
                break;

            default:
                break;
        }
    }
}
