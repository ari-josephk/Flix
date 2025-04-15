namespace Flix.ServiceInterface.Downloaders.TMDB.Settings;

public class TMDBDownloaderSettings
{
	public const string OptionsName = "DownloaderSettings:TMDB";
	public required string ApiKeyPath { get; set; }
	public required long DownloadDelayMilliseconds { get; set; }
	public int MaxPagesToDownload { get; set; }
}