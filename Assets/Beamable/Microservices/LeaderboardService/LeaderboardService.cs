using System;
using System.Collections.Generic;
using System.Linq;
using Beamable.Common;
/*using Beamable.Serialization.SmallerJSON;*/
using Beamable.Server;
using MongoDB.Driver;
using UnityEngine;

namespace Beamable.Microservices
{
	[Microservice("LeaderboardService")]
	public class LeaderboardService : Microservice
	{

        [ClientCallable]
        public async Promise<bool> SaveEntry(string _playerName, string _petName, int _petRank, int _petXp)
        {
            bool isSuccess = false;

            try
            {
                // MongoDB connection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");

                var filter = Builders<PlayerEntry>.Filter.Empty;
                if (filter != null)
                {
                    Debug.Log($"Test: Database already populated");
                }

                // create new data
                collection.InsertOne(new PlayerEntry()
                {
                    playerName = _playerName,
                    petName = _petName,
                    petRank = _petRank,
                    petXp = _petXp

                });
                Debug.Log($"{_playerName}'s data added to database");

                // add method to update data!!!

                isSuccess = true;
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

            return isSuccess;

        }

        [ClientCallable]
        public async void UpdateEntry(string _playerName)
        {
            // MongoDB connection
            var db = await Storage.GetDatabase<LeaderboardStorage>();
            var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
            var filter = Builders<PlayerEntry>.Filter.Eq("playerName", _playerName);
        }

        [ClientCallable]
        public async Promise<List<string>> GetEntry()
        {
            // Filter entries based on playerName
            var filter = Builders<PlayerEntry>.Filter.Empty;
           
            var db = await Storage.GetDatabase<LeaderboardStorage>();
            var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
            var LeaderboardEntries = collection
               .Find(filter)
               .ToList();

            List<string> entries = new List<string>();


            // Sort retrieved data into an object, then return list of entry objects
            foreach (PlayerEntry entry in LeaderboardEntries)
            {
                var jsonEntry = JsonUtility.ToJson(entry);
                /*JObject jsonEntry = JObject.FromObject(entry);*/
                entries.Add(jsonEntry);

            }

            return entries;
        }


    }

    
}
