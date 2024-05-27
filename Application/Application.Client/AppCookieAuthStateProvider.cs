using Application.Client.Models;
using AuthLibrary;
using System.Security.Claims;
using Domain.Extensions;

namespace Application.Client;

public class AppCookieAuthStateProvider(
    IHttpClientFactory factory, 
    ILogger<AppCookieAuthStateProvider> logger) : CookieAuthenticationStateProvider<UserInfo>(factory.CreateClient(ApiClient.Name), logger)
{
    protected override string UserInfoEndpoint => "/api/userinfo";

    protected override IEnumerable<Claim> GetClaims(UserInfo userInfo) => userInfo.ToClaims();
    
}
