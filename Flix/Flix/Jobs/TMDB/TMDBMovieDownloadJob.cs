using Flix.Downloaders;
using Flix.Downloaders.TMDB;
using Flix.JobData;
using Flix.Stores;
using Flix.Stores.Models;
using Quartz;

namespace Flix.Jobs.TMDB;

public class TMDBMovieDownloadJob : IJob
{
	private readonly IMovieStore _movieStore;
	private readonly TMDBMovieDownloader _downloader;
	private readonly ILogger<TMDBMovieDownloadJob> _logger;

	public TMDBMovieDownloadJob(IMovieStore movieStore, TMDBMovieDownloader downloader, ILogger<TMDBMovieDownloadJob> logger)
	{
		_movieStore = movieStore;
		_downloader = downloader;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var movie = await _downloader.DownloadAsync(context.JobDetail.JobDataMap.GetString(DownloadJobParameters.EntityId.ToString()));

		if (movie != null && !movie.IsErrorResponse()) 
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