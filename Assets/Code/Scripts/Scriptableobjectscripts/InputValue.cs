using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputValue", menuName = "ScriptableObject/InputValue", order = 100)]
public class InputValue : ScriptableObject
{
    public bool EnableInput;
    [Space]
    public Vector2 Input;


    public void _InputSet(InputAction.CallbackContext context)
    {
        Input = context.ReadValue<Vector2>();
    }

    //public void _InputSet(InputAction.CallbackContext context)
    //{
        
    //}

    public void _InputSet(Vector2 vector2)
    {
        Input = vector2;
    }
}
