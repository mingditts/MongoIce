using System;

namespace MongoIce.Core
{
	public class ClientConfiguration
	{
		/// <summary>
		/// Use ssl for the connection
		/// </summary>
		public bool UseSsl { get; set; }

		/// <summary>
		/// Connection timeout
		/// </summary>
		public TimeSpan ConnectTimeout { get; set; }
	}
}