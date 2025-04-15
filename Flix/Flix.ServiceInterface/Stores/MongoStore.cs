using Flix.ServiceInterface.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flix.ServiceInterface.Stores;

public abstract class MongoStore<T>
{
	internal readonly IMongoCollection<T> _collection;

	public MongoStore(IOptions<FlixDatabaseSettings> dbSettings, string collectionName)
	{
		// var client = new MongoClient(dbSettings.Value.ConnectionString);
		// var database = client.GetDatabase(dbSettings.Value.DatabaseName);
		// _collection = database.GetCollection<T>(dbSettings.Value.MoviesCollectionName);

		var mongoClientSettings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
		mongoClientSettings.ClusterConfigurator = cb => { /* No-op for in-memory */ };// Use V2 for LINQ support
		var client = new MongoClient(mongoClientSettings);
		var database = client.GetDatabase("InMemoryDatabase");
		_collection = database.GetCollection<T>(collectionName);
	}
}
