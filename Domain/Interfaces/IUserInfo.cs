namespace Domain.Interfaces;

/// <summary>
/// use this as a bridge between the Domain and the App project 
/// for ApplicationUser and whatever type is persisted by the auth state provider
/// </summary>
public interface IUserInfo
{
    string Id { get; set; }
    string? UserName { get; set; }
    string? Email { get; set; }
    string? TimeZoneId { get; set; }
    string? PhoneNumber { get; set; }
}
