using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetConfigs", menuName = "ScriptableObject/PetConfigs", order = 100)]

public class PetConfigs : ScriptableObject
{
    [Header("Movement EDITOR")]
    public float Incrimental;
    public float Acceleration;
    public float RotationSpeed;
    public float Speed;
    [Space]
    [Header("NAV MESH CONFIGS")]
    public float BaseSpeed;
    public float BaseAcceleration;
    [Header("BOOSTERS Multipler")]
    public float RunMultipler;
    public float SwimMultiplier;
    public float ClimbMultiplier;
    public float FlyMultiplier;
    [Header("Player State")]
    public _Playertate State;

}
