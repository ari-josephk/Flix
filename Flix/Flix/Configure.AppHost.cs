using Flix.ServiceInterface;

namespace Flix;

public class AppHost : AppHostBase, IHostingStartup
{
    public AppHost() : base("Flix", typeof(AppHost).Assembly) {
        ServiceAssemblies.Add(typeof(StatusQueryHandler).Assembly);
    }

    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {
            // Configure ASP.NET Core IOC Dependencies
        });
}