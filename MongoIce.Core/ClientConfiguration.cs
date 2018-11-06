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

		/// <summary>
		/// Heartbeat interval
		/// </summary>
		public TimeSpan HeartbeatInterval { get; set; }

		/// <summary>
		/// Heartbeat timeout
		/// </summary>
		public TimeSpan HeartbeatTimeout { get; set; }
	}
}
