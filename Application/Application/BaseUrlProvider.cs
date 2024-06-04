namespace Application;

internal class BaseUrlProvider
{
	public string BaseUrl { get; private set; } = default!;

	public void Set(string baseUrl)
	{
		BaseUrl = baseUrl;
	}
}
