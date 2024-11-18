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
        public async void CreateEntry(string _playerId, string _walletAddress, bool _nftOwned)
        {

            try
            {
                // Declare or target existing "WalletStorage" collection
                var db = await Storage.GetDatabase<WalletStorage>();
                var collection = db.GetCollection<Web3Data>("Web3Data");

                // create new data within collection
                collection.InsertOne(new Web3Data()
                {
                    playerId = _playerId,
                    walletAddress = _walletAddress,
                    nftOwned = _nftOwned
                });
                Debug.Log($"Added ID:{_playerId} data added to database");

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
                var collection = db.GetCollection<Web3Data>("Web3Data");
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
            var collection = db.GetCollection<Web3Data>("Web3Data");
            var Web3DataEntry = await collection
               .Find(filter)
               .FirstOrDefaultAsync();

            // Convert the entry to a JSON string
            var jsonEntry = JsonUtility.ToJson(Web3DataEntry);

            Debug.Log($"THE storage ENTRY IS: {jsonEntry}");

            return jsonEntry;
        }

        #endregion
    }
}
