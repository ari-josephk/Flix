using System.Collections;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Flix.ServiceInterface.Services;

public class SchedulerService
{
	private readonly IScheduler _scheduler;
	private readonly ILogger<SchedulerService> _logger;

	public SchedulerService(ISchedulerFactory schedulerFactory, ILogger<SchedulerService> logger)
	{
		_logger = logger;
		_scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
	}

	public async Task StartAsync()
	{
		await _scheduler.Start();
	}

	public async Task<IReadOnlyCollection<(IJobDetail JobDetail, DateTimeOffset? LastRunTime, DateTimeOffset? NextRunTime)>> GetScheduledJobsWithRunTimesAsync()
	{
		var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
		var jobsWithRunTimes = new List<(IJobDetail, DateTimeOffset?, DateTimeOffset?)>();

		foreach (var jobKey in jobKeys)
		{
			var jobDetail = await _scheduler.GetJobDetail(jobKey);
			if (jobDetail != null)
			{
				var triggers = await _scheduler.GetTriggersOfJob(jobKey);
				DateTimeOffset? lastRunTime = null;
				DateTimeOffset? nextRunTime = null;

				foreach (var trigger in triggers)
				{
					var triggerState = await _scheduler.GetTriggerState(trigger.Key);
					if (triggerState == TriggerState.Complete || triggerState == TriggerState.Normal)
					{
						lastRunTime = trigger.GetPreviousFireTimeUtc();
						nextRunTime = trigger.GetNextFireTimeUtc();
					}
				}

				jobsWithRunTimes.Add((jobDetail, lastRunTime, nextRunTime));
			}
		}

		return jobsWithRunTimes.AsReadOnly();
	}

	public async Task ScheduleJob<TJob>(JobDataMap jobDataMap, TimeSpan repeatInterval, DateTimeOffset? startTime = null) where TJob : IJob
	{
		_logger.LogInformation($"Scheduling job {typeof(TJob).Name} to start at {startTime} and repeat every {repeatInterval}");

		var job = JobBuilder.Create<TJob>()
			.WithIdentity(typeof(TJob).Name)
			.UsingJobData(jobDataMap)
			.Build();

		var triggerBuilder = TriggerBuilder.Create()
			.WithIdentity($"{typeof(TJob).Name}Trigger")
			.WithSimpleSchedule(x => x.WithInterval(repeatInterval).RepeatForever());

		if (startTime.HasValue)
		{
			triggerBuilder.StartAt(startTime.Value);
		}
		else
		{
			triggerBuilder.StartNow();
		}

		var trigger = triggerBuilder.Build();

		await _scheduler.ScheduleJob(job, trigger);
	}

	public async Task ScheduleOneTimeJob<TJob>(JobDataMap jobDataMap, TimeSpan? delay = null) where TJob : IJob
	{
		var job = JobBuilder.Create<TJob>()
			.WithIdentity(typeof(TJob).Name)
			.UsingJobData(jobDataMap)
			.Build();

		var triggerBuilder = TriggerBuilder.Create()
			.WithIdentity($"{typeof(TJob).Name}OneTimeTrigger")
			.StartNow();

		if (delay.HasValue)
		{
			triggerBuilder.StartAt(DateTimeOffset.Now.Add(delay.Value));
		}

		var trigger = triggerBuilder.Build();

		await _scheduler.ScheduleJob(job, trigger);
	}

	public async Task StopAsync()
	{
		await _scheduler.Shutdown();
	}
}