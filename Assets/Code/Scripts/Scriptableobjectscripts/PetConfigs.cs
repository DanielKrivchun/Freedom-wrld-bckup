using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetConfigs", menuName = "ScriptableObject/PetConfigs", order = 100)]

public class PetConfigs : ScriptableObject
{
    [Header("Movement EDITOR")]
    public float m_incrimental;
    public float m_acceleration;
    public float m_rotationspeed;
    public float m_speed;
    [Space]
    [Header("NAV MESH CONFIGS")]
    [Header("Stearing")]
    public float BaseSpeed;
    public float Acceleration;
    [Header("BOOSTERS Multipler")]
    public float m_run_multiplier;
    public float m_swim_multiplier;
    public float m_climb_multiplier;
    public float m_fly_multiplier;
    [Space]
    public int MyCoins;
    public int MyXp;
    [Space]
    public float maxstemina;
    [Space]
    public float TapMultiplier;
    [Header("Player State")]
    public _Playertate m_player_state;

}
