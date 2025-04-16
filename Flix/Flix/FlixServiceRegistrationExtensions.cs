using Flix.ServiceInterface.Downloaders.TMDB;
using Flix.ServiceInterface.Downloaders.TMDB.Settings;
using Flix.ServiceInterface.Services;
using Flix.ServiceInterface.Settings;
using Flix.ServiceInterface.Stores;

using Quartz;

namespace Flix
{
	public static class FlixServiceRegistrationExtensions
	{
		public static IServiceCollection AddFlixServices(this IServiceCollection services)
		{
			services.AddSingleton<IMovieStore, MovieStore>();
			services.AddSingleton<ISchedulerService, SchedulerService>();

			services.AddLogging();

			// Add Downloaders
			services.AddTransient<TMDBMovieDownloader>();
			services.AddTransient<TMDBMovieCatalogDownloader>();

			services.AddQuartz();

			return services;
		}

		public static IServiceCollection AddFlixSettings(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<FlixDatabaseSettings>(
				configuration.GetSection(nameof(FlixDatabaseSettings)));

			services.Configure<TMDBDownloaderSettings>(
				configuration.GetSection(TMDBDownloaderSettings.OptionsName));

			return services;
		}
	}
}