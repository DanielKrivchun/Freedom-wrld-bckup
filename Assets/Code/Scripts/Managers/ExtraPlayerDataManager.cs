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
        //await ExtraPlayerDataService();
        Debug.Log($"Data manager running: {extraPlayerDataListSO.extraPlayerData.playerId}");
        await UpdatePlayerEntry("1766823754227713", 1, 1, 1, 1);
    }

    public async Task UpdatePlayerEntry(string playerId, int introTutorial, int eatTutorial, int showerTutorial, int inventoryTutorial)
    {
        //await _ExtraPlayerDataServiceClient.UpdateEntryByPlayerId(playerId, introTutorial, eatTutorial, showerTutorial, inventoryTutorial);

        await _ExtraPlayerDataServiceClient.UpdateEntryTest(extraPlayerDataListSO.extraPlayerData.objectId, "1766823754227713");

        //Debug.Log($"Updating Player Entry: PlayerId: {playerId}, IntroTutorial: {introTutorial}, EatTutorial: {eatTutorial}, ShowerTutorial: {showerTutorial}, InventoryTutorial: {inventoryTutorial}");
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

        string objectId = (string)parsedJson["_id"];
        string playerId = (string)parsedJson["playerId"];
        int introTutorial = (int)parsedJson["introTutorial"];
        int eatTutorial = (int)parsedJson["eatTutorial"];
        int showerTutorial = (int)parsedJson["showerTutorial"];
        int inventoryTutorial = (int)parsedJson["inventoryTutorial"];

        entryList.Add(
                new ExtraPlayerData { objectId = objectId, playerId = playerId, introTutorial = introTutorial, eatTutorial = eatTutorial, showerTutorial = showerTutorial, inventoryTutorial = inventoryTutorial }
            );

        // Log each entry in the list
        foreach (var entry in entryList)
        {
            Debug.Log($"ObjectId: {entry.objectId} PlayerId: {entry.playerId}, IntroTutorial: {entry.introTutorial}, EatTutorial: {entry.eatTutorial}, ShowerTutorial: {entry.showerTutorial}, InventoryTutorial: {entry.inventoryTutorial}");
        }

        
        // Add the entries to the ScriptableObject
        if (extraPlayerDataListSO != null)
        {
            foreach (var entry in entryList)
            {
                extraPlayerDataListSO.SetAllExtraPlayerData(entry.objectId, entry.playerId, entry.introTutorial, entry.eatTutorial, entry.showerTutorial, entry.inventoryTutorial);

                Debug.Log($"Added: {entry.playerId} to the Scriptable object");
            }

        }
        else
        {
            Debug.LogWarning("ScriptableObject is not assigned.");
        }

        return entryList;

    }

    private async void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Saving Data on Pause...");
            //await UpdatePlayerEntry(extraPlayerDataListSO.extraPlayerData.playerId, 1, 1, 1, 1);
            await _ExtraPlayerDataServiceClient.UpdateEntryByPlayerId("1766823754227713", 1, 1, 1, 1);
        }
        else
        {
            if (extraPlayerDataListSO.extraPlayerData != null)
            {
                //await ExtraPlayerDataService();
            }
        }
    }

    private async void OnApplicationQuit()
    {
        Debug.Log("Saving Data on Quit...");
        //await UpdatePlayerEntry(extraPlayerDataListSO.extraPlayerData.playerId, 1, 1, 1, 1);
        //await _ExtraPlayerDataServiceClient.UpdateEntryByPlayerId(extraPlayerDataListSO.extraPlayerData.playerId, 1, 1, 1, 1);
    }
}