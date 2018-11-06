using System;

namespace MongoIce.Core.Attributes
{
	public class IndexDescriptor : Attribute
	{
		/// <summary>
		/// Key of the index
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Type of index
		/// </summary>
		public IndexType Type { get; set; }

		/// <summary>
		/// Unique index
		/// </summary>
		public bool Unique { get; set; }

		/// <summary>
		/// Sparse index
		/// </summary>
		public bool Sparse { get; set; }
		/// <summary>
		/// Background index
		/// </summary>
		public bool Background { get; set; }

		public IndexDescriptor(string key, IndexType type = IndexType.Ascending, bool unique = false, bool sparse = false, bool background = false)
		{
			this.Key = key;
			this.Type = type;
			this.Unique = unique;
			this.Sparse = sparse;
			this.Background = background;
		}
	}
}
