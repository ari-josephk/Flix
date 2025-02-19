using Flix.ServiceInterface;
using Flix.Stores;

namespace Flix
{
	public static class FlixServiceRegistrationExtensions
	{
		public static IServiceCollection AddFlixServices(this IServiceCollection services)
		{
			services.AddSingleton<IMovieStore, MovieStore>();

			services.Configure<HostConfig>(config =>
			{
				config.DefaultRedirectPath = "/metadata";
			});

			return services;
		}
	}
}