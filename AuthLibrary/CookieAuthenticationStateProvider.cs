using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace AuthLibrary;

/// <summary>
/// based on https://github.com/dotnet/blazor-samples/blob/main/8.0/BlazorWebAssemblyStandaloneWithIdentity/BlazorWasmAuth/Identity/CookieAuthenticationStateProvider.cs
/// </summary>
public abstract class CookieAuthenticationStateProvider(HttpClient httpClient, ILogger<CookieAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    private bool _authenticated = false;

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    private readonly ClaimsPrincipal _anonUser = new(new ClaimsIdentity());

    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<CookieAuthenticationStateProvider> _logger = logger;

    /// <summary>
    /// where to get the user info from, should return Claim array
    /// </summary>
    protected abstract string UserInfoEndpoint { get; }
    

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;
        var user = _anonUser;

        _logger.LogDebug("Getting user info from {UserInfoEndpoint}", UserInfoEndpoint);
        var response = await _httpClient.GetAsync(UserInfoEndpoint);

        try
        {
            response.EnsureSuccessStatusCode();
            var claims = await response.Content.ReadFromJsonAsync<Claim[]>() ?? throw new Exception("Couldn't deserialize user info");            
            user = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider)));
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to get user info");
            throw;
        }
        
        return new AuthenticationState(new ClaimsPrincipal(user));  
    }
}
