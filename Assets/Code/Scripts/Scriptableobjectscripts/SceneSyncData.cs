using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabHolder", menuName = "ScriptableObject/SceneSyncData", order = 100)]
public class SceneSyncData : ScriptableObject
{
    public bool ShowWelcomeScreen;

    public int XpGained;
    public int CoinsGained;

    public void _Reset()
    {
        ShowWelcomeScreen = false;
        XpGained = 0;
        CoinsGained = 0;
    }
}
