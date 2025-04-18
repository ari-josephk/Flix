
using ServiceStack;
using Flix.ServiceModel.Queries;
using Flix.ServiceModel.Models;
using Flix.ServiceInterface.Stores;

namespace Flix.ServiceInterface.QueryHandlers;

public class MovieQueryHandler(IMovieStore movieStore) : Service
{
	IMovieStore _movieStore = movieStore;
	public async Task<MovieResponse> Get(MovieQuery query)
	{
		var objectId = MongoDB.Bson.ObjectId.Parse(query.Id);
		var serviceMovie = await _movieStore.GetMovieByIdAsync(objectId);

		if (serviceMovie == null)
		{
			throw HttpError.NotFound($"Movie with ID {query.Id} not found.");
		}

		return new MovieResponse
		{
			Movie = new Movie
			{
				Id = serviceMovie.Id.ToString(),
				Title = serviceMovie.Title,
				Director = serviceMovie.Director,
				ReleaseYear = serviceMovie.ReleaseYear,
				Genre = serviceMovie.Genre
			}
		};
	}
}

