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
                break;

            case _AnimState.Happy:
                break;

            case _AnimState.Toilet:
                break;

            case _AnimState.Bath:
                break;

            case _AnimState.Sleep:
                break;

            default:
                break;
        }
    }
}
