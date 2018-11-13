using MongoDB.Bson;
using MongoDB.Driver;
using MongoIce.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoIce.Core
{
	public class MongoDatabase : IDocumentDatabase, IDisposable
	{
		private string _serverUrl;
		private MongoClient _client;

		private string _databaseName;
		private ClientConfiguration _clientConfiguration;
		private IMongoDatabase _database;

		private BaseDocumentContext _context;

		public MongoDatabase(string serverUrl, string databaseName, BaseDocumentContext context, ClientConfiguration clientConfiguration)
		{
			this._serverUrl = serverUrl;
			this._databaseName = databaseName;
			this._clientConfiguration = clientConfiguration;

			if (this._clientConfiguration == null)
			{
				this._clientConfiguration = new ClientConfiguration
				{
					UseSsl = true,
					ConnectTimeout = TimeSpan.FromMilliseconds(30000)
				};
			}

			this._context = context;
		}

		/// <summary>
		/// Collection exists
		/// </summary>
		/// <param name="collectionName"></param>
		/// <returns></returns>
		private bool CollectionExists(string collectionName)
		{
			var filter = new BsonDocument("name", collectionName);
			var options = new ListCollectionNamesOptions { Filter = filter };
			return this._database.ListCollectionNames(options).Any();
		}

		/// <summary>
		/// Migrate collection
		/// </summary>
		/// <param name="name"></param>
		/// <param name="collectionPropertyType"></param>
		/// <param name="descriptor"></param>
		private void MigrateCollection(string name, Type collectionPropertyType, CollectionDescriptor descriptor)
		{
			if (!this.CollectionExists(name))
			{
				this._database.CreateCollection(name, new CreateCollectionOptions
				{
					MaxDocuments = descriptor.MaxDocuments > 0 ? descriptor.MaxDocuments : (long?)null,
					Capped = descriptor.Capped,
					MaxSize = descriptor.MaxSize > 0 ? descriptor.MaxSize : (long?)null,
					ValidationAction = descriptor.ValidationAction,
					ValidationLevel = descriptor.ValidationLevel
				});
			}

			var typedCollection = typeof(MongoDatabase)
				.GetMethod("GetCollectionByName", BindingFlags.NonPublic | BindingFlags.Instance)
				.MakeGenericMethod(new Type[] { collectionPropertyType })
				.Invoke(this, new object[] { name });

			this._context.GetType().GetProperty(name).SetValue(this._context, typedCollection);
		}

		/// <summary>
		/// Database migration
		/// </summary>
		private void Migrate()
		{
			var properties = this._context.GetType().GetProperties();

			var collectionProperties = properties.Where(x => Attribute.IsDefined(x, typeof(CollectionDescriptor))).ToList();

			foreach (var collectionProperty in collectionProperties)
			{
				CollectionDescriptor descriptor = collectionProperty.GetCustomAttributes(typeof(CollectionDescriptor), false).FirstOrDefault() as CollectionDescriptor;

				Type entityType = Type.GetType(collectionProperty.PropertyType.GenericTypeArguments[0].FullName + "," + collectionProperty.Module.Assembly.FullName);

				this.MigrateCollection(collectionProperty.Name, entityType, descriptor);
			}

			this._context.CreateIndexes();
		}

		/// <summary>
		/// Get collection by name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		private IMongoCollection<T> GetCollectionByName<T>(string name)
		{
			return this._database.GetCollection<T>(name);
		}

		#region IDocumentDatabase

		public void Connect()
		{
			MongoClientSettings clientSettings = MongoClientSettings.FromUrl(MongoUrl.Create(this._serverUrl));

			clientSettings.UseSsl = this._clientConfiguration.UseSsl;
			clientSettings.ConnectTimeout = this._clientConfiguration.ConnectTimeout;

			this._client = new MongoClient(clientSettings);

			this._database = this._client.GetDatabase(this._databaseName);

			this.Migrate();
		}

		public void Disconnect()
		{
			throw new NotImplementedException();
		}

		public void CreateCollection(string name)
		{
			this._database.CreateCollection(name);
		}

		public void DropCollection(string name)
		{
			this._database.DropCollection(name);
		}

		public void DropAllCollections()
		{
			foreach (BsonDocument collection in this._database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
			{
				string name = collection["name"].AsString;
				this.DropCollection(name);
			}
		}

		public void CreateIndex<T>(IMongoCollection<T> collection)
		{
			var properties = this._context.GetType().GetProperties();

			IList<PropertyInfo> collectionProperties = properties.Where(x => Attribute.IsDefined(x, typeof(CollectionDescriptor))).ToList();

			if (collectionProperties == null)
			{
				return;
			}

			PropertyInfo collectionProperty = collectionProperties.Where(x => x.Name.Equals(collection.CollectionNamespace.CollectionName)).FirstOrDefault();

			if (collectionProperty == null)
			{
				return;
			}

			IndexDescriptor indexDescriptor = collectionProperty.GetCustomAttributes(typeof(IndexDescriptor), false).FirstOrDefault() as IndexDescriptor;

			if (indexDescriptor == null || string.IsNullOrEmpty(indexDescriptor.Key))
			{
				return;
			}

			IndexKeysDefinition<T> index = null;

			FieldDefinition<T> indexedColumn = indexDescriptor.Key;

			switch (indexDescriptor.Type)
			{
				case IndexType.Ascending:
					index = new IndexKeysDefinitionBuilder<T>().Ascending(indexedColumn);
					break;

				case IndexType.Descending:
					index = new IndexKeysDefinitionBuilder<T>().Descending(indexedColumn);
					break;

				case IndexType.Geo2D:
					index = new IndexKeysDefinitionBuilder<T>().Geo2D(indexedColumn);
					break;

				case IndexType.Geo2DSphere:
					index = new IndexKeysDefinitionBuilder<T>().Geo2DSphere(indexedColumn);
					break;

				case IndexType.Hashed:
					index = new IndexKeysDefinitionBuilder<T>().Hashed(indexedColumn);
					break;

				case IndexType.Text:
					index = new IndexKeysDefinitionBuilder<T>().Text(indexedColumn);
					break;

				default:
					return;
			}

			if (index == null)
			{
				return;
			}

			var indexModel = new CreateIndexModel<T>(index, new CreateIndexOptions() { Background = indexDescriptor.Background, Sparse = indexDescriptor.Sparse, Unique = indexDescriptor.Unique });

			collection.Indexes.CreateOne(indexModel);
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{

		}

		#endregion
	}
}