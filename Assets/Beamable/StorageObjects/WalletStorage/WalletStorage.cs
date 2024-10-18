using Beamable.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("WalletStorage")]
	public class WalletStorage : MongoStorageObject
	{
	}

    public class Web3Data
    {
        public ObjectId Id;
        public string playerId;
        public string walletAddress;
        public bool nftOwned;
    }

    public static class WalletStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for WalletStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> WalletStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<WalletStorage>();

		/// <summary>
		/// Gets a MongoDB collection from WalletStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="WalletStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> WalletStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<WalletStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from WalletStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="WalletStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> WalletStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<WalletStorage, TCollection>();
	}
}
