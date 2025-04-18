using Flix.ServiceInterface.QueryHandlers;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceModel.Queries;
using Moq;
using NUnit.Framework;
using ServiceStack;

namespace Flix.Tests.QueryHandlers;
public class MovieQueryTests
{
	private Mock<IMovieStore> _movieStoreMock;
	private MovieQueryHandler _handler;

	[SetUp]
	public void SetUp()
	{
		_movieStoreMock = new Mock<IMovieStore>();
		_handler = new MovieQueryHandler(_movieStoreMock.Object);
	}

	[Test]
	public async Task Handle_ShouldReturnMovie_WhenMovieExists()
	{
		// Arrange
		var movieId = Guid.NewGuid().ToString("N")[..24];
		var bsonId = MongoDB.Bson.ObjectId.Parse(movieId);
		var movie = new Movie { Id = bsonId, Title = "Movie 1" };

		_movieStoreMock
			.Setup(repo => repo.GetMovieByIdAsync(bsonId))
			.Returns(Task.FromResult(movie!));

		var query = new MovieQuery { Id = movieId };

		// Act
		var result = await _handler.Get(query);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Movie, Is.Not.Null);
		Assert.That(result.Movie?.Title, Is.EqualTo("Movie 1"));
	}

	[Test]
	public void Handle_ShouldThrowNotFound_WhenMovieDoesNotExist()
	{
		// Arrange
		var movieId = Guid.NewGuid().ToString("N")[..24];
		var bsonId = MongoDB.Bson.ObjectId.Parse(movieId);

		_movieStoreMock
			.Setup(repo => repo.GetMovieByIdAsync(bsonId))
			.Returns(Task.FromResult<Movie?>(null) as Task<Movie>);

		var query = new MovieQuery { Id = movieId };

		// Act & Assert
		var ex = Assert.ThrowsAsync<HttpError>(async () => await _handler.Get(query));
		Assert.That(ex.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
	}

	[TearDown]
	public void TearDown()
	{
		_handler?.Dispose();
	}
}