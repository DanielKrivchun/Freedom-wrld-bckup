using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputValue", menuName = "ScriptableObject/InputValue", order = 100)]
public class InputValue : ScriptableObject
{
    public bool m_enable_input;
    public bool m_enable_navmesh;
    [Space]
    public Vector2 m_input;


    public void _InputSet(InputAction.CallbackContext context)
    {
        m_input = context.ReadValue<Vector2>();
    }

    //public void _InputSet(InputAction.CallbackContext context)
    //{

    //}

    public void _InputSet(Vector2 vector2)
    {
        m_input = vector2;
    }
}
