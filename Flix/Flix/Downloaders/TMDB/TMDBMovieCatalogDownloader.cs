using Flix.Stores.Models;
using Flix.Stores.ProviderMappings;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace Flix.Downloaders.TMDB;

public class TMDBMovieCatalogDownloader : IDownloader<IEnumerable<Movie>>
{
	private static readonly string _apiKeyPath = "TMDB:ApiKey";
	private readonly TMDbClient _client;

	public TMDBMovieCatalogDownloader(IConfiguration config)
	{
		_client = new TMDbClient(config[_apiKeyPath]);
	}
	public async Task<IEnumerable<Movie>?> DownloadAsync(string? entityId)
	{
		var outMovies = new List<Movie>();
		
		var tmdbResponse = await _client.DiscoverMoviesAsync().OrderBy(TMDbLib.Objects.Discover.DiscoverMovieSortBy.PopularityDesc).Query();
		
		foreach (SearchMovie tmdbMovie in tmdbResponse.Results)
		{
			var movie = new Movie
			{
				Title = tmdbMovie.Title,
				CoverImage = tmdbMovie.PosterPath,
				ReleaseYear = tmdbMovie.ReleaseDate?.Year ?? 1948,
				ProviderIds = new() { { Provider.TMDB, tmdbMovie.Id.ToString() } }
			};
			
			outMovies.Add(movie);
		}

		return outMovies;
	}
}