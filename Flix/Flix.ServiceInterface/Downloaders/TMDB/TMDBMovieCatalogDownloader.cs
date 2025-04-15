using Flix.ServiceInterface.Downloaders.TMDB.Settings;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace Flix.ServiceInterface.Downloaders.TMDB;

public class TMDBMovieCatalogDownloader(IConfiguration config, IOptions<TMDBDownloaderSettings> options) : IDownloader<IEnumerable<Movie>>
{
	private readonly TMDbClient _client = new(config[options.Value.ApiKeyPath]);
	private readonly long _delay = options.Value.DownloadDelayMilliseconds;
	private readonly int _maxPagesToDownload = options.Value.MaxPagesToDownload;

	//Testing constructor
	public TMDBMovieCatalogDownloader(TMDbClient client) : this(null!, null!)
	{
		_client = client;
		_delay = 1000;
	}

	public async Task<IEnumerable<Movie>?> DownloadAsync(string? entityId)
	{
		await Task.Delay(TimeSpan.FromMilliseconds(_delay));

		var outMovies = new List<Movie>();

		var tmdbResponse = await _client.DiscoverMoviesAsync().OrderBy(TMDbLib.Objects.Discover.DiscoverMovieSortBy.PopularityDesc).Query();
		var totalPages = Math.Min(tmdbResponse.TotalPages, _maxPagesToDownload);
		var totalExpectedResults = tmdbResponse.TotalResults; // TODO: Log a check to make sure all downloaded

		for (int page = 0; page < totalPages; page++)
		{
			tmdbResponse = await _client.DiscoverMoviesAsync().OrderBy(TMDbLib.Objects.Discover.DiscoverMovieSortBy.PopularityDesc).Query(page: page);

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
		}

		return outMovies;
	}
}