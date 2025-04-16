using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.JobData;
using Flix.ServiceInterface.Jobs.TMDB;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Flix.Tests.Jobs.TMDB;

public class TMDBMovieDownloadJobTests 
{
	private Mock<IMovieStore> _movieStoreMock;
	private Mock<ILogger<TMDBMovieDownloadJob>> _loggerMock;
	private Mock<TMDBMovieDownloader> _downloaderMock;
	private TMDBMovieDownloadJob _job;

	private IJobExecutionContext _context;

	[SetUp]
	public void Setup()
	{
		_movieStoreMock = new Mock<IMovieStore>();
		_loggerMock = new Mock<ILogger<TMDBMovieDownloadJob>>(MockBehavior.Loose);
		_downloaderMock = new Mock<TMDBMovieDownloader>(null!);

		_job = new TMDBMovieDownloadJob(_movieStoreMock.Object, _downloaderMock.Object, _loggerMock.Object);

		var jobDetail = JobBuilder.Create<TMDBMovieDownloadJob>()
			.UsingJobData(DownloadJobParameters.EntityId.ToString(), "123")
			.Build();

		_context = Mock.Of<IJobExecutionContext>(ctx => ctx.JobDetail == jobDetail);
	}
	
	[Test]
	public async Task Execute_ShouldDownloadAndStoreMovie_WhenMovieExists()
	{
		// Arrange
		var movie = new Movie { Id = new(), Title = "Movie 1", IsProcessed = false, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "123" } } };

		_downloaderMock.Setup(d => d.DownloadAsync(It.IsAny<string>())).ReturnsAsync(movie);

		// Act
		await _job.Execute(_context);

		// Assert
		_movieStoreMock.Verify(m => m.UpdateMovieByProviderIdAsync(movie, Provider.TMDB), Times.Once);
	}

	[Test]
	public async Task Execute_ShouldThrowException_WhenMovieNotFound()
	{
		// Arrange
		_downloaderMock.Setup(d => d.DownloadAsync(It.IsAny<string>())).ReturnsAsync((Movie)null);

		// Act & Assert
		var ex = Assert.Throws<Exception>(() => _job.Execute(_context).GetAwaiter().GetResult());
		Assert.That(ex.Message, Is.EqualTo("Error downloading movie from TMDB."));
	}

	[Test]
	public async Task Execute_ShouldLogWarning_WhenMovieIsNotProcessed()
	{
		// Arrange
		var movie = new Movie { Id = new(), Title = "Movie 1", IsProcessed = false, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "123" } } };

		_downloaderMock.Setup(d => d.DownloadAsync(It.IsAny<string>())).ReturnsAsync(movie);

		// Act
		await _job.Execute(_context);

		// Assert
		_loggerMock.Verify(
			l => l.Log(
				LogLevel.Warning,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Movie {movie.Title} is not processed fully after download."),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
			Times.Once);
	}
}