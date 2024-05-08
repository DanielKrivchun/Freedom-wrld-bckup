

using UnityEngine;

public enum _AnimState
{
    Idle,
    Wallk,
    Run,
    Eating,
    Jump,
    Running,
    Happy,
    Toilet,
    Bath,
    Sleep
}

public enum _Playertate
{
    None,
    RunState,
    SwimState,
    ClimbState,
    FlyState
}

[System.Serializable]
public class _WayPoints
{
    public Transform[] m_points;
}

public class _Strings
{
    public static string Velocity = "Velocity";
    public static string Jumping = "Jumping";
    public static string Idle = "Idle";
}