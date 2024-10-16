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
        public async void CreateEntry(string _playerName, string _petName, int _petRank, int _petXp)
        {

            try
            {
                // Declare or target existing "LeaderboardStorage" collection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");

                // create new data within collection
                collection.InsertOne(new PlayerEntry()
                {
                    playerName = _playerName,
                    petName = _petName,
                    petRank = _petRank,
                    petXp = _petXp

                });
                Debug.Log($"Added {_playerName}'s data added to database");

            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }


        }

        [ClientCallable]
        public async void UpdateRank(string _playerName, int _petRank)
        {
            try
            {
                // Connect to LeaderboardStorage collection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
                var filter = Builders<PlayerEntry>.Filter.Eq("playerName", _playerName);
                var update = Builders<PlayerEntry>.Update.Set("petRank", _petRank);

                // Update petRank data
                collection.UpdateOne(filter, update);

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [ClientCallable]
        public async void UpdateXp(string _playerName, int _petXp)
        {
            try
            {
                // Declare "LeaderboardStorage" collection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
                var filter = Builders<PlayerEntry>.Filter.Eq("playerName", _playerName);
                var update = Builders<PlayerEntry>.Update.Set("petXp", _petXp);

                // Update petRank data
                collection.UpdateOne(filter, update);
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

        }

        [ClientCallable]
        public async Promise<List<string>> GetAllEntries()
        {
            // Filter all entries from service storage
            var filter = Builders<PlayerEntry>.Filter.Empty;
            var db = await Storage.GetDatabase<LeaderboardStorage>();
            var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
            var LeaderboardEntries = collection
               .Find(filter)
               .ToList();

            List<string> entries = new List<string>();

            // Create a list of json strings
            foreach (PlayerEntry entry in LeaderboardEntries)
            {
                var jsonEntry = JsonUtility.ToJson(entry);
                entries.Add(jsonEntry);

            }

            return entries;
        }


    }

    
}
