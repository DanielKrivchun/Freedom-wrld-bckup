using Beamable.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("ExtraPlayerDataStorage")]
	public class ExtraPlayerDataStorage : MongoStorageObject
	{
	}

    public class ExtraPlayerData 
    {
        public ObjectId Id;
        public string playerId;
        public int introTutorial;
        public int eatTutorial;
        public int showerTutorial;
        public int inventoryTutorial;
		public string vitaminsAteTime;
        public string cosmicBerryElectrolyteBoughtTime;
        public string miracleCognitiveBoughtTime;
        public string proteinShakeBoughtTime;
    }

    public static class ExtraPlayerDataStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for ExtraPlayerDataStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> ExtraPlayerDataStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<ExtraPlayerDataStorage>();

		/// <summary>
		/// Gets a MongoDB collection from ExtraPlayerDataStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="ExtraPlayerDataStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> ExtraPlayerDataStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<ExtraPlayerDataStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from ExtraPlayerDataStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="ExtraPlayerDataStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> ExtraPlayerDataStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<ExtraPlayerDataStorage, TCollection>();
	}
}
