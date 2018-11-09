using MongoDB.Driver;
using System;

namespace MongoIce.Core.Attributes
{
	public class CollectionDescriptor : Attribute
	{
		/// <summary>
		/// Type of the collection
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Get max documents of the collection
		/// </summary>
		public long MaxDocuments { get; }

		/// <summary>
		/// Get max size of the collection
		/// </summary>
		public long MaxSize { get; }

		/// <summary>
		/// Get the validation action of the collection
		/// </summary>
		public DocumentValidationAction ValidationAction { get; }

		/// <summary>
		/// Get the validation level of the collection
		/// </summary>
		public DocumentValidationLevel ValidationLevel { get; }

		/// <summary>
		/// Get if is a capped collection
		/// </summary>
		public bool Capped { get; }

		public CollectionDescriptor(
			Type type,
			bool capped = false,
			long maxDocuments = 0,
			long maxSize = 0,
			DocumentValidationAction validationAction = DocumentValidationAction.Error,
			DocumentValidationLevel validationLevel = DocumentValidationLevel.Off)
		{
			this.Type = type;
			this.Capped = capped;
			this.MaxDocuments = maxDocuments;
			this.MaxSize = maxSize;
			this.ValidationAction = validationAction;
			this.ValidationLevel = validationLevel;
		}
	}
}