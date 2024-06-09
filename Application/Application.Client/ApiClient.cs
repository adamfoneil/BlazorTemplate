using ClientHelpers;

namespace Application.Client;

public partial class ApiClient(
	IHttpClientFactory factory,
	ILogger<ApiClient> logger,
	ApiEventHandler apiEventHandler) : ApiClientBase(factory.CreateClient(Name), logger)
{
	public const string Name = "API";

	private readonly ApiEventHandler _eventHandler = apiEventHandler;

	/// <summary>
	/// this should make a loading gif appear in LoadingSpionner component
	/// </summary>
	protected override void OnStarted(HttpMethod method, string uri)
	{
		base.OnStarted(method, uri);
		_eventHandler.Start();		
	}

	/// <summary>
	/// makes the loading gif disappear
	/// </summary>
	protected override void OnStopped(HttpMethod method, string uri, bool success)
	{
		base.OnStopped(method, uri, success);
		_eventHandler.Stop();		
	}

	protected override void OnError(HttpMethod method, string uri, Exception exception)
	{
		_eventHandler.Error(exception.Message);
	}

	// todo: add your API client calls to this class as partial classes in feature folders throughout your project
}
