using Microsoft.Extensions.DependencyInjection;

namespace ClientHelpers;

public static class ServiceCollectionExtensions
{
	public static void AddHttpClient<THandler>(this IServiceCollection services, string clientName, Func<IServiceProvider, HttpClient, string> baseUrlAccessor) where THandler : DelegatingHandler
	{
		services
			.AddTransient<THandler>()
			.AddSingleton(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName))
			.AddHttpClient(clientName, (sp, client) =>
			{
				client.BaseAddress = new Uri(baseUrlAccessor(sp, client));
			}).AddHttpMessageHandler<THandler>();
	}
}
