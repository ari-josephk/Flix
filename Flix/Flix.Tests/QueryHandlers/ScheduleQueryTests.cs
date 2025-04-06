using Flix.ServiceInterface.QueryHandlers;
using Flix.ServiceInterface.Services;
using Flix.ServiceModel.Queries;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;

namespace Flix.Tests.QueryHandlers
{
	public class ScheduleQueryTests
	{
		private Mock<SchedulerService> _scheduleMock;
		private ScheduleQueryHandler _handler;

		[SetUp]
		public void Setup()
		{
			_scheduleMock = new Mock<SchedulerService>();
			_handler = new ScheduleQueryHandler(_scheduleMock.Object);
		}

		[Test]
		public async Task Get_ShouldReturnScheduledJobs_WhenJobsExist()
		{
			// Arrange
			var scheduledJobs = new List<(IJobDetail JobDetail, DateTimeOffset? LastRunTime, DateTimeOffset? NextRunTime)>
			{
				(new JobDetailImpl("job1", "", typeof(object)), DateTime.UtcNow, DateTime.UtcNow.AddHours(1)),
				(new JobDetailImpl("job2", "", typeof(object)), DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(2))
			};

			_scheduleMock.Setup(s => s.GetScheduledJobsWithRunTimesAsync()).Returns(Task.FromResult<IReadOnlyCollection<(IJobDetail JobDetail, DateTimeOffset? LastRunTime, DateTimeOffset? NextRunTime)>>(
				scheduledJobs.Select(job => (job.JobDetail, job.LastRunTime, job.NextRunTime)).ToList()));

			var query = new ScheduleQuery();

			// Act
			var result = await _handler.Get(query);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Jobs.Count());

			var job1 = result.Jobs.FirstOrDefault(j => j.Item1 == "job1");
			Assert.IsNotNull(job1);
			Assert.AreEqual("job1", job1.Item1);

			var job2 = result.Jobs.FirstOrDefault(j => j.Item1 == "job2");
			Assert.IsNotNull(job2);
			Assert.AreEqual("job2", job2.Item1);

			Assert.IsTrue(job1.Item3 > job1.Item2);
			Assert.IsTrue(job2.Item3 > job2.Item2);
		}
	}
}