
using ServiceStack;
using Flix.ServiceModel.Queries;
using Flix.ServiceModel.Models;
using Flix.ServiceInterface.Stores;

namespace Flix.ServiceInterface.QueryHandlers;

public class MoviesQueryHandler(IMovieStore movieStore) : Service
{
	IMovieStore _movieStore = movieStore;
	public async Task<MoviesResponse> Get(MoviesQuery query)
	{
		var serviceMovies = await _movieStore.GetAllMoviesAsync();

		return new MoviesResponse
		{
			Movies = serviceMovies.Select(m => new Movie
			{
				Id = m.Id.ToString(),
				Title = m.Title,
				Director = m.Director,
				ReleaseYear = m.ReleaseYear,
				Genre = m.Genre
			}).ToList()
		};
	}
}

