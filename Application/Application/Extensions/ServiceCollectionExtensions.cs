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

	/// <summary>
	/// you don't want the api to redirect to the login page because the returned html is not a valid API response.
    /// It's better to return a 401 status code so the client doesn't treat it as a successful response.
	/// </summary>    
	public static void DisableApiRedirectToLogin(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                {
                    context.Response.StatusCode = 401;
                }
                else
                {
                    context.Response.Redirect(context.RedirectUri);
                }
                return Task.CompletedTask;
            };
        });
    }
}
