

using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum _AnimState
{
    Idle,
    Wallk,
    Run,
    Eating,
    Jump,
    Flying,
    Swimming,
    Happy,
    Bath,
    Sleep,
    Sick
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
    public List<Transform> m_points;
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
    public static string IsSleeping = "IsSleeping";
    public static string IsSick = "IsSick";

    public static string FoamBubble = "FoamBubble";
}

public struct _Tags
{
    public const string WinLine = "WinLine";
    public const string Water = "Water";
    public const string Flying = "Flyng";
    public const string Land = "Land";
}

public enum PetCareState
{
    Happy,
    Eat,
    Clean,
    Energy
}

public enum PetTraining
{
    NotSelected,
    Running,
    Climbing,
    Swimming,
    Flying,
    Intelligence
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

    public static FoodItems IdentifyMyFoodType(string foodName)
    {
        switch (foodName)
        {
            case "Apple":
                return FoodItems.Apple;

            case "Mushroom":
                return FoodItems.Mushroom;
        }

        return FoodItems.None;
    }
}
