using ServiceStack;

namespace Flix.ServiceModel.Queries;

[Route("/schedule", "GET")]
public class ScheduleQuery : IReturn<StatusResponse>
{
}

public class ScheduleResponse
{
    public required IEnumerable<(string, DateTimeOffset?, DateTimeOffset?)> Jobs { get; set; }
}
