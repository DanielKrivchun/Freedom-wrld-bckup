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
                // Declare "LeaderboardStorage" collection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");

                // create new data
                collection.InsertOne(new PlayerEntry()
                {
                    playerName = _playerName,
                    petName = _petName,
                    petRank = _petRank,
                    petXp = _petXp

                });
                Debug.Log($"Added {_playerName}'s data added to database");

                isSuccess = true;
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

            return isSuccess;

        }

        [ClientCallable]
        public async Promise<bool> UpdateRank(string _playerName, int _petRank)
        {
            bool isSuccess = false;

            try
            {
                // Declare "LeaderboardStorage" collection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
                var filter = Builders<PlayerEntry>.Filter.Eq("playerName", _playerName);
                var update = Builders<PlayerEntry>.Update.Set("petRank", _petRank);

                // Update petRank data
                collection.UpdateOne(filter, update);
                Debug.Log($"Updated {_playerName}'s rank");

                isSuccess = true;
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

            return isSuccess;

        }

        [ClientCallable]
        public async Promise<bool> UpdateXp(string _playerName, int _petXp)
        {
            bool isSuccess = false;

            try
            {
                // Declare "LeaderboardStorage" collection
                var db = await Storage.GetDatabase<LeaderboardStorage>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardStorage");
                var filter = Builders<PlayerEntry>.Filter.Eq("playerName", _playerName);
                var update = Builders<PlayerEntry>.Update.Set("petXp", _petXp);

                // Update petRank data
                collection.UpdateOne(filter, update);
                Debug.Log($"Updated {_playerName}'s petXp");

                isSuccess = true;
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

            return isSuccess;

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
