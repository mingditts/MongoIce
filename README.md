# MongoIce
An entity framework-like library for MongoDB

## Features

This library allows to create collections and indexes automatically based on a context decorated with some entity framework-like attributes.
It allows also some additional helpers like collections cleanup.
It's work in progress!

## Usage

Model definition and description:

```c#
public class Comment
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }

  public long Timestamp { get; set; }

  public string AuthorName { get; set; }

  public string Content { get; set; }
}

public class MongoContext : BaseDocumentContext
{
  /// <summary>
  /// Comment collection
  /// </summary>
  [CollectionDescriptor("Comments", typeof(Comment))]
  [IndexDescriptor("Timestamp", IndexType.Ascending, true, false, false)]
  public IMongoCollection<Comment> Comments { get; set; }

  public MongoContext(string serverUrl, string databaseName) : base(serverUrl, databaseName)
  {

  }

  public override void CreateIndexes()
  {
    base.Database.CreateIndex<Comment>(this.Comments);
  }
}
```

Context usage:

```c#
using (var context = new MongoContext("mongodb://localhost:27017", "MongoIceTestDatabase"))
{
  // perform all collection specific operation using linq and mongodb features
  
  context.Comments.DeleteMany(x => x.Id != null);
}
```
