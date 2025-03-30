using ServiceStack;
using Flix.ServiceModel.Queries;

namespace Flix.ServiceInterface.QueryHandlers;

public class StatusQueryHandler : Service
{
	public static StatusResponse Get(StatusQuery query)
	{
		var temp = query; 

		return new StatusResponse
		{
			Status = "Service is running",
			Message = "Free Watermelons for everyone!"
		};
	}
}
