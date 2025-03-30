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

	public async Task<Movie?> DownloadAsync(string? tmdbId)
	{
		if (string.IsNullOrEmpty(tmdbId))
		{
			return null;
		}

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