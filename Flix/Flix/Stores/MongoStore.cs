using Flix.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flix.Stores;

public abstract class MongoStore<T>
{
	internal readonly IMongoCollection<T> _collection;

	public MongoStore(IOptions<FlixDatabaseSettings> dbSettings)
	{
		var client = new MongoClient(dbSettings.Value.ConnectionString);
		var database = client.GetDatabase(dbSettings.Value.DatabaseName);
		_collection = database.GetCollection<T>(dbSettings.Value.MoviesCollectionName);
	}
}
