using Beamable.Server;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtraPlayerDataList", menuName = "ScriptableObject/ExtraPlayerDataList", order = 1)]
public class ExtraPlayerDataListSO : ScriptableObject
{
    public ExtraPlayerData extraPlayerData;

    public void SetAllExtraPlayerData(string objectId, string playerId, int introTutorial, int eatTutorial, int showerTutorial, int inventoryTutorial)
    { 
        extraPlayerData = new ExtraPlayerData();

        extraPlayerData.objectId = objectId;
        extraPlayerData.playerId = playerId;
        extraPlayerData.introTutorial = introTutorial;
        extraPlayerData.eatTutorial = eatTutorial;
        extraPlayerData.showerTutorial = showerTutorial;
        extraPlayerData.inventoryTutorial = inventoryTutorial;
    }
}