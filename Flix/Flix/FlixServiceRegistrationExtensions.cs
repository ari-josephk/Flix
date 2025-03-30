using Flix.Downloaders;
using Flix.Downloaders.TMDB;
using Flix.ServiceInterface.QueryHandlers;
using Flix.Services;
using Flix.Stores;

namespace Flix
{
	public static class FlixServiceRegistrationExtensions
	{
		public static IServiceCollection AddFlixServices(this IServiceCollection services)
		{
			services.AddSingleton<IMovieStore, MovieStore>();
			services.AddSingleton<SchedulerService>();

			services.AddLogging();

			// Add Downloaders
			services.AddTransient<TMDBMovieDownloader>();
			services.AddTransient<TMDBMovieCatalogDownloader>();
			return services;
		}
	}
}