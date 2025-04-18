using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.Jobs.TMDB;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceInterface.Stores.ProviderMappings;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Flix.Tests.Jobs.TMDB;

public class TMDBMovieCatalogDownloadJobTests 
{
	private Mock<IMovieStore> _movieStoreMock;
	private Mock<ILogger<TMDBMovieCatalogDownloadJob>> _loggerMock;
	private Mock<TMDBMovieCatalogDownloader> _downloaderMock;
	private TMDBMovieCatalogDownloadJob _job;

	[SetUp]
	public void Setup()
	{
		_movieStoreMock = new Mock<IMovieStore>();
		_loggerMock = new Mock<ILogger<TMDBMovieCatalogDownloadJob>>();
		_downloaderMock = new Mock<TMDBMovieCatalogDownloader>(null!);

		_job = new TMDBMovieCatalogDownloadJob(_movieStoreMock.Object, _downloaderMock.Object, _loggerMock.Object);
	}

	[Test]
	public async Task Execute_ShouldDownloadAndStoreMovies_WhenMoviesExist()
	{
		// Arrange
		var movies = new List<Movie>
		{
			new Movie { Id = new(), Title = "Movie 1", IsProcessed = false, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "123" } } },
			new Movie { Id = new(), Title = "Movie 2", IsProcessed = false, ProviderIds = new Dictionary<Provider, string> { { Provider.TMDB, "456" } } }
		};

		_downloaderMock.Setup(d => d.DownloadAsync(null)).ReturnsAsync(movies);

		// Act
		await _job.Execute(null);

		// Assert
		foreach (var movie in movies)
		{
			_movieStoreMock.Verify(m => m.UpdateMovieByProviderIdAsync(movie, Provider.TMDB), Times.Once);
		}
	}

	[Test]
	public async Task Execute_ShouldThrowException_WhenNoMoviesFound()
	{
		// Arrange
		_downloaderMock.Setup(d => d.DownloadAsync(null)).ReturnsAsync(new List<Movie>());

		// Act & Assert
		var ex = Assert.Throws<Exception>(() => _job.Execute(null).GetAwaiter().GetResult());
		Assert.That(ex.Message, Is.EqualTo("Error downloading movie catalog from TMDB."));
	}
	
	[Test]
	public async Task Execute_ShouldThrowException_WhenErrorResponseFromTMDB()
	{
		// Arrange
		_downloaderMock.Setup(d => d.DownloadAsync(null)).ThrowsAsync(new Exception("TMDB error"));

		// Act & Assert
		var ex = Assert.Throws<Exception>(() => _job.Execute(null).GetAwaiter().GetResult());
		Assert.That(ex.Message, Is.EqualTo("Error downloading movie catalog from TMDB."));
	}
}