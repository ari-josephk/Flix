
using ServiceStack;
using Flix.ServiceModel.Queries;
using Flix.ServiceInterface.Services;

namespace Flix.ServiceInterface.QueryHandlers;
public class ScheduleQueryHandler(SchedulerService scheduler) : Service
{
	private readonly SchedulerService _scheduler = scheduler;

	public async Task<ScheduleResponse> Get(ScheduleQuery query)
	{
		var jobs = await _scheduler.GetScheduledJobsWithRunTimesAsync();

		return new ScheduleResponse
		{
			Jobs = jobs.Select(job => (job.JobDetail.Key.Name, job.LastRunTime, job.NextRunTime))
		};
	}
}
