using Moq;
using NUnit.Framework;
using Flix.ServiceInterface.Stores;
using Microsoft.Extensions.Logging;
using Flix.ServiceInterface.Jobs;
using Flix.ServiceInterface.Services;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Flix.ServiceInterface.Jobs.TMDB;
using Quartz;
using Flix.ServiceInterface.JobData;
using Quartz.Impl;

namespace Flix.Tests.Jobs;

[TestFixture]
public class UniversalSchedulerJobTests
{
	private Mock<IMovieStore> _movieStoreMock;
	private Mock<ILogger<UniversalSchedulerJob>> _loggerMock;
	private Mock<ISchedulerService> _schedulerServiceMock;

	private UniversalSchedulerJob _job;

	[SetUp]
	public void Setup()
	{
		_movieStoreMock = new Mock<IMovieStore>();
		_loggerMock = new Mock<ILogger<UniversalSchedulerJob>>();
		_schedulerServiceMock = new Mock<ISchedulerService>();

		_job = new UniversalSchedulerJob(_schedulerServiceMock.Object, _loggerMock.Object, _movieStoreMock.Object);
	}

	[Test]
	public async Task Execute_ShouldScheduleJobs_WhenMoviesExist()
	{
		// Arrange
		var movies = new List<Movie>
		{
			new Movie { Id = new(), Title = "Movie 1", IsProcessed = false, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "123" } } },
			new Movie { Id = new(), Title = "Movie 2", IsProcessed = false, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "456" } } }
		};

		_movieStoreMock.Setup(m => m.GetAllMoviesAsync()).ReturnsAsync(movies);

		// Act
		await _job.Execute(null);

		// Assert
		_schedulerServiceMock.Verify(s => s.ScheduleOneTimeJob<TMDBMovieDownloadJob>(
			It.Is<JobDataMap>(data => data[DownloadJobParameters.EntityId.ToString()].ToString() == "123"),
			It.Is<TimeSpan>(time => time <= TimeSpan.FromMinutes(1))), Times.Once);

		_schedulerServiceMock.Verify(s => s.ScheduleOneTimeJob<TMDBMovieDownloadJob>(
			It.Is<JobDataMap>(data => data[DownloadJobParameters.EntityId.ToString()].ToString() == "456"),
			It.Is<TimeSpan>(time => time <= TimeSpan.FromMinutes(1))), Times.Once);
	}

	[Test]
	public async Task Execute_ShouldNotScheduleJobs_WhenNoMoviesExist()
	{
		// Arrange
		var movies = new List<Movie>();

		_movieStoreMock.Setup(m => m.GetAllMoviesAsync()).ReturnsAsync(movies);

		// Act
		await _job.Execute(null);

		// Assert
		_schedulerServiceMock.Verify(s => s.ScheduleOneTimeJob<TMDBMovieDownloadJob>(It.IsAny<JobDataMap>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[Test]
	public async Task Execute_ShouldNotScheduleJobs_WhenMoviesAreProcessed()
	{
		// Arrange
		var movies = new List<Movie>
		{
			new Movie { Id = new(), Title = "Movie 1", IsProcessed = true, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "123" } } },
			new Movie { Id = new(), Title = "Movie 2", IsProcessed = true, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "456" } } }
		};

		_movieStoreMock.Setup(m => m.GetAllMoviesAsync()).ReturnsAsync(movies);

		// Act
		await _job.Execute(null);

		// Assert
		_schedulerServiceMock.Verify(s => s.ScheduleOneTimeJob<TMDBMovieDownloadJob>(It.IsAny<JobDataMap>(), It.IsAny<TimeSpan>()), Times.Never);
	}

}
