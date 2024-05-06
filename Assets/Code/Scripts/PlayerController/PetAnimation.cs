using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
    public Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// CHANGE ANIMATIONS HERE
    /// SEND PARAMATER TO CHANGE
    /// </summary>
    public void _ChangeAnimationState(_AnimState _state)
    {
        switch (_state)
        {
            case _AnimState.Idle:
                break;
            case _AnimState.Wallk:
                break;
            case _AnimState.Run:
                break;
            case _AnimState.Eating:
                break;
            case _AnimState.Jump:
                break;
            case _AnimState.Running:
                break;
            default:
                break;
        }
    }
}
