using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceInterface.Stores.ProviderMappings;
using MongoDB.Bson;

namespace Flix.ServiceInterface.Stores;

public interface IMovieStore
{
	Task<IEnumerable<Movie>> GetAllMoviesAsync();
	Task<Movie> GetMovieByIdAsync(ObjectId id);
	Task<Movie> GetMovieByProviderIdAsync(string providerId, Provider provider);
	Task AddMovieAsync(Movie movie);
	Task<bool> UpdateMovieAsync(Movie movie);
	Task<bool> UpdateMovieByProviderIdAsync(Movie movie, Provider provider);
}
