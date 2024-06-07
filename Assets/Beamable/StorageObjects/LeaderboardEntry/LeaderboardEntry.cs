using Beamable.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("LeaderboardEntry")]
	public class LeaderboardEntry : MongoStorageObject
	{
     
    }

    public class PlayerEntry
    {
        public ObjectId Id;
        public string playerName;
        public string petName;
        public int petRank;
        public int petXp;
    }

    public static class LeaderboardEntryExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for LeaderboardEntry
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> LeaderboardEntryDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<LeaderboardEntry>();

		/// <summary>
		/// Gets a MongoDB collection from LeaderboardEntry by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="LeaderboardEntryCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> LeaderboardEntryCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<LeaderboardEntry, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from LeaderboardEntry by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="LeaderboardEntryCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> LeaderboardEntryCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<LeaderboardEntry, TCollection>();
	}
}
