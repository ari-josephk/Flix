namespace Flix.Downloaders;

public interface IDownloader<T>
{
	public Task<T?> DownloadAsync(string? entityId = null);
}
