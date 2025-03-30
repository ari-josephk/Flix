using ServiceStack;

namespace Flix.ServiceModel.Queries;

[Route("/status", "GET")]
public class StatusQuery : IReturn<StatusResponse>
{
}

public class StatusResponse
{
    public required string Status { get; set; }
    public required string Message { get; set; }
}
