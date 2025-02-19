using Flix.Stores.Models;

namespace Flix.Stores
{
	public interface IMovieStore
	{
		Task<IEnumerable<Movie>> GetAllMoviesAsync();
		Task<Movie> GetMovieByIdAsync(int id);
		Task AddMovieAsync(Movie movie);
		Task UpdateMovieAsync(Movie movie);
	}
}