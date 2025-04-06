using Flix.ServiceInterface.QueryHandlers;
using Flix.ServiceInterface.Stores;
using Flix.ServiceInterface.Stores.Models;
using Flix.ServiceModel.Queries;
using Moq;
using NUnit.Framework;

namespace Flix.Tests.QueryHandlers
{
	public class MoviesQueryTests
	{
		private Mock<IMovieStore> _movieStoreMock;
		private MoviesQueryHandler _handler;

		[SetUp]
		public void SetUp()
		{
			_movieStoreMock = new Mock<IMovieStore>();
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
			var result = _handler.Get(query);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Movies.Count());
			Assert.AreEqual("Movie 1", result.Movies.First().Title);
		}
	}
}