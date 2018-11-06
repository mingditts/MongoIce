using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoIce.Core;
using MongoIce.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoIce.Test
{
	#region Mock DataModel

	public class Comment
	{
		/// <summary>
		/// Id of the entity
		/// </summary>
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		/// <summary>
		/// Timestamp of the entity
		/// </summary>
		public long Timestamp { get; set; }

		/// <summary>
		/// The name of the author
		/// </summary>
		public string AuthorName { get; set; }

		/// <summary>
		/// Content of the entity
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// Comments
		/// </summary>
		public IList<Comment> Comments { get; set; }
	}

	public class MongoContext : BaseMongoContext
	{
		/// <summary>
		/// Reports collection
		/// </summary>
		[CollectionDescriptor("Comments", typeof(Comment))]
		[IndexDescriptor("Timestamp", IndexType.Ascending, true, false, false)]
		public IMongoCollection<Comment> Comments { get; set; }

		public MongoContext(string serverUrl, string databaseName) : base(serverUrl, databaseName)
		{

		}

		public override void CreateIndexes()
		{
			base.Database.CreateIndex<Comment>(this.Comments);      //create indexes if you want. This method will be called in migration by the internal db object
		}
	}

	#endregion

	[TestClass]
	public class BasicTest
	{
		private string DatabaseUrl = "mongodb://localhost:27017";
		private string DatabaseName = "MongoIceTestDatabase";

		[TestInitialize()]
		public void Initialize()
		{

		}

		[TestMethod]
		public void TestCreation()
		{
			using (var context = new MongoContext(DatabaseUrl, DatabaseName))
			{
				context.Comments.DeleteMany(x => x.Id != null);

				var count = context.Comments.CountDocuments(new BsonDocument());

				Assert.AreEqual(count, 0);

				var comment = new Comment
				{
					//Id = ObjectId.GenerateNewId(),
					Content = "The content of the comment 1",
					Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
					AuthorName = "John Doe"
				};

				context.Comments.InsertOneAsync(comment, null).Wait();

				context.Comments.InsertOneAsync(new Comment
				{
					Id = ObjectId.GenerateNewId().ToString(),
					Content = "The content of the comment 2",
					Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 100,
					AuthorName = "John Doe"
				}, null).Wait();

				count = context.Comments.CountDocuments(new BsonDocument());

				Assert.AreEqual(count, 2);

				Comment lastComment = context.Comments.AsQueryable().Where(x => x.Id != null).FirstOrDefault();

				Assert.AreEqual(lastComment.Content, "The content of the comment 1");
			}
		}

		[TestMethod]
		public void TestNestedDocuments()
		{
			using (var context = new MongoContext(DatabaseUrl, DatabaseName))
			{
				context.Comments.DeleteMany(x => x.Id != null);

				var count = context.Comments.CountDocuments(new BsonDocument());

				Assert.AreEqual(count, 0);

				var subComments = new List<Comment>
				{
					new Comment {
						//Id = new Random().Next(),
						Content = "The content of the sub-comment",
						Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
						AuthorName = "John Doe",
						Comments = null
					}
				};

				context.Comments.InsertOneAsync(new Comment
				{
					//Id = new Random().Next(),
					Content = "The content of the comment",
					Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
					AuthorName = "John Doe",
					Comments = subComments
				}, null).Wait();

				count = context.Comments.CountDocuments(new BsonDocument());

				Assert.AreEqual(count, 1);

				var firstComment = context.Comments.AsQueryable().FirstOrDefault();

				Assert.AreNotEqual(firstComment, null);
				Assert.AreEqual(firstComment.Comments.Count, 1);
				Assert.AreNotEqual(firstComment.Comments[0], null);
				AssertHelper.AreEquals(firstComment.Comments[0], subComments[0]);
			}
		}
	}
}