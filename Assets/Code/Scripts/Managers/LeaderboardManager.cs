using System;
using Beamable.Server.Clients;
using Beamable;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardServiceTest : MonoBehaviour
{
    //  Variables/Fields  ---------------------------------------
    [SerializeField] public PetDataRef petDataRef;
    public PetLocalData petLocalRef;

    [SerializeField] public Transform entryContainer;
    [SerializeField] public Transform entryTemplate;
    private List<LeaderboardEntry> leaderboardEntryList;
    private List<Transform> leaderboardEntryTransformList;

    private LeaderboardServiceClient _LeaderboardServiceClient = null;

    private class LeaderboardEntry
    {
        public string playerName;
        public string petName;
        public int petRank;
        public int petXp;
    }

    private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);

        // Check if the current scene is "Leaderboard"
        if (SceneManager.GetActiveScene().name == "Leaderboard")
        {
            // Only generate leaderboard if Entry Container and Entry Template are assigned
            if (entryContainer != null && entryTemplate != null)
            {
                generateLeaderboard();
            }
            else
            {
                Debug.LogWarning("Entry Container or Entry Template not assigned in the scene.");
            }
        }

        Debug.Log($"Start()"); // Optionally keep this log
    }

    private void Start()
    {
        _LeaderboardServiceClient = new LeaderboardServiceClient();
    }

    //  Methods  --------------------------------------

    private async Task<List<LeaderboardEntry>> LeaderboardService()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");

        // Call Microservice method
        List<string> jsonEntry = await _LeaderboardServiceClient.GetAllEntries();
        List<LeaderboardEntry> entryList = new List<LeaderboardEntry>();

        // Convert JSON string to object
        foreach (string entry in jsonEntry)
        {
            JObject parsedJson = JObject.Parse(entry);
            string playerName = (string)parsedJson["playerName"];
            string petName = (string)parsedJson["petName"];
            int petRank = (int)parsedJson["petRank"];
            int petXp = (int)parsedJson["petXp"];

            // populate current leaderboard list
            entryList.Add(new LeaderboardEntry { playerName = playerName, petName = petName, petRank = petRank, petXp = petXp });
        }

        return entryList;
    }

    private async void generateLeaderboard()
    {
        List<LeaderboardEntry> tempEntries = await LeaderboardService();

        leaderboardEntryList = tempEntries;

        // sort leaderboard entries by petXp
        leaderboardEntryList.Sort((entry1, entry2) => entry2.petXp.CompareTo(entry1.petXp));

        // Instantiate leaderboard entry transforms
        leaderboardEntryTransformList = new List<Transform>();
        foreach (LeaderboardEntry leaderboardEntry in leaderboardEntryList)
        {
            CreateLeaderboardEntryTransform(leaderboardEntry, entryContainer, leaderboardEntryTransformList);
        }
    }

    // Method to create a single leaderboard entry
    private void CreateLeaderboardEntryTransform(LeaderboardEntry leaderboardEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 20f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        // create rank suffix
        int rank = transformList.Count + 1;
        string rankString = rank == 1 ? "1ST" : rank == 2 ? "2ND" : rank == 3 ? "3RD" : $"{rank}TH";

        entryTransform.Find("petStanding").GetComponent<Text>().text = rankString;
        entryTransform.Find("petName").GetComponent<Text>().text = leaderboardEntry.petName;
        entryTransform.Find("petRank").GetComponent<Text>().text = leaderboardEntry.petRank.ToString();
        entryTransform.Find("petXp").GetComponent<Text>().text = leaderboardEntry.petXp.ToString();

        transformList.Add(entryTransform);
    }

    // Method to check if the entry exists, and if not, create it
    public async Task CheckAndCreateLeaderboardEntry()
    {
        var beamContext = BeamContext.Default;
        await beamContext.OnReady;

        string playerId = beamContext.PlayerId.ToString();
        string existingEntry = await _LeaderboardServiceClient.GetEntry(playerId);

        // If entry does not exist, create a new one
        if (existingEntry == "null")
        {
            string petName = petDataRef.petData.petname;
            int petRank = getPetRank();
            int petXp = getPetXp();

            // Create a new entry using the microservice
            await _LeaderboardServiceClient.CreateEntry(beamContext.PlayerId.ToString(), petName, petRank, petXp);
            Debug.Log($"Created leaderboard entry for {petName}");
        }
        else
        {
            Debug.Log("Leaderboard entry already exists.");
        }
    }

    public int getPetXp()
    {
        float petXp = petDataRef.petData.xp;
        return (int)Math.Round(petXp);
    }

    public int getPetRank()
    {
        return petDataRef.petData.rank;
    }
}
