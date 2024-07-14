using Beamable;
using Beamable.CloudSavingService;
using Beamable.Server.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ExtraPlayerDataManager : MonoBehaviour
{
    private BeamContext beamContext;
    private ExtraPlayerDataServiceClient _ExtraPlayerDataServiceClient = null;

    [SerializeField]
    private ExtraPlayerDataListSO extraPlayerDataListSO;

    public class ExtraPlayerData
    {
        public string objectId;
        public string playerId;
        public int introTutorial;
        public int eatTutorial;
        public int showerTutorial;
        public int inventoryTutorial;
    }

    // Start is called before the first frame update
    public async void Start()
    {
        _ExtraPlayerDataServiceClient = new ExtraPlayerDataServiceClient();
        await SaveDataFromPrefs();
        await ExtraPlayerDataService();
        Debug.Log($"Data manager running: {extraPlayerDataListSO.extraPlayerData.playerId}");
    }


    public async Task<List<ExtraPlayerData>> ExtraPlayerDataService()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        // Call Microservice method
        string jsonEntry = await _ExtraPlayerDataServiceClient.GetEntryByPlayerId(beamContext.PlayerId.ToString());
        List<ExtraPlayerData> entryList = new List<ExtraPlayerData>();

        if (jsonEntry == "null")
        {
            Debug.Log("Json entry is empty. Create one");
            await _ExtraPlayerDataServiceClient.CreateEntry(
                beamContext.PlayerId.ToString(),
                0,
                0,
                0,
                0);

            jsonEntry = await _ExtraPlayerDataServiceClient.GetEntryByPlayerId(beamContext.PlayerId.ToString());
        }

        // Parse json string to .net
        JObject parsedJson = JObject.Parse(jsonEntry);

        string objectId = (string)parsedJson["Id"];
        string playerId = (string)parsedJson["playerId"];
        int introTutorial = (int)parsedJson["introTutorial"];
        int eatTutorial = (int)parsedJson["eatTutorial"];
        int showerTutorial = (int)parsedJson["showerTutorial"];
        int inventoryTutorial = (int)parsedJson["inventoryTutorial"];

        entryList.Add(
                new ExtraPlayerData { objectId = objectId, playerId = playerId, introTutorial = introTutorial, eatTutorial = eatTutorial, showerTutorial = showerTutorial, inventoryTutorial = inventoryTutorial }
            );

        
        // Add the entries to the ScriptableObject
        if (extraPlayerDataListSO != null)
        {
            foreach (var entry in entryList)
            {
                extraPlayerDataListSO.SetAllExtraPlayerData(entry.objectId, entry.playerId, entry.introTutorial, entry.eatTutorial, entry.showerTutorial, entry.inventoryTutorial);
            }

        }
        else
        {
            Debug.LogWarning("ScriptableObject is not assigned.");
        }

        return entryList;

    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Saving Data on Pause...");
            _ExtraPlayerDataServiceClient.UpdateEntryByPlayerId(extraPlayerDataListSO.extraPlayerData.playerId, extraPlayerDataListSO.extraPlayerData.introTutorial, extraPlayerDataListSO.extraPlayerData.eatTutorial, extraPlayerDataListSO.extraPlayerData.showerTutorial, extraPlayerDataListSO.extraPlayerData.inventoryTutorial);
        }
        else
        {
            if (extraPlayerDataListSO.extraPlayerData != null)
            {
                //await ExtraPlayerDataService();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (extraPlayerDataListSO != null && extraPlayerDataListSO.extraPlayerData != null)
        {
            PlayerPrefs.SetInt("IntroTutorial", extraPlayerDataListSO.extraPlayerData.introTutorial);
            PlayerPrefs.SetInt("EatTutorial", extraPlayerDataListSO.extraPlayerData.eatTutorial);
            PlayerPrefs.SetInt("ShowerTutorial", extraPlayerDataListSO.extraPlayerData.showerTutorial);
            PlayerPrefs.SetInt("InventoryTutorial", extraPlayerDataListSO.extraPlayerData.inventoryTutorial);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("extraPlayerDataListSO or extraPlayerDataListSO.extraPlayerData is null.");
        }

        Debug.Log("Saving Data on Quit...");
    }

    private async Task SaveDataFromPrefs()
    {
        if (_ExtraPlayerDataServiceClient != null)
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;

            await _ExtraPlayerDataServiceClient.UpdateEntryByPlayerId(beamContext.PlayerId.ToString(), PlayerPrefs.GetInt("IntroTutorial", 0), PlayerPrefs.GetInt("EatTutorial", 0), PlayerPrefs.GetInt("ShowerTutorial", 0), PlayerPrefs.GetInt("InventoryTutorial", 0));
            Debug.Log("Pushing data from playerprefs to ExtraPlayerData storage");
        }
        else
        {
            Debug.LogError("extraPlayerDataListSO or extraPlayerDataListSO.extraPlayerData is null.");
        }
    }
}