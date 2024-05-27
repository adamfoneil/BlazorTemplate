using ApiClientBaseLibrary;

namespace Application.Client;

public partial class ApiClient(
    IHttpClientFactory factory,
    ILogger<ApiClient> logger,
    BackendWorkIndicator backendWorkIndicator) : ApiClientBase(factory.CreateClient(Name), logger)
{
    public const string Name = "API";

    private readonly BackendWorkIndicator _worker = backendWorkIndicator;

    /// <summary>
    /// this makes a loading gif appear
    /// </summary>
    protected override void OnStarted() => _worker.Start();

    /// <summary>
    /// makes the loading gif disappear
    /// </summary>
    protected override void OnStopped() => _worker.Stop();

    // todo: add your API client calls to this class as partial classes in feature folders throughout your project
}
