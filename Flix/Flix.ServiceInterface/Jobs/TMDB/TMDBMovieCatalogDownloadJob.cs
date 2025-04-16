using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Flix.ServiceInterface.Jobs.TMDB;

public class TMDBMovieCatalogDownloadJob : IJob
{
	private readonly IMovieStore _movieStore;
	private readonly TMDBMovieCatalogDownloader _downloader;
	private readonly ILogger<TMDBMovieCatalogDownloadJob> _logger;

	public TMDBMovieCatalogDownloadJob(IMovieStore movieStore, TMDBMovieCatalogDownloader downloader, ILogger<TMDBMovieCatalogDownloadJob> logger)
	{
		_movieStore = movieStore;
		_downloader = downloader;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			_logger.LogInformation("TMDB Movie Catalog Download Job started.");
			var movies = await _downloader.DownloadAsync(null);

			if (movies != null && movies.Any())
			{
				foreach (var movie in movies)
				{
					await _movieStore.UpdateMovieByProviderIdAsync(movie, Provider.TMDB);
				}
			}
			else
			{
				throw new Exception("No movies found or error response from TMDB.");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error downloading movie catalog from TMDB: {Message}", ex.Message);
			throw new Exception("Error downloading movie catalog from TMDB.", ex);
		}
	}
}