using Flix.Downloaders;
using Flix.Downloaders.TMDB;
using Flix.JobData;
using Flix.Stores;
using Flix.Stores.Models;
using Flix.Stores.ProviderMappings;
using Quartz;

namespace Flix.Jobs.TMDB;

public class TMDBMovieCatalogDownloadJob : IJob
{
	private readonly IMovieStore _movieStore;
	private readonly TMDBMovieCatalogDownloader _downloader;
	private readonly ILogger<TMDBMovieDownloadJob> _logger;

	public TMDBMovieCatalogDownloadJob(IMovieStore movieStore, TMDBMovieCatalogDownloader downloader, ILogger<TMDBMovieDownloadJob> logger)
	{
		_movieStore = movieStore;
		_downloader = downloader;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		_logger.LogInformation("TMDB Movie Catalog Download Job started.");
		var movies = await _downloader.DownloadAsync(null);

		if (movies != null && movies.Any())
		{
			foreach (var movie in movies)
			{
				var existingMovie = await _movieStore.GetMovieByProviderIdAsync(movie.ProviderIds[Provider.TMDB], Provider.TMDB);

				if (existingMovie != null)
				{
					await _movieStore.UpdateMovieAsync(movie);
				}
				else
				{
					// Add new movie
					await _movieStore.AddMovieAsync(movie);
				}
			}
		}
		else
		{
			throw new Exception("No movies found or error response from TMDB.");
		}
	}
}