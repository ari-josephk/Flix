using System.Runtime.CompilerServices;
using Flix.Stores.Models;
using Flix.Stores.ProviderMappings;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace Flix.Downloaders.TMDB;

public class TMDBMovieDownloader : IDownloader<Movie>
{
	private static readonly string _apiKeyPath = "TMDB:ApiKey";
	
	private readonly TMDbClient _client;

	public TMDBMovieDownloader(IConfiguration config)
	{
		_client = new TMDbClient(config[_apiKeyPath]);
	}

	public async Task<IEnumerable<Movie>> DownloadMoviesAsync()
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

	public async Task<Movie?> DownloadMovieAsync(int tmdbId)
	{
		var tmdbMovie = await _client.GetMovieAsync(tmdbId);
		
		return tmdbMovie != null ? new Movie
		{
			Title = tmdbMovie.Title,
			CoverImage = tmdbMovie.PosterPath,
			Director = tmdbMovie.Credits.Crew.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Unknown",
			Genre = tmdbMovie.Genres.Join(", "),
			RunTime = tmdbMovie.Runtime,
			ReleaseYear = tmdbMovie.ReleaseDate?.Year ?? 1948,
			ProviderIds = new() { { Provider.TMDB, tmdbId.ToString() } },
			Media = tmdbMovie.Images.Posters.Select(p=> p.FilePath).ToList(),
			Actors = tmdbMovie.Credits.Cast.Select(c => c.Name).ToList(),
		} : null;
	}
}