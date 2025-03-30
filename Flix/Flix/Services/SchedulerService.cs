namespace Flix.Services;

using System.Collections;
using Quartz;
using Quartz.Impl;

public class SchedulerService
{
	private readonly IScheduler _scheduler;

	public SchedulerService()
	{
		var schedulerFactory = new StdSchedulerFactory();
		_scheduler = schedulerFactory.GetScheduler().Result;
	}

	public async Task StartAsync()
	{
		await _scheduler.Start();
	}

	public async Task ScheduleJob<TJob>(IDictionary jobDataMap, string cronExpression) where TJob : IJob
	{
		var job = JobBuilder.Create<TJob>()
			.WithIdentity(typeof(TJob).Name)
			.UsingJobData(new JobDataMap(jobDataMap))
			.Build();

		var trigger = TriggerBuilder.Create()
			.WithIdentity($"{typeof(TJob).Name}Trigger")
			.WithCronSchedule(cronExpression)
			.Build();

		await _scheduler.ScheduleJob(job, trigger);
	}

	public async Task ScheduleOneTimeJob<TJob>(IDictionary jobDataMap, TimeSpan? delay = null) where TJob : IJob
	{
		var job = JobBuilder.Create<TJob>()
			.WithIdentity(typeof(TJob).Name)
			.UsingJobData(new JobDataMap(jobDataMap))
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