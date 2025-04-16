
using Flix.ServiceInterface.JobData;
using Flix.ServiceInterface.Jobs.TMDB;
using Flix.ServiceInterface.Services;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Flix.ServiceInterface.Jobs;

public class UniversalSchedulerJob(ISchedulerService schedulerService, ILogger<UniversalSchedulerJob> logger, IMovieStore movieStore) : IJob
{
	private readonly ILogger<UniversalSchedulerJob> _logger = logger;
	private readonly ISchedulerService _schedulerService = schedulerService;
	private readonly IMovieStore _movieStore = movieStore;

	private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

	public async Task Execute(IJobExecutionContext context)
	{
		var movies = await _movieStore.GetAllMoviesAsync();

		var moviesToProcess = movies.Where(m => !m.IsProcessed).ToList();

		//TODO: Make this configurable
		//Update TMDB Movie data where needed
		var tmdbMovieIdsToProcess = moviesToProcess
			.Where(m => m.ProviderIds.ContainsKey(Provider.TMDB) && m.ProviderIds[Provider.TMDB] != null && m.IsProcessed == false)
			.Select(m => m.ProviderIds[Provider.TMDB])
			.ToList();

		var movieNames = moviesToProcess
			.Where(m => m.ProviderIds.ContainsKey(Provider.TMDB) && m.ProviderIds[Provider.TMDB] != null && m.IsProcessed == false)
			.Select(m => m.Title)
			.ToList();

		for (int i = 0; i < tmdbMovieIdsToProcess.Count; i++)
		{
			var tmdbId = tmdbMovieIdsToProcess[i];
			var movieName = movieNames[i];

			var jobDataMap = new JobDataMap
			{
				{ DownloadJobParameters.EntityId.ToString(), tmdbId },
				{ DownloadJobParameters.JobIdentity.ToString(), movieName }
			};

			await _schedulerService.ScheduleOneTimeJob<TMDBMovieDownloadJob>(jobDataMap, Delay * i);
		}
	}
}
