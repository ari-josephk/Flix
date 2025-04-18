using Flix.ServiceInterface.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flix.ServiceInterface.Stores;

public abstract class MongoStore<T>
{
	internal readonly IMongoCollection<T> _collection;

	public MongoStore(IOptions<FlixDatabaseSettings> dbSettings, string collectionName)
	{
		var client = new MongoClient(dbSettings.Value.ConnectionString);
		var database = client.GetDatabase(dbSettings.Value.DatabaseName);

		_collection = database.GetCollection<T>(collectionName);
	}
}
