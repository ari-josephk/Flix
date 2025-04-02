using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.Services;
using Flix.ServiceInterface.Stores;

using Quartz;

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

			services.AddQuartz();

			return services;
		}
	}
}