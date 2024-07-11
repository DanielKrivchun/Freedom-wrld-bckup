using Beamable.Common;
using Beamable.Mongo;
using Beamable.Server;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Beamable.Microservices
{
	[Microservice("ExtraPlayerDataService")]
	public class ExtraPlayerDataService : Microservice
	{

        [ClientCallable]
		public void ServerCall()
		{
			// This code executes on the server.
		}

        [ClientCallable]
        public async void CreateEntry(string _playerId, int _introTutorial, int _eatTutorial, int _showerTutorial, int _inventoryTutorial)
        {
            try
            {
                // Declare "ExtraPlayerDataStorage" collection
                var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
                var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerDataStorage");

                // Check if an entry with the same playerId already exists
                var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", _playerId);
                var existingEntry = await collection.Find(filter).FirstOrDefaultAsync();

                if (existingEntry != null)
                {
                    Debug.LogWarning($"Entry for playerId {_playerId} already exists. Skipping creation.");
                    return;
                }

                // Create new data
                collection.InsertOne(new ExtraPlayerData()
                {
                    playerId = _playerId,
                    introTutorial = _introTutorial,
                    eatTutorial = _eatTutorial,
                    showerTutorial = _showerTutorial,
                    inventoryTutorial = _inventoryTutorial
                });

                Debug.Log($"Added {_playerId}'s data to database, Intro Tutorial is: {_introTutorial}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [ClientCallable]
        public async void UpdateTutorial(string _playerId, string _tutorialDone)
        {
            try
            {
                // Declare "LeaderboardStorage" collection
                var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
                var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerData");
                var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", _playerId);
                var update = Builders<ExtraPlayerData>.Update.Set(_tutorialDone, 1);

                // Update petRank data
                collection.UpdateOne(filter, update);

                Debug.Log("Updated player entry");
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

        }

        [ClientCallable]
        public async void UpdateEntryByPlayerId(string _playerId, int _introTutorial, int _eatTutorial, int _showerTutorial, int _inventoryTutorial)
        {
            try
            {
                // Declare "ExtraPlayerDataStorage" collection
                var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
                var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerData");

                var filter = Builders<ExtraPlayerData>.Filter.Eq("Id", "66881db136782f3c03103692");
                var update = Builders<ExtraPlayerData>.Update
                    .Set("playerId", _playerId)
                    .Set("introTutorial", _introTutorial)
                    .Set("eatTutorial", _eatTutorial)
                    .Set("showerTutorial", _showerTutorial)
                    .Set("inventoryTutorial", _inventoryTutorial);

                // Update the document in the collection
                var result = await collection.UpdateOneAsync(filter, update);

                // Check if the update was acknowledged and successful
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    Debug.Log($"Updated entry for playerId {_playerId} in microservice storage. introTutorial: {_introTutorial}, eatTutorial: {_eatTutorial}, showerTutorial: {_showerTutorial}, inventoryTutorial: {_inventoryTutorial}");
                }
                else
                {
                    Debug.LogWarning($"No document found with playerId {_playerId} to update.");
                }

                //Debug.Log($"This is what the result is: {result}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        [ClientCallable]
        public async void UpdateEntryTest(string _id, string _playerId)
        {
            Debug.Log("Trying to update the entry? Maybe it worked maybe it didn't??????");

            var updateData = new ExtraPlayerData
            {
                playerId = "1766823754227713",
                introTutorial = 1,
                eatTutorial = 1,
                showerTutorial = 1,
                inventoryTutorial = 1
            };

            try
            {   
                await Storage.Update<ExtraPlayerDataStorage, ExtraPlayerData>(_id, updateData);
                Debug.Log("Update successful");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Update failed: {ex.ToString()}");
            }
        }

        [ClientCallable]
        public async Promise<string> GetEntryByPlayerId(string playerId)
        {
            // Filter entry by playerId
            var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", playerId);
            var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
            var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerDataStorage");
            var ExtraPlayerDataEntry = await collection
               .Find(filter)
               .FirstOrDefaultAsync();

            // Convert the entry to a JSON string
            var jsonEntry = JsonUtility.ToJson(ExtraPlayerDataEntry);

            Debug.Log($"THE JSON ENTRY IS: {jsonEntry}, CAN WE SEE THE OBJECT ID");

            return jsonEntry;
        }
    }
}
