using System;
using Beamable.Common;
using Beamable.Server;
using MongoDB.Driver;
using UnityEngine;

namespace Beamable.Microservices
{
	[Microservice("walletService")]
	public class walletService : Microservice
	{

        [ClientCallable]
		public void ServerCall()
		{
			// This code executes on the server.
		}

        #region Web3/NFT Ownership

        [ClientCallable]
        public async void CreateEntry(string _playerId, bool _nftOwned)
        {
            try
            {
                // Declare or target existing "WalletStorage" collection
                var db = await Storage.GetDatabase<WalletStorage>();
                var collection = db.GetCollection<Web3Data>("WalletStorage");

                // Check if an entry with the same playerId already exists
                var fil = Builders<Web3Data>.Filter.Eq("playerId", _playerId);
                var existingEntry = await collection.Find(fil).FirstOrDefaultAsync();

                if (existingEntry != null)
                {
                    Debug.LogWarning($"Entry for playerId {_playerId} already exists. Skipping creation.");
                    return;
                }

                // create new data within collection
                collection.InsertOne(new Web3Data()
                {
                    playerId = _playerId,
                    nftOwned = _nftOwned
                });
                Debug.Log($"Added ID:{_playerId} data to wallet database");

            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }


        }

        [ClientCallable]
        public async void UpdateNftOwned(string _playerId, bool _nftOwned)
        {
            try
            {
                var db = await Storage.GetDatabase<WalletStorage>();
                var collection = db.GetCollection<Web3Data>("WalletStorage");
                var filter = Builders<Web3Data>.Filter.Eq("playerId", _playerId);
                var update = Builders<Web3Data>.Update.Set("nftOwned", _nftOwned);

                // Update nftOwned bool
                collection.UpdateOne(filter, update);
            }
            catch (Exception e)
            {

                Debug.LogError(e.Message);
            }

        }

        [ClientCallable]
        public async Promise<string> GetEntryByPlayerId(string playerId)
        {
            // Filter entry by playerId
            var filter = Builders<Web3Data>.Filter.Eq("playerId", playerId);
            var db = await Storage.GetDatabase<WalletStorage>();
            var collection = db.GetCollection<Web3Data>("WalletStorage");
            var Web3DataEntry = collection
               .Find(filter)
               .FirstOrDefault();

            Debug.Log($"Web3DataEntry: {Web3DataEntry}");

            // Convert the entry to a JSON string
            var jsonEntry = JsonUtility.ToJson(Web3DataEntry);

            Debug.Log($"Entry: {jsonEntry}");

            return jsonEntry;
        }

        #endregion
    }
}
