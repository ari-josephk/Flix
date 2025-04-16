using Flix.ServiceInterface.Jobs;
using Flix.ServiceInterface.Jobs.TMDB;
using Flix.ServiceInterface.QueryHandlers;
using Flix.ServiceInterface.Services;
using Flix.ServiceInterface.Stores.ProviderMappings;
using MongoDB.Bson.Serialization.Serializers;
using Quartz;

namespace Flix;

public class AppHost : AppHostBase, IHostingStartup
{
    public AppHost() : base("Flix", typeof(AppHost).Assembly)
    {
        ServiceAssemblies.Add(typeof(StatusQueryHandler).Assembly);
        ServiceAssemblies.Add(typeof(ScheduleQueryHandler).Assembly);
        ServiceAssemblies.Add(typeof(MoviesQueryHandler).Assembly);
    }

    public override void Configure(Funq.Container container)
    {
        // Mongo Configuration
        MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new EnumSerializer<Provider>(MongoDB.Bson.BsonType.String));

        // Start the scheduler
        var scheduler = container.Resolve<ISchedulerService>();
        scheduler.StartAsync().Wait();
        Console.WriteLine("Scheduler started.");
        // Schedule permanent jobs
        // TODO: Make this configurable
        _ = scheduler.ScheduleJob<TMDBMovieCatalogDownloadJob>(new JobDataMap(), TimeSpan.FromDays(1));

        // Start scheduling jobs automatically after one minute
        _ = scheduler.ScheduleJob<UniversalSchedulerJob>(new JobDataMap(), TimeSpan.FromDays(1));
    }

    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {

        });
}