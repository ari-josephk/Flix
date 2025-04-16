using Quartz;

namespace Flix.ServiceInterface.Services;
public interface ISchedulerService
{
	Task StartAsync();
	Task<IReadOnlyCollection<(IJobDetail JobDetail, DateTimeOffset? LastRunTime, DateTimeOffset? NextRunTime)>> GetScheduledJobsWithRunTimesAsync();
	Task ScheduleJob<TJob>(JobDataMap jobDataMap, TimeSpan repeatInterval, DateTimeOffset? startTime = null) where TJob : IJob;
	Task ScheduleOneTimeJob<TJob>(JobDataMap jobDataMap, TimeSpan? delay = null) where TJob : IJob;
}