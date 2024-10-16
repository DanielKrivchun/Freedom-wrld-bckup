using Beamable.Common;
using Beamable.Mongo;
using Beamable.Server;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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

        #region Web3/NFT Ownership

        [ClientCallable]
        public async void UpdateNftOwned(string _playerId, bool _nftOwned)
        {
            try
            {
                var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
                var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerDataStorage");
                var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", _playerId);
                var update = Builders<ExtraPlayerData>.Update.Set("nftOwned", _nftOwned);

                // Update nftOwned bool
                collection.UpdateOne(filter, update);
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

        }

        [ClientCallable]
        public async Promise<string> GetAddress(string _playerId)
        {
            var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", _playerId);
            var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
            var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerDataStorage");

            // Projection to return only the walletAddress field
            var projection = Builders<ExtraPlayerData>.Projection.Include("walletAddress").Exclude("_id");

            // Find the first document matching the filter and apply the projection
            var result = collection
                .Find(filter)
                .Project(projection)
                .FirstOrDefault();

            // If no result is found, return null or an appropriate message
            if (result == null)
            {
                return "no address found";  // or return "No address found";
            }
            var jsonResult = JsonUtility.ToJson(result);

            /*var jsonResult = result.ToJson();*/
            return jsonResult;
        }

        #endregion

        #region Create/Update Entry

        [ClientCallable]
        public async void CreateEntry(string _playerId, int _introTutorial, int _eatTutorial, int _showerTutorial, int _inventoryTutorial, string _vitaminsAteTime, string _cosmicBerryElectrolyteBoughtTime, string _miracleCognitiveBoughtTime, string _proteinShakeBoughtTime)
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
                    inventoryTutorial = _inventoryTutorial,
                    vitaminsAteTime = _vitaminsAteTime,
                    cosmicBerryElectrolyteBoughtTime = _cosmicBerryElectrolyteBoughtTime,
                    miracleCognitiveBoughtTime = _miracleCognitiveBoughtTime,
                    proteinShakeBoughtTime = _proteinShakeBoughtTime

                });
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
                var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
                var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerDataStorage");
                var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", _playerId);
                var update = Builders<ExtraPlayerData>.Update.Set(_tutorialDone, 0);

                // Update petRank data
                collection.UpdateOne(filter, update);

                Debug.Log("Updated player tutorial");
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

        }

        [ClientCallable]
        public async Task UpdateEntryByPlayerId(string _playerId, int _introTutorial, int _eatTutorial, int _showerTutorial, int _inventoryTutorial, string _vitaminsAteTime, string _cosmicBerryElectrolyteBoughtTime, string _miracleCognitiveBoughtTime, string _proteinShakeBoughtTime)
        {
            try
            {
                // Declare "ExtraPlayerDataStorage" collection
                var db = await Storage.GetDatabase<ExtraPlayerDataStorage>();
                var collection = db.GetCollection<ExtraPlayerData>("ExtraPlayerDataStorage");

                var filter = Builders<ExtraPlayerData>.Filter.Eq("playerId", _playerId);
                var update = Builders<ExtraPlayerData>.Update
                    .Set("introTutorial", _introTutorial)
                    .Set("eatTutorial", _eatTutorial)
                    .Set("showerTutorial", _showerTutorial)
                    .Set("inventoryTutorial", _inventoryTutorial)
                    .Set("vitaminsAteTime", _vitaminsAteTime)
                    .Set("cosmicBerryElectrolyteBoughtTime", _cosmicBerryElectrolyteBoughtTime)
                    .Set("miracleCognitiveBoughtTime", _miracleCognitiveBoughtTime)
                    .Set("proteinShakeBoughtTime", _proteinShakeBoughtTime);

                // Update the document in the collection
                var result = collection.UpdateOne(filter, update);

                // Check if the update was acknowledged and successful
                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    Debug.Log($"Updated entry for playerId {_playerId} in microservice storage. introTutorial: {_introTutorial}, eatTutorial: {_eatTutorial}, showerTutorial: {_showerTutorial}, inventoryTutorial: {_inventoryTutorial}, VitaminsAteTime: {_vitaminsAteTime}");
                }
                else
                {
                    Debug.LogWarning($"No document found with playerId {_playerId} to update.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion

        #region Get Entry
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

            Debug.Log($"THE storage ENTRY IS: {jsonEntry}");

            return jsonEntry;
        }

        #endregion
    }
}
