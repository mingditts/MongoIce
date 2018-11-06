using MongoDB.Driver;

namespace MongoIce.Core
{
	public interface IDocumentDatabase
	{
		/// <summary>
		/// Connect
		/// </summary>
		void Connect();

		/// <summary>
		/// Disconnect
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Create collection
		/// </summary>
		/// <param name="name"></param>
		void CreateCollection(string name);

		/// <summary>
		/// Drop collection
		/// </summary>
		/// <param name="name"></param>
		void DropCollection(string name);

		/// <summary>
		/// Drop all collections
		/// </summary>
		void DropAllCollections();

		/// <summary>
		/// Create index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		void CreateIndex<T>(IMongoCollection<T> collection);
	}
}
