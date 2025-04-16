namespace Flix.ServiceInterface.Downloaders;

public interface IDownloader<T>
{
	public Task<T?> DownloadAsync(string? entityId = null);
}
