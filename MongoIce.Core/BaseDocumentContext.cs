using MongoDB.Driver;
using MongoIce.Core.Attributes;
using System;
using System.Linq;

namespace MongoIce.Core
{
	public abstract class BaseDocumentContext : IDisposable
	{
		/// <summary>
		/// Low level database
		/// </summary>
		public IDocumentDatabase Database { get; }

		/// <summary>
		/// Autoconnect after creation
		/// </summary>
		protected bool Autoconnect = true;

		protected BaseDocumentContext(string serverUrl, string databaseName, bool autoconnect = true)
		{
			this.Database = new MongoDatabase(serverUrl, databaseName, this, null);

			if (autoconnect)
			{
				this.Database.Connect();
			}
		}

		/// <summary>
		/// Create indexes
		/// </summary>
		public abstract void CreateIndexes();

		/// <summary>
		/// Get a collection by generic
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IMongoCollection<T> GetCollectionByGeneric<T>()
		{
			var collectionProperties = this.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(CollectionDescriptor))).ToList();

			foreach (var collectionProperty in collectionProperties)
			{
				Type entityType = Type.GetType(collectionProperty.PropertyType.GenericTypeArguments[0].FullName + "," + collectionProperty.Module.Assembly.FullName);

				if (entityType == typeof(T))
				{
					return collectionProperty.GetValue(this) as IMongoCollection<T>;
				}
			}

			return null;
		}

		#region IDisposable

		public void Dispose()
		{

		}

		#endregion
	}
}