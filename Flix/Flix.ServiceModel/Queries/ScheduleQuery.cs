using Flix.ServiceModel.Models;
using ServiceStack;

namespace Flix.ServiceModel.Queries;

[Route("/schedule", "GET")]
public class ScheduleQuery : IReturn<StatusResponse>
{
}

public class ScheduleResponse
{
    public required IEnumerable<ScheduleJob> Jobs { get; set; }
}
