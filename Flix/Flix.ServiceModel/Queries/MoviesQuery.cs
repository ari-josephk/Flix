using ServiceStack;
using Flix.ServiceModel.Models;

namespace Flix.ServiceModel.Queries;

[Route("/movies", "GET")]
public class MoviesQuery : IReturn<MoviesResponse>
{
}

public class MoviesResponse
{
	public List<Movie> Movies { get; set; }
}

