using Domain.Interfaces;
using System.Security.Claims;

namespace Domain.Extensions;

public static class IUserInfoExtensions
{
    public static IEnumerable<Claim> ToClaims(this IUserInfo user)
    {
        yield return new Claim(ClaimTypes.NameIdentifier, user.Id);
        yield return new Claim(ClaimTypes.Name, user.UserName!);
        yield return new Claim(ClaimTypes.Email, user.Email!);

        if (user.TimeZoneId is not null)
        {
            yield return new Claim(nameof(ApplicationUser.TimeZoneId), user.TimeZoneId!);
        }

        if (user.PhoneNumber is not null)
        {
            yield return new Claim(ClaimTypes.MobilePhone, user.PhoneNumber!);
        }
    }

    public static IUserInfo FromClaims<T>(this ClaimsPrincipal principal) where T : IUserInfo, new() => principal.Claims.FromClaims<T>();

    public static IUserInfo FromClaims<T>(this IEnumerable<Claim> claims) where T : IUserInfo, new() => new T()
    {
        Id = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
        UserName = claims.First(c => c.Type == ClaimTypes.Name).Value,
        Email = claims.First(c => c.Type == ClaimTypes.Email).Value,
        TimeZoneId = claims.FirstOrDefault(c => c.Type == nameof(ApplicationUser.TimeZoneId))?.Value ?? string.Empty,
        PhoneNumber = claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value ?? string.Empty
    };
}
