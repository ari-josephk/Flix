using Flix.ServiceInterface.Jobs.TMDB;
using Flix.ServiceInterface.QueryHandlers;
using Flix.ServiceInterface.Services;
using Flix.ServiceModel.Queries;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;

namespace Flix.Tests.QueryHandlers;
public class ScheduleQueryTests
{
	private Mock<ISchedulerService> _scheduleMock;
	private ScheduleQueryHandler _handler;

	[SetUp]
	public void Setup()
	{
		_scheduleMock = new Mock<ISchedulerService>();
		_handler = new ScheduleQueryHandler(_scheduleMock.Object);
	}

	[Test]
	public async Task Get_ShouldReturnScheduledJobs_WhenJobsExist()
	{
		// Arrange
		var scheduledJobs = new List<(IJobDetail JobDetail, DateTimeOffset? LastRunTime, DateTimeOffset? NextRunTime)>
		{
			(
				new JobDetailImpl("job1", "group1", typeof(TMDBMovieDownloadJob)),
				DateTimeOffset.UtcNow.AddMinutes(-10),
				DateTimeOffset.UtcNow.AddMinutes(10)
			),
			(
				new JobDetailImpl("job2", "group2", typeof(TMDBMovieDownloadJob)),
				DateTimeOffset.UtcNow.AddMinutes(-5),
				DateTimeOffset.UtcNow.AddMinutes(5)
			)
		};

		_scheduleMock.Setup(s => s.GetScheduledJobsWithRunTimesAsync()).Returns(Task.FromResult<IReadOnlyCollection<(IJobDetail JobDetail, DateTimeOffset? LastRunTime, DateTimeOffset? NextRunTime)>>(
			scheduledJobs.Select(job => (job.JobDetail, job.LastRunTime, job.NextRunTime)).ToList()));

		var query = new ScheduleQuery();

		// Act
		var result = await _handler.Get(query);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Jobs.Count(), Is.EqualTo(2));

		var job1 = result.Jobs.FirstOrDefault(j => j.JobName == "job1");
		Assert.That(job1, Is.Not.Null);
		Assert.That(job1?.JobName, Is.EqualTo("job1"));

		var job2 = result.Jobs.FirstOrDefault(j => j.JobName == "job2");
		Assert.That(job2, Is.Not.Null);
		Assert.That(job2?.JobName, Is.EqualTo("job2"));

		Assert.That(job1?.NextRunTime.HasValue, Is.True);
		Assert.That(job1?.LastRunTime.HasValue, Is.True);
		Assert.That(job1?.NextRunTime > job1?.LastRunTime, Is.True);

		Assert.That(job2?.NextRunTime.HasValue, Is.True);
		Assert.That(job2?.LastRunTime.HasValue, Is.True);
		Assert.That(job2?.NextRunTime > job2?.LastRunTime, Is.True);
	}

	[TearDown]
	public void TearDown()
	{
		_handler?.Dispose();
	}
}
