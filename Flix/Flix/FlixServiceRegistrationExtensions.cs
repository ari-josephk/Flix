using Flix.ServiceInterface;

namespace Flix
{
	public static class FlixServiceRegistrationExtensions
	{
		public static IServiceCollection AddFlixServices(this IServiceCollection services)
		{
			services.Configure<HostConfig>(config =>
			{
				config.DefaultRedirectPath = "/metadata";
			});

			return services;
		}
	}
}