using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Beamable.Common;
using Beamable.Server;
using MongoDB.Driver;
using UnityEngine;
using static Beamable.Server.LeaderboardEntry;

namespace Beamable.Microservices
{
	[Microservice("GameServer")]
	public class GameServer : Microservice
	{
		[ClientCallable]
		public async Promise<bool> SaveEntry(string playerName, string petName, int petRank, int petXp) 
		{
            bool isSuccess = false;

			try
			{
                var db = await Storage.GetDatabase<LeaderboardEntry>();
                var collection = db.GetCollection<PlayerEntry>("LeaderboardEntries");
                collection.InsertOne(new PlayerEntry()
                {
                    playerName = playerName,
                    petName = petName,
                    petRank = petRank,
                    petXp = petXp

                });
            } 
            catch (Exception e) {

                UnityEngine.Debug.LogError(e.Message);
            }

            return isSuccess;
         
        }

        [ClientCallable]
        public async Promise<List<string>> GetEntry(string playerName, string petName, int petRank, int petXp)
        {
            var db = await Storage.GetDatabase<LeaderboardEntry>();
            var collection = db.GetCollection<PlayerEntry>("LeaderboardEntries");
            var LeaderboardEntries = collection
               .Find(data => data.playerName == playerName && data.petName == petName && data.petRank == petRank && data.petXp == petXp)
               .ToList();

            return LeaderboardEntries.Select(entry => entry.playerName + entry.petName + entry.petRank + entry.petXp).ToList();
        }

    }
}
