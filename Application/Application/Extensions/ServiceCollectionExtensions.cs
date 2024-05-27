using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

internal static class ServiceCollectionExtensions
{
	/// <summary>
	/// help from Dave's project
	/// https://github.com/ripteqdavid/sample-blazor8-auth-ms/blob/master/BlazorAppMSAuth/BlazorAppMSAuth/Program.cs#L69
	/// </summary>
	public static void MigrateDatabase<T>(this IServiceCollection services) where T : DbContext
	{
		using var serviceProvider = services.BuildServiceProvider();
		using var scope = serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<T>();
		context.Database.Migrate();
	}
}
