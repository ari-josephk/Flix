using ServiceStack;
using Flix.ServiceModel.Models;

namespace Flix.ServiceModel.Queries;

[Route("/movie", "GET")]
public class MovieQuery : IReturn<MoviesResponse>
{
	public required string Id { get; set; }
}

public class MovieResponse
{
	public required Movie Movie { get; set; }
}
