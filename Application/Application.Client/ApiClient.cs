using ApiClientBaseLibrary;
using System.Runtime.CompilerServices;

namespace Application.Client;

public partial class ApiClient(
	IHttpClientFactory factory,
	ILogger<ApiClient> logger,
	BackendWorkIndicator backendWorkIndicator) : ApiClientBase(factory.CreateClient(Name), logger)
{
	public const string Name = "API";

	private readonly BackendWorkIndicator _worker = backendWorkIndicator;

	/// <summary>
	/// this should make a loading gif appear in NavMenu.razor
	/// </summary>
	protected override void OnStarted(HttpMethod method, string uri)
	{
        _worker.Start();
        Logger.LogDebug("Started: {Method} {Uri}", method, uri);		
	}

	/// <summary>
	/// makes the loading gif disappear
	/// </summary>
	protected override void OnStopped(HttpMethod method, string uri, bool success)
	{
        _worker.Stop();
        if (success) Logger.LogDebug("Success: {Method} {Uri}", method, uri);
	}

	protected override async Task<bool> ThrowExceptionAsync(HttpResponseMessage? response, Exception exception, [CallerMemberName] string? methodName = null) =>
		await Task.FromResult(false);

	// todo: add your API client calls to this class as partial classes in feature folders throughout your project
}
