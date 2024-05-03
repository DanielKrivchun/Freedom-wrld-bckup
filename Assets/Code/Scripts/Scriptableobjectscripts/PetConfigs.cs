using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetConfigs", menuName = "ScriptableObject/PetConfigs", order = 100)]

public class PetConfigs : ScriptableObject
{
    [Header("Movement")]
    public float Incrimental;
    public float Acceleration;
    public float RotationSpeed;
    public float Speed;
    [Space]
    [Header("NAV MESH CONFIGS")]
    public float NavSpeed;
    public float NavAcceleration;
}
