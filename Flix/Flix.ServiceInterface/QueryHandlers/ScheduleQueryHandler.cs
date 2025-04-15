
using ServiceStack;
using Flix.ServiceModel.Queries;
using Flix.ServiceInterface.Services;
using Flix.ServiceModel.Models;

namespace Flix.ServiceInterface.QueryHandlers;
public class ScheduleQueryHandler(SchedulerService scheduler) : Service
{
	private readonly SchedulerService _scheduler = scheduler;

	public async Task<ScheduleResponse> Get(ScheduleQuery query)
	{
		var jobs = await _scheduler.GetScheduledJobsWithRunTimesAsync();

		return new ScheduleResponse
		{
			Jobs = [.. jobs.Select(job => new ScheduleJob {
				JobName = job.JobDetail.Key.Name,
				LastRunTime = job.LastRunTime,
				NextRunTime = job.NextRunTime
			})]
		};
	}
}
