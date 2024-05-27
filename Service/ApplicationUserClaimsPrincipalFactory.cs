using Domain;
using Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Service;

public class ApplicationUserClaimsPrincipalFactory(
	UserManager<ApplicationUser> userManager,
	IOptions<IdentityOptions> optionsAccessor) : UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor)
{
	protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
	{
		var identity = await base.GenerateClaimsAsync(user);
		foreach (var claim in user.ToClaims()) identity.AddClaim(claim);
		return identity;
	}
}