using Flix.ServiceInterface.Downloaders.TMDB.Settings;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ServiceStack;
using TMDbLib.Client;

namespace Flix.ServiceInterface.Downloaders.TMDB;

public class TMDBMovieDownloader: IDownloader<Movie>
{
	private readonly TMDbClient _client;

	public TMDBMovieDownloader(IConfiguration config, IOptions<TMDBDownloaderSettings> options)
	{
		_client = new TMDbClient(config[options.Value.ApiKeyPath]);
	}

	//Testing constructor
	public TMDBMovieDownloader(TMDbClient client)
	{
		_client = client;
	}

	public virtual async Task<Movie?> DownloadAsync(string? tmdbId)
	{
		if (string.IsNullOrEmpty(tmdbId))
		{
			return null;
		}

		if (!int.TryParse(tmdbId, out int tmdbIdInt))
		{
			throw new ArgumentException($"Invalid TMDB ID: {tmdbId}");
		}

		var tmdbMovie = await _client.GetMovieAsync(tmdbIdInt);
		tmdbMovie.Credits = await _client.GetMovieCreditsAsync(tmdbIdInt);
		tmdbMovie.Images = await _client.GetMovieImagesAsync(tmdbIdInt);
		tmdbMovie.Videos = await _client.GetMovieVideosAsync(tmdbIdInt);
		
		return tmdbMovie != null ? new Movie
		{
			Title = tmdbMovie.Title,
			CoverImage = tmdbMovie.PosterPath,
			Director = tmdbMovie.Credits?.Crew.FirstOrDefault(c => c.Job == "Director")?.Name ?? "Unknown",
			Genre = tmdbMovie.Genres.Select(g => g.Name).Join(", "),
			RunTime = tmdbMovie.Runtime,
			ReleaseYear = tmdbMovie.ReleaseDate?.Year ?? 1948,
			ProviderIds = new() { { Provider.TMDB, tmdbId.ToString() } },
			Media = tmdbMovie.Images?.Posters.Select(p=> p.FilePath).ToList() ?? [],
			Actors = tmdbMovie.Credits?.Cast.Select(c => c.Name).ToList() ?? [],
			Trailer = tmdbMovie.Videos?.Results.FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer")?.Key,
			IsProcessed = true,
		} : null;
	}
}