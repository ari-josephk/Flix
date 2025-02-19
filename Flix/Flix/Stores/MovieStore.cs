using Flix.Settings;
using Flix.Stores.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Flix.Stores;

public class MovieStore : MongoStore<Movie>, IMovieStore
{
    public MovieStore(IOptions<FlixDatabaseSettings> dbSettings) : base(dbSettings)
    {
    }

    public Task AddMovieAsync(Movie movie)
    {
		return _collection.InsertOneAsync(movie);
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
		var movies = await _collection.Find(_ => true).ToListAsync();
		return movies;
    }

    public Task<Movie> GetMovieByIdAsync(int id)
    {
		return _collection.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public Task UpdateMovieAsync(Movie movie)
    {
		var filter = Builders<Movie>.Filter.Eq(m => m.Id, movie.Id);
		return _collection.ReplaceOneAsync(filter, movie);
    }
}