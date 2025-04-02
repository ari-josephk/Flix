using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.JobData;
using Flix.ServiceInterface.Stores;
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
		var movie = await _downloader.DownloadAsync(context.JobDetail.JobDataMap.GetString(DownloadJobParameters.EntityId.ToString()));

		if (movie != null) 
		{
			if (!movie.IsProcessed) _logger.LogWarning("Movie {Title} is not processed fully after download.", movie.Title);

			await _movieStore.UpdateMovieAsync(movie);
		}
		else
		{
			throw new Exception("Movie not found or error response from TMDB.");
		} 
	}
}