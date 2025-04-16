using Flix.ServiceInterface.QueryHandlers;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceModel.Queries;
using Moq;
using NUnit.Framework;

namespace Flix.Tests.QueryHandlers;
public class MoviesQueryTests
{
	private Mock<IMovieStore> _movieStoreMock;
	private MoviesQueryHandler _handler;

	[SetUp]
	public void SetUp()
	{
		_movieStoreMock = new Mock<IMovieStore>();
		_handler = new MoviesQueryHandler(_movieStoreMock.Object);
	}

	[Test]
	public async Task Handle_ShouldReturnMovies_WhenMoviesExist()
	{
		// Arrange
		var movies = new List<Movie>
			{
				new Movie { Id = new(), Title = "Movie 1" },
				new Movie { Id = new(), Title = "Movie 2" }
			};

		_movieStoreMock
			.Setup(repo => repo.GetAllMoviesAsync())
			.Returns(Task.FromResult<IEnumerable<Movie>>(movies));

		var query = new MoviesQuery();

		// Act
		var result = await _handler.Get(query);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Movies.Count(), Is.EqualTo(2));
		Assert.That(result.Movies.First().Title, Is.EqualTo("Movie 1"));
	}

	[TearDown]
	public void TearDown()
	{
		_handler?.Dispose();
	}
}