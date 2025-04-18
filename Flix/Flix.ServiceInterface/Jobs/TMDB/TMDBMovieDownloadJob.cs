using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.JobData;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Flix.ServiceInterface.Jobs.TMDB;

public class TMDBMovieDownloadJob(IMovieStore movieStore, TMDBMovieDownloader downloader, ILogger<TMDBMovieDownloadJob> logger) : IJob
{
	private readonly IMovieStore _movieStore = movieStore;
	private readonly TMDBMovieDownloader _downloader = downloader;
	private readonly ILogger<TMDBMovieDownloadJob> _logger = logger;

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			var movie = await _downloader.DownloadAsync(context.JobDetail.JobDataMap.GetString(DownloadJobParameters.EntityId.ToString()));

			if (movie != null)
			{
				if (!movie.IsProcessed) _logger.LogWarning("Movie {Title} is not processed fully after download.", movie.Title);

				await _movieStore.UpdateMovieByProviderIdAsync(movie, Provider.TMDB);
			}
			else
			{
				throw new Exception("Movie not found or error response from TMDB.");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error downloading movie from TMDB: {Message}", ex.Message);
			throw new Exception("Error downloading movie from TMDB.", ex);
		}
	}
}