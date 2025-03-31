
using ServiceStack;
using Flix.ServiceModel.Queries;
using Flix.ServiceModel.Models;

namespace Flix.ServiceInterface.QueryHandlers;

public class MoviesQueryHandler : Service
{
	public MoviesResponse Get(MoviesQuery query)
	{
		return new MoviesResponse
		{
			Movies = new List<Movie>
			{
				new Movie { Id = 1, Title = "Inception", Genre = "Sci-Fi"},
				new Movie { Id = 2, Title = "The Matrix", Genre = "Action"},
				new Movie { Id = 3, Title = "Interstellar", Genre = "Sci-Fi"},
				new Movie { Id = 4, Title = "The Godfather", Genre = "Crime"},
				new Movie { Id = 5, Title = "Pulp Fiction", Genre = "Crime"}
			}
		};
	}
}

