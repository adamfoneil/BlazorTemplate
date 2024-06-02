using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace AuthLibrary;

/// <summary>
/// based on https://github.com/dotnet/blazor-samples/blob/main/8.0/BlazorWebAssemblyStandaloneWithIdentity/BlazorWasmAuth/Identity/CookieAuthenticationStateProvider.cs
/// I did this to address an issue with PersistentAuthenticationStateProvider -- it appeared that its PersistentComponentState.TryTakeFromJson was not working
/// for some reason, so I went this route. However while this one works, it doesn't address the root issue.
/// </summary>
public abstract class CookieAuthenticationStateProvider<TUserInfo>(HttpClient httpClient, ILogger<CookieAuthenticationStateProvider<TUserInfo>> logger) : AuthenticationStateProvider
{
    private bool _authenticated = false;

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    private readonly ClaimsPrincipal _anonUser = new(new ClaimsIdentity());
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<CookieAuthenticationStateProvider<TUserInfo>> _logger = logger;

    /// <summary>
    /// where to get the user info from, should return TUserInfo
    /// </summary>
    protected abstract string UserInfoEndpoint { get; }

    /// <summary>
    /// how do we convert TUserInfo to a set of claims?
    /// </summary>
    protected abstract IEnumerable<Claim> GetClaims(TUserInfo userInfo);

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;
        var user = _anonUser;

        _logger.LogDebug("Getting user info from {UserInfoEndpoint}", UserInfoEndpoint);
        var response = await _httpClient.GetAsync(UserInfoEndpoint);

        try
        {
            response.EnsureSuccessStatusCode();
            var userInfo = await response.Content.ReadFromJsonAsync<TUserInfo>() ?? throw new Exception("Couldn't deserialize user info");
            var claims = GetClaims(userInfo);
            user = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider<TUserInfo>)));
            _authenticated = true;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to get user info");
            throw;
        }

        return new AuthenticationState(new ClaimsPrincipal(user));
    }
}
