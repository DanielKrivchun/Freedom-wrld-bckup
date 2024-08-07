using Beamable;
using Beamable.CloudSavingService;
using Beamable.Server.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

    [Header("Pet Care Stat Data Reference")]
    public PetCareStatData petCareStatDatRef;

    [SerializeField]
    private ExtraPlayerDataListSO extraPlayerDataListSO;

    [Space]
    public GetServerTime getServerTime;

    public class ExtraPlayerData
    {
        public string objectId;
        public string playerId;
        public int introTutorial;
        public int eatTutorial;
        public int showerTutorial;
        public int inventoryTutorial;
        public string vitaminsAteTime;
    }

    // Start is called before the first frame update
    public async void Start()
    {
        _ExtraPlayerDataServiceClient = new ExtraPlayerDataServiceClient();
        await SaveDataFromPrefs();
        await ExtraPlayerDataService();
        await CheckVitaminsTime();
        Debug.Log($"Data manager running: {extraPlayerDataListSO.extraPlayerData.playerId}");
    }

    //This is a funciton that needs to be made. If pet dies and new pet is created need to reset stats like when they last took vitamins
    //public async ResetPetData()
    //{ }

    #region Call to create entry if needed, which then fills the EPD Scriptable Object
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
                0,
                "");

            jsonEntry = await _ExtraPlayerDataServiceClient.GetEntryByPlayerId(beamContext.PlayerId.ToString());
        }

        // Parse json string to .net
        JObject parsedJson = JObject.Parse(jsonEntry);

        string objectId         = (string)parsedJson["Id"];
        string playerId         = (string)parsedJson["playerId"];
        int introTutorial       = (int)parsedJson["introTutorial"];
        int eatTutorial         = (int)parsedJson["eatTutorial"];
        int showerTutorial      = (int)parsedJson["showerTutorial"];
        int inventoryTutorial   = (int)parsedJson["inventoryTutorial"];
        string vitaminsAteTime  = (string)parsedJson["vitaminsAteTime"];

        entryList.Add(
                new ExtraPlayerData { objectId = objectId, playerId = playerId, introTutorial = introTutorial, eatTutorial = eatTutorial, showerTutorial = showerTutorial, inventoryTutorial = inventoryTutorial, vitaminsAteTime = vitaminsAteTime }
            );

        
        // Add the entries to the ScriptableObject
        if (extraPlayerDataListSO != null)
        {
            foreach (var entry in entryList)
            {
                extraPlayerDataListSO.SetAllExtraPlayerData(entry.objectId, entry.playerId, entry.introTutorial, entry.eatTutorial, entry.showerTutorial, entry.inventoryTutorial, entry.vitaminsAteTime);
            }

        }
        else
        {
            Debug.LogWarning("ScriptableObject is not assigned.");
        }

        return entryList;

    }

    #endregion

    #region Food Timings
    public void SetVitaminsAteTime()
    {
        getServerTime.GetCurrentTime(timeNow => { extraPlayerDataListSO.extraPlayerData.vitaminsAteTime = timeNow.ToString(); });
        //Debug.Log($"Vitamins eaten: {extraPlayerDataListSO.extraPlayerData.vitaminsAteTime}");
    }

    private async Task CheckVitaminsTime()
    {
        DateTime currentTime = await getServerTime.GetCurrentTimeTask();

        if (DateTime.TryParse(extraPlayerDataListSO.extraPlayerData.vitaminsAteTime, out DateTime vitaminsTime))
        {
            TimeSpan timeSinceVitamins = currentTime - vitaminsTime;
            if (timeSinceVitamins.Days >= 3)
            {
                // Logic for when vitamins were eaten more than 3 days ago
                Debug.Log("Vitamins were eaten more than 3 days ago.");
                // Reset the flu chance or perform any other actions needed
                petCareStatDatRef.fluChance = 3;

                extraPlayerDataListSO.extraPlayerData.vitaminsAteTime = "";
            }
            else
            {
                Debug.Log("Vitamins were eaten less than 3 days ago.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid date format for vitaminsAteTime.");
        }
    }
    #endregion

    #region Save On Quit
    private void OnApplicationQuit()
    {
        if (extraPlayerDataListSO != null && extraPlayerDataListSO.extraPlayerData != null)
        {
            PlayerPrefs.SetInt("IntroTutorial", extraPlayerDataListSO.extraPlayerData.introTutorial);
            PlayerPrefs.SetInt("EatTutorial", extraPlayerDataListSO.extraPlayerData.eatTutorial);
            PlayerPrefs.SetInt("ShowerTutorial", extraPlayerDataListSO.extraPlayerData.showerTutorial);
            PlayerPrefs.SetInt("InventoryTutorial", extraPlayerDataListSO.extraPlayerData.inventoryTutorial);
            PlayerPrefs.SetString("vitaminsAteTime", extraPlayerDataListSO.extraPlayerData.vitaminsAteTime);
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

            await _ExtraPlayerDataServiceClient.UpdateEntryByPlayerId(beamContext.PlayerId.ToString(), PlayerPrefs.GetInt("IntroTutorial", 0), PlayerPrefs.GetInt("EatTutorial", 0), PlayerPrefs.GetInt("ShowerTutorial", 0), PlayerPrefs.GetInt("InventoryTutorial", 0), PlayerPrefs.GetString("vitaminsAteTime"));
            Debug.Log("Pushing data from playerprefs to ExtraPlayerData storage");
        }
        else
        {
            Debug.LogError("extraPlayerDataListSO or extraPlayerDataListSO.extraPlayerData is null.");
        }
    }

    #endregion
}