using Domain;
using Domain.Interfaces;

namespace Application.Client.Models;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public class UserInfo : IUserInfo
{
	public string Id { get; set; } = default!;
	public string? Email { get; set; }
	public string? UserName { get; set; }
	public string? TimeZoneId { get; set; }
	public string? PhoneNumber { get; set; }

	public static UserInfo FromApplicationUser(ApplicationUser user) => new()
	{
		Id = user.Id,
		Email = user.Email,
		UserName = user.UserName,
		TimeZoneId = user.TimeZoneId,
		PhoneNumber = user.PhoneNumber
	};
}
