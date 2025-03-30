using Flix.Stores.Models;
using Flix.Stores.ProviderMappings;

namespace Flix.Stores;

public interface IMovieStore
{
	Task<IEnumerable<Movie>> GetAllMoviesAsync();
	Task<Movie> GetMovieByIdAsync(int id);
	Task<Movie> GetMovieByProviderIdAsync(string providerId, Provider provider);
	Task AddMovieAsync(Movie movie);
	Task<bool> UpdateMovieAsync(Movie movie);
}
