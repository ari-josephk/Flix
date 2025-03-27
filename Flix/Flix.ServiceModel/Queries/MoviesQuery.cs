using System.Collections.Generic;
using Flix.ServiceModel.Models;
using ServiceStack;

namespace Flix.ServiceModel.Queries
{
	[Route("/movies", "GET")]
	public class MoviesQuery : IReturn<MoviesResponse>
	{
	}

	public class MoviesResponse
	{
		public List<Movie> Movies { get; set; }
	}
}