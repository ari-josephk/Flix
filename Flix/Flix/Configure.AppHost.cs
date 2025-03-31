using Flix.Jobs;
using Flix.Jobs.TMDB;
using Flix.ServiceInterface.QueryHandlers;
using Flix.Services;
using Quartz;

namespace Flix;

public class AppHost : AppHostBase, IHostingStartup
{
    public AppHost() : base("Flix", typeof(AppHost).Assembly)
    {
        //ServiceAssemblies.Add(typeof(StatusQueryHandler).Assembly);
		ServiceAssemblies.Add(typeof(MoviesQueryHandler).Assembly);
    }

    public override void Configure(Funq.Container container)
    {

        // Start the scheduler
        var scheduler = container.Resolve<SchedulerService>();
        scheduler.StartAsync().Wait();

        _ = scheduler.ScheduleJob<TMDBMovieCatalogDownloadJob>(new JobDataMap(), "0 0 12 * * ?");

        _ = scheduler.ScheduleJob<UniversalSchedulerJob>(new JobDataMap(), "0 0 6 * * ?");
    }

    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {

        });
}