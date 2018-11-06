using System;

namespace MongoIce.Core
{
	public abstract class BaseMongoContext : IDisposable
	{
		/// <summary>
		/// Low level database
		/// </summary>
		public IDocumentDatabase Database { get; }

		/// <summary>
		/// Autoconnect after creation
		/// </summary>
		protected bool Autoconnect = true;

		protected BaseMongoContext(string serverUrl, string databaseName, bool autoconnect = true)
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

		#region IDisposable

		public void Dispose()
		{

		}

		#endregion
	}
}