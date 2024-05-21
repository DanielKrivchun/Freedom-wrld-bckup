

using Fusion;
using System.Collections;
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
    public static string Happy = "Happy";
    public static string Eat = "Eat";
    public static string Toilet = "Toilet";
    public static string Bath = "Bath";
    public static string Sleep = "Sleep";
}

public enum PetCareState
{
    Happy,
    Feed,
    Clean,
    Energy
}


public static class Utils
{
    public static bool IsLocalPlayer(NetworkObject networkObj)
    {
        return networkObj.IsValid == networkObj.HasInputAuthority;
    }
    public static IEnumerator PlayAnimAndSetStateWhenFinished(GameObject parent, Animator animator, string clipName, bool activeStateAtTheEnd = true)
    {
        animator.Play(clipName);

        var animationLengt = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(animationLengt);

        parent.SetActive(activeStateAtTheEnd);
    }
}
