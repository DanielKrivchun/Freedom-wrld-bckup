

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
    Sick,
    Climbing
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
    public static string PetID = "PetID";

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
    public const string Flying = "Flying";
    public const string Land = "Land";
    public const string Climbing = "Climbing";
    public const string Jack = "Jack";
}

public enum PetCareState
{
    None,
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

            case "Orange":
                return FoodItems.Orange;

            case "KaleSalad":
                return FoodItems.KaleSalad;

            case "GoldenApple":
                return FoodItems.GoldenApple;

            case "EnergyDrink":
                return FoodItems.EnergyDrink;

            case "Fairy":
                return FoodItems.Fairy;

            case "GoldenFairy":
                return FoodItems.GoldenFairy;

            case "CosmicBerryElectrolyteDrink":
                return FoodItems.CosmicBerryElectrolyteDrink;

            case "ProteinShake":
                return FoodItems.ProteinShake;

            case "MiracleCognitiveSupplements":
                return FoodItems.MiracleCognitiveSupplements;

            case "AntiBiotics":
                return FoodItems.AntiBiotics;
        }

        return FoodItems.None;
    }
}
