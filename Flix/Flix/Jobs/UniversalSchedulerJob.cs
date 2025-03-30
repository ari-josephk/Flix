using System;
using System.Threading.Tasks;
using Flix.Jobs.TMDB;
using Flix.Services;
using Flix.Stores;
using Flix.Stores.Models;
using Flix.Stores.ProviderMappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Flix.Jobs;

public class UniversalSchedulerJob : IJob
{
	private readonly ILogger<UniversalSchedulerJob> _logger;
	private readonly SchedulerService _schedulerService;
	private readonly IMovieStore _movieStore;

	private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

	public UniversalSchedulerJob(SchedulerService schedulerService, ILogger<UniversalSchedulerJob> logger, IMovieStore movieStore)
	{
		_schedulerService = schedulerService;
		_logger = logger;
		_movieStore = movieStore;
	}

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

		for (int i = 0; i < tmdbMovieIdsToProcess.Count; i++)
		{
			var tmdbId = tmdbMovieIdsToProcess[i];

			var jobDataMap = new JobDataMap
			{
				{ "tmdbId", tmdbId },
				{ "movieStore", _movieStore }
			};

			await _schedulerService.ScheduleOneTimeJob<TMDBMovieDownloadJob>(jobDataMap, Delay * i);
		}
	}
}
