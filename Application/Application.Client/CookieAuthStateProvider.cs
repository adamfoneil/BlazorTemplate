using AuthLibrary;

namespace Application.Client;

public class CookieAuthStateProvider(
    IHttpClientFactory factory, 
    ILogger<CookieAuthenticationStateProvider> logger) : CookieAuthenticationStateProvider(factory.CreateClient(ApiClient.Name), logger)
{
    protected override string UserInfoEndpoint => "/api/userinfo";
}
