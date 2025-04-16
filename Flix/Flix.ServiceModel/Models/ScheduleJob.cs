using System;

namespace Flix.ServiceModel.Models;

public class ScheduleJob
{
	public string JobName { get; set; } = string.Empty;
	public DateTimeOffset? LastRunTime { get; set; }
	public DateTimeOffset? NextRunTime { get; set; }
}
