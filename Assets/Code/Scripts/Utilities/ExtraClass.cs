

using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    Climbing,
    Stumble
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
    public static string StumbleBool = "StumbleBool";
    public static string FoamBubble = "FoamBubble";

    public static string XpToAdd = "XpToAdd";
    public static string CoinsToAdd = "CoinsToAdd";
    public static string DatFromRaceScene = "DatFromRaceScene";


    public static string RaceScene = "RaceScene";
    public static string PetCareScene = "PetCareScene";


    public static string HostMigrated = "You have been disconnected from the race and cannot rejoin.";
    public const string ServerError = "Disconnected from server.";
    public const string ConnectuonFailed = "Connectiton Failed.";
    public const string LeftOnlyOnePlay = "All players left the game.";
}


public struct _Tags
{
    public const string WinLine = "WinLine";
    public const string Water = "Water";
    public const string Flying = "Flying";
    public const string Land = "Land";
    public const string Climbing = "Climbing";
    public const string Jack = "Jack";
    public const string JackSecond = "JackSecond";
    public const string Rank = "Rank";
}

[System.Serializable]
public class _FakeUsers
{
    public List<string> Names;
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

[System.Serializable]
public class _RankPlayers
{
    public int PathNo;
    public float MyDistance;

}

[System.Serializable]
public class _StumbleObjects
{
    public GameObject MyColider;
    public GameObject CannonPosition;
    public GameObject ParticleEffect;
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

    //DO TWEENER ANIMATIONS
    public static void _DoUIPopup(Transform _t, float _time, Ease _e)
    {
        _t.DOScale(1f, _time).SetEase(_e);
    }

    public static void _DoButtonAnimation(Transform _t)
    {
        _t.DOScale(1.1f, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _t.DOScale(1f, 0.1f);
        });
    }
    public static void _DoButtonAnimation(Transform _t, Ease _e)
    {
        _t.DOScale(1.1f, 0.1f).SetEase(_e).OnComplete(() =>
        {
            _t.DOScale(1f, 0.1f);
        });
    }

    public static void _DoButtonAnimation(Transform _t, GameObject _obj)
    {
        _t.DOScale(1.1f, 0.1f).OnComplete(() =>
        {
            _t.DOScale(1f, 0.1f);
            _obj.SetActive(false);
        });
    }

    public static async Task _Waiter(int m_mili_second_to_wait)
    {
        await Task.Delay(m_mili_second_to_wait);
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
